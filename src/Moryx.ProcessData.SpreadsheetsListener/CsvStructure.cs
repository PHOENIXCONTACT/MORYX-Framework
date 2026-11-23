// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Globalization;
using System.Runtime.CompilerServices;
using CsvHelper;
using Microsoft.Extensions.Logging;
using Moryx.Logging;
using Moryx.Tools;

[assembly: InternalsVisibleTo("Moryx.ProcessData.SpreadsheetsListener.Tests")]
namespace Moryx.ProcessData.SpreadsheetsListener;

internal class CsvStructure
{
    private readonly IList<string> _header = new List<string>();
    private readonly ICollection<CsvRow> _rows = new List<CsvRow>();
    private readonly Lock _headerLock = new();
    private readonly Lock _rowsLock = new();

    private readonly string _path;
    private readonly string _measurand;
    private string _completeFilePath;

    private readonly CsvWriterOptions _csvOptions;
    private readonly IModuleLogger _logger;

    private int _writtenRows;
    private readonly int _maxNumOfRows;

    private FileStream _fileStream;
    private StreamWriter _streamWriter;
    private CsvWriter _csvWriter;

    internal CsvStructure(SpreadsheetsListenerConfig config, string measurand, IModuleLogger logger)
    {
        _path = config.Path;
        _measurand = measurand;
        _maxNumOfRows = Math.Max(2, config.MaxNumOfRows);
        _logger = logger;

        var separator = CsvWriterOptions.Default.Separator;
        if (!string.IsNullOrEmpty(config.Separator))
        {
            // TODO: CommandCenter does not support char for now: https://github.com/PHOENIXCONTACT/MORYX-Framework/issues/1648
            if (config.Separator.Length > 1)
            {
                _logger.LogWarning("Only single character separators are supported. Using '{UsedSeparator}' instead of '{ConfiguredSeparator}'",
                    config.Separator[0], config.Separator);
            }

            separator = config.Separator[0];
        }

        _csvOptions = new CsvWriterOptions
        {
            Separator = separator,
            // Rows may have fewer fields than the header when the header was extended later on
            ValidateFieldCount = false,
            Quoting = CsvQuoting.WhenNeededOrWhitespace
        };

        CreateNewFile();
    }

    public async Task AddMeasurement(Measurement measurement)
    {
        var record = MeasurementToDictionary(measurement);
        var header = record.Keys.ToList();
        var row = new CsvRow(record.Values.ToList());
        await AdjustHeaderAddRow(header, row);
    }

    private async Task AdjustHeaderAddRow(IList<string> header, CsvRow row)
    {
        List<string> tempHeader;
        lock (_headerLock)
        {
            tempHeader = _header.ToList();
        }

        var addHeadings = header.Except(tempHeader).ToArray();
        if (addHeadings.Length != 0)
        {
            lock (_headerLock)
            {
                _header.AddRange(addHeadings);
            }

            await OverwriteHeader();
            if (_writtenRows == 0)
            {
                _writtenRows++;
            }

            OpenFile();
        }

        CsvRow csvRow;
        lock (_headerLock)
        {
            var rowArray = new object[_header.Count];
            for (var i = 0; i < header.Count; i++)
            {
                var heading = header[i];
                var index = _header.IndexOf(heading);
                rowArray[index] = row.ElementAt(i);
            }
            csvRow = new CsvRow(rowArray);
        }
        lock (_rowsLock)
        {
            _rows.Add(csvRow);
        }
    }

    public void Close()
    {
        if (_csvWriter != null && _fileStream != null && _fileStream.CanWrite)
        {
            _csvWriter.Flush();
        }

        _streamWriter?.Close();
    }

    private void CreateNewFile()
    {
        Directory.CreateDirectory(_path);

        // Files created within the same millisecond would get the same name and overwrite each other
        var timeStamp = DateTime.UtcNow;
        do
        {
            var fullFileName = _measurand + "_" + timeStamp.ToString("yyyyMMdd_HHmmssfff", CultureInfo.InvariantCulture);
            _completeFilePath = Path.Combine(_path, fullFileName + ".csv");
            timeStamp = timeStamp.AddMilliseconds(1);
        } while (File.Exists(_completeFilePath));

        _fileStream = File.Open(_completeFilePath, FileMode.CreateNew);
        _writtenRows = 0;
        _streamWriter = new StreamWriter(_fileStream);
        _csvWriter = CsvWriter.Create(_streamWriter, _csvOptions);
    }

