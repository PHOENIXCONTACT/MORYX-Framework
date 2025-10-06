// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Logging;
using Moryx.Logging;
using Moryx.Tools;
using System.Globalization;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Moryx.ProcessData.SpreadsheetsListener.Tests")]
namespace Moryx.ProcessData.SpreadsheetsListener
{
    internal class CsvStructure
    {
        private readonly IList<string> _header = new List<string>();
        private readonly ICollection<CsvRow> _rows = new List<CsvRow>();
        private object _headerLock = new();
        private object _rowsLock = new();

        private string _path;
        private string _measurand;
        private string _completeFilePath;

        private CsvConfiguration _csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture);
        private IModuleLogger _logger;

        private int _writtenRows = 0;
        private int _maxNumOfRows;

        private FileStream _fileStream;
        private StreamWriter _streamWriter;
        private CsvWriter _csvWriter;

        internal CsvStructure(SpreadsheetsListenerConfig config, string measurand, IModuleLogger logger)
        {
            _path = config.Path;
            _measurand = measurand;
            _maxNumOfRows = Math.Max(2, config.MaxNumOfRows);
            if (config.Delimiter != null)
                _csvConfig.Delimiter = config.Delimiter;
            _logger = logger;
            CreateNewFile();
        }

        public async Task AddMeasurement(Measurement measurement)
        {
            IDictionary<string, object> record = MeasurementToDictionary(measurement);
            var header = record.Keys.ToList();
            var row = new CsvRow(record.Values.ToList());
            await AdjustHeaderAddRow(header, row);
        }

        private async Task AdjustHeaderAddRow(IList<string> header, CsvRow row)
        {
            List<string> tempHeader;
            lock (_headerLock)
                tempHeader = _header.ToList();

            var addHeadings = header.Except(tempHeader);
            if (addHeadings.Count() > 0)
            {
                lock (_headerLock)
                    _header.AddRange(addHeadings);
                await OverwriteHeader();
                if (_writtenRows == 0)
                    _writtenRows++;
                OpenFile();
            }

            CsvRow csvRow;
            lock (_headerLock)
            {
                var rowArray = new object[_header.Count];
                for (int i = 0; i < header.Count; i++)
                {
                    var heading = header[i];
                    var index = _header.IndexOf(heading);
                    rowArray[index] = row.ElementAt(i);
                }
                csvRow = new CsvRow(rowArray);
            }
            lock (_rowsLock)
                _rows.Add(csvRow);
        }

        public void Close()
        {
            if (_csvWriter != null && _fileStream != null && _fileStream.CanWrite)
                _csvWriter.Flush();
            if (_streamWriter != null)
            {
                _streamWriter.Close();
            }
        }

        private void CreateNewFile()
        {
            var fullFileName = _measurand + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmssfff");
            _completeFilePath = Path.Combine(_path, fullFileName + ".csv");

            //if (!Directory.Exists(_filePath))
            Directory.CreateDirectory(_path);
            _fileStream = (File.Create(_completeFilePath));
            _writtenRows = 0;
            _streamWriter = new StreamWriter(_fileStream);
            _csvWriter = new CsvWriter(_streamWriter, _csvConfig);
        }

        private void OpenFile()
        {
            _fileStream = File.Open(_completeFilePath, FileMode.Append);
            _streamWriter = new StreamWriter(_fileStream);
            _csvWriter = new CsvWriter(_streamWriter, _csvConfig);
        }

        private IDictionary<string, object> MeasurementToDictionary(Measurement measurement)
        {
            try
            {
                Dictionary<string, object> measurementDictionary = new Dictionary<string, object>();
                var tStamp = measurement.TimeStamp.ToUniversalTime();
                measurementDictionary.Add("time", tStamp);

                SortedDictionary<string, object> sortedDictionary = new SortedDictionary<string, object>();
                measurement.Fields.ForEach(f => sortedDictionary.Add(f.Name, f.Value));
                measurement.Tags.ForEach(f => sortedDictionary.Add(f.Name, f.Value));
                var currentPlatform = Platform.Current;
                if (currentPlatform != null)
                    sortedDictionary.Add("application", currentPlatform.ProductName);

                // time stamps should be in the first column
                foreach (var element in sortedDictionary)
                    measurementDictionary.Add(element.Key, element.Value);

                return measurementDictionary;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while transforming measurement: {0}.", measurement);
                throw;
            }
        }

        private async Task OverwriteHeader()
        {
            List<string> tempHeader;
            lock (_headerLock)
                tempHeader = _header.ToList();

            Close();
            string tempfile = Path.GetTempFileName();
            using (var writer = new StreamWriter(tempfile))
            using (var reader = new StreamReader(_completeFilePath))
            using (var csv = new CsvWriter(writer, _csvConfig))
            {
                _logger.Log(LogLevel.Debug, "Overwriting header: Writing {0} headings to header of spreadsheet '{1}'", tempHeader.Count, Path.GetFileName(_completeFilePath));
                foreach (var heading in tempHeader)
                {
                    csv.WriteField(heading);
                }
                await csv.NextRecordAsync();
                await reader.ReadLineAsync();
                while (!reader.EndOfStream)
                    await writer.WriteLineAsync(reader.ReadLine());
            }
            File.Copy(tempfile, _completeFilePath, true);
            File.Delete(tempfile);
        }

        private async Task WriteHeader()
        {
            List<string> tempHeader;
            lock (_headerLock)
                tempHeader = _header.ToList();

            _logger.Log(LogLevel.Debug, "Writing {0} headings to header of spreadsheet '{1}'", tempHeader.Count, Path.GetFileName(_completeFilePath));
            foreach (var heading in tempHeader)
            {
                _csvWriter.WriteField(heading);
            }
            await _csvWriter.NextRecordAsync();
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
                _logger.Log(LogLevel.Debug, "No measurements for {0} to be written", _measurand);
                return;
            }
            try
            {
                int availableNumOfRows = _maxNumOfRows - _writtenRows;
                var rowsToCurrentFile = tempRows.GetRange(0, Math.Min(availableNumOfRows, tempRows.Count));

                _logger.Log(LogLevel.Debug, "Writing {0} records to spreadsheet '{1}'", tempRows.Count, Path.GetFileName(_completeFilePath));
                foreach (var row in rowsToCurrentFile)
                {
                    foreach (var field in row.Fields)
                    {
                        _csvWriter.WriteField(field);
                    }
                    await _csvWriter.NextRecordAsync();
                    _writtenRows++;
                    if (!tempRows.Remove(row))
                    {
                        _logger.Log(LogLevel.Debug, "Failed to remove row from '{0}' queue. Row was not found in queue", _measurand);
                    }
                }

                if (tempRows.Count > 0)
                {
                    lock (_rowsLock)
                        _rows.AddRange(tempRows);
                    Close();
                    CreateNewFile();
                    await WriteHeader();
                    await WriteRows();
                }
                else
                    await _csvWriter.FlushAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while writing records to spreadsheet '{0}'. Moving records back to queue.", Path.GetFileName(_completeFilePath));
            }
        }
    }
}