    private void OpenFile()
    {
        _fileStream = File.Open(_completeFilePath, FileMode.Append);
        _streamWriter = new StreamWriter(_fileStream);
        _csvWriter = CsvWriter.Create(_streamWriter, _csvOptions);
    }

    private IDictionary<string, object> MeasurementToDictionary(Measurement measurement)
    {
        try
        {
            var measurementDictionary = new Dictionary<string, object>();
            var tStamp = measurement.TimeStamp.ToUniversalTime();
            measurementDictionary.Add("time", tStamp);

            var sortedDictionary = new SortedDictionary<string, object>();
            measurement.Fields.ForEach(f => sortedDictionary.Add(f.Name, f.Value));
            measurement.Tags.ForEach(f => sortedDictionary.Add(f.Name, f.Value));
            var currentPlatform = Platform.Current;
            if (currentPlatform != null)
            {
                sortedDictionary.Add("application", currentPlatform.ProductName);
            }

            // time stamps should be in the first column
            foreach (var element in sortedDictionary)
            {
                measurementDictionary.Add(element.Key, element.Value);
            }

            return measurementDictionary;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while transforming measurement: {Measurement}.", measurement);
            throw;
        }
    }

    private async Task OverwriteHeader()
    {
        List<string> tempHeader;
        lock (_headerLock)
        {
            tempHeader = _header.ToList();
        }

        Close();
        var tempfile = Path.GetTempFileName();
        await using (var writer = new StreamWriter(tempfile))
        using (var reader = new StreamReader(_completeFilePath))
        await using (var csv = CsvWriter.Create(writer, _csvOptions, leaveOpen: true))
        {
            _logger.Log(LogLevel.Debug, "Overwriting header: Writing {HeadingCount} headings to header of spreadsheet '{FileName}'", tempHeader.Count, Path.GetFileName(_completeFilePath));
            foreach (var heading in tempHeader)
            {
                csv.WriteField(heading);
            }
            await csv.EndRecordAsync();

            // The csv writer buffers internally, so flush the header before appending the existing rows
            await csv.FlushAsync();

            // Skip the old header and copy the remaining rows
            await reader.ReadLineAsync();
            string line;
            while ((line = await reader.ReadLineAsync()) != null)
            {
                await writer.WriteLineAsync(line);
            }
        }
        File.Copy(tempfile, _completeFilePath, true);
        File.Delete(tempfile);
    }

    private async Task WriteHeader()
    {
        List<string> tempHeader;
        lock (_headerLock)
        {
            tempHeader = _header.ToList();
        }

        _logger.Log(LogLevel.Debug, "Writing {HeadingCount} headings to header of spreadsheet '{FileName}'",
            tempHeader.Count, Path.GetFileName(_completeFilePath));
        foreach (var heading in tempHeader)
        {
            _csvWriter.WriteField(heading);
        }
        await _csvWriter.EndRecordAsync();
        await _csvWriter.FlushAsync();
        _writtenRows++;
    }

    public async Task WriteRows()
    {
        List<CsvRow> tempRows;
        lock (_rowsLock)
        {
            tempRows = _rows.ToList();
            _rows.Clear();
        }
        if (tempRows.Count == 0)
        {
            _logger.Log(LogLevel.Debug, "No measurements for {Measurand} to be written", _measurand);
            return;
        }
        try
        {
            var availableNumOfRows = _maxNumOfRows - _writtenRows;
            var rowsToCurrentFile = tempRows.GetRange(0, Math.Min(availableNumOfRows, tempRows.Count));

            _logger.Log(LogLevel.Debug, "Writing {RecordCount} records to spreadsheet '{FileName}'",
                tempRows.Count, Path.GetFileName(_completeFilePath));
            foreach (var row in rowsToCurrentFile)
            {
                foreach (var field in row.Fields)
                {
                    _csvWriter.WriteField(field);
                }
                await _csvWriter.EndRecordAsync();
                _writtenRows++;
                if (!tempRows.Remove(row))
                {
                    _logger.Log(LogLevel.Debug, "Failed to remove row from '{Measurand}' queue. Row was not found in queue", _measurand);
                }
            }

            if (tempRows.Count > 0)
            {
                lock (_rowsLock)
                {
                    _rows.AddRange(tempRows);
                }

                Close();
                CreateNewFile();
                await WriteHeader();
                await WriteRows();
            }
            else
            {
                await _csvWriter.FlushAsync();
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while writing records to spreadsheet '{FileName}'. Moving records back to queue.",
                Path.GetFileName(_completeFilePath));
        }
    }
}
