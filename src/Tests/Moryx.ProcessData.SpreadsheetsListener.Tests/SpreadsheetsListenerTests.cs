// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Globalization;
using CsvHelper;
using Microsoft.Extensions.Logging.Abstractions;
using Moryx.Logging;
using Moryx.TestTools.UnitTest;
using NUnit.Framework;

namespace Moryx.ProcessData.SpreadsheetsListener.Tests;

[TestFixture]
sealed class SpreadsheetsListenerTests
{
    private SpreadsheetsListener _spreadsheetsListener;
    private readonly string _path = Path.Combine(Path.GetTempPath(), nameof(CsvStructure));
    private SpreadsheetsListenerConfig _config;
    private NotSoParallelOps _notSoParallelOps;

    [SetUp]
    public void SetUp()
    {
        _notSoParallelOps = new NotSoParallelOps();

        _config = new SpreadsheetsListenerConfig();
        _config.Path = _path;
        _config.ReportIntervalMs = 100;
        _config.Separator = ";";

        _spreadsheetsListener = new SpreadsheetsListener();
        _spreadsheetsListener.ParallelOperations = _notSoParallelOps;
        var logger = new ModuleLogger("Dummy", new NullLoggerFactory());
        _spreadsheetsListener.Logger = logger;
        _spreadsheetsListener.Initialize(_config);
    }

    [Test]
    public void ShouldWriteMeasurementsToTwoFiles()
    {
        // Arrange
        var measurementName = Guid.NewGuid().ToString();
        const int maxNumOfRows = 6;
        const int numOfMeasurements = 2 * (maxNumOfRows - 1);
        _config.MeasurandConfigs.Add(new Listener.MeasurandConfig()
        {
            Name = measurementName,
            IsEnabled = true
        });
        _config.MaxNumOfRows = maxNumOfRows;

        for (var i = 0; i < numOfMeasurements; i++)
        {
            var measurement = new Measurement(measurementName);
            measurement.Add(new DataField("A", i));
            measurement.Add(new DataTag("B", i.ToString(CultureInfo.InvariantCulture)));

            _spreadsheetsListener.MeasurementAdded(measurement);
        }

        // Act
        _spreadsheetsListener.Stop();

        // Assert
        var files = TestDirectory.GetOrderedFiles(_path, measurementName + "*");
        Assert.That(files, Has.Length.EqualTo(2), $"{files.Length} files were created but two were expected");
        Assert.That(ReadInCsv(files[0]), Has.Count.EqualTo(maxNumOfRows), $"{maxNumOfRows} lines in csv file were expected");
        Assert.That(ReadInCsv(files[1]), Has.Count.EqualTo(maxNumOfRows), $"{maxNumOfRows} lines in csv file were expected");
    }

    [Test]
    public void ShouldWriteTwoFilesWithSameContent()
    {
        // Arrange
        const int maxNumOfRows = 6;
        const int numOfMeasurements = maxNumOfRows - 1;

        var firstMeasurementName = Guid.NewGuid().ToString();
        var secondMeasurementName = Guid.NewGuid().ToString();
        foreach (var measurementName in new string[] { firstMeasurementName, secondMeasurementName })
        {
            _config.MeasurandConfigs.Add(new Listener.MeasurandConfig()
            {
                Name = measurementName,
                IsEnabled = true
            });
        }
        _config.MaxNumOfRows = maxNumOfRows;

        for (var i = 0; i < numOfMeasurements; i++)
        {
            var df = new DataField("A", i);
            var dt = new DataTag("B", i.ToString(CultureInfo.InvariantCulture));
            var timeStamp = DateTime.Now;

            var firstMeasurement = new Measurement(firstMeasurementName, timeStamp);
            firstMeasurement.Add(df);
            firstMeasurement.Add(dt);
            _spreadsheetsListener.MeasurementAdded(firstMeasurement);

            var secondMeasurement = new Measurement(secondMeasurementName, timeStamp);
            secondMeasurement.Add(df);
            secondMeasurement.Add(dt);
            _spreadsheetsListener.MeasurementAdded(secondMeasurement);
        }

        // Act
        _spreadsheetsListener.Stop();

        // Assert
        var firstFile = Directory.GetFiles(_path, firstMeasurementName + "*").Single();
        var firstFileContent = ReadInCsv(firstFile);
        var secondFile = Directory.GetFiles(_path, secondMeasurementName + "*").Single();
        var secondFileContent = ReadInCsv(secondFile);

        Assert.That(firstFileContent, Has.Count.EqualTo(secondFileContent.Count), "Files do not have the same number of rows");

        for (var i = 0; i < firstFileContent.Count; i++)
        {
            Assert.That(firstFileContent[i].SequenceEqual(secondFileContent[i]), $"Row {i} was modified");
        }
    }

    [Test]
    public void ShouldOverwriteHeader()
    {
        // Arrange
        var measurementName = Guid.NewGuid().ToString();
        var maxNumOfRows = 6;
        var numOfMeasurements = maxNumOfRows - 1;
        _config.MeasurandConfigs.Add(new Listener.MeasurandConfig()
        {
            Name = measurementName,
            IsEnabled = true
        });
        _config.MaxNumOfRows = maxNumOfRows;

        var ogMeasurements = new List<Measurement>();
        for (var i = 0; i < numOfMeasurements - 1; i++)
        {
            var measurement = new Measurement(measurementName);
            measurement.Add(new DataField("B", i));
            measurement.Add(new DataTag("D", i.ToString(CultureInfo.InvariantCulture)));
            ogMeasurements.Add(measurement);
            _spreadsheetsListener.MeasurementAdded(measurement);
        }
        _spreadsheetsListener.Stop();
        var ogFile = Directory.GetFiles(_path, measurementName + "*").Single();
        var ogFileContent = ReadInCsv(ogFile);
        var ogHeader = ogFileContent[0];
        File.Delete(ogFile);

        // Act
        _spreadsheetsListener.Start();
        foreach (var ogMeas in ogMeasurements)
        {
            _spreadsheetsListener.MeasurementAdded(ogMeas);
        }

        var measurementWithNewKeys = new Measurement(measurementName);
        measurementWithNewKeys.Add(new DataField("A", numOfMeasurements));
        measurementWithNewKeys.Add(new DataTag("C", numOfMeasurements.ToString(CultureInfo.InvariantCulture)));

        _spreadsheetsListener.MeasurementAdded(measurementWithNewKeys);
        _spreadsheetsListener.Stop();

        // Assert
        var changedHdrFile = Directory.GetFiles(_path, measurementName + "*").Single();
        var changedHdrFileContent = ReadInCsv(changedHdrFile);
        Assert.That(changedHdrFileContent, Has.Count.EqualTo(maxNumOfRows), $"{maxNumOfRows} lines in csv file were expected");

        Assert.That(changedHdrFileContent[0].SequenceEqual(ogHeader.Concat(new List<string>() { "A", "C" })), "Header was not changed properly");
        for (var i = 1; i < ogFileContent.Count; i++)
        {
            Assert.That(changedHdrFileContent[i].SequenceEqual(ogFileContent[i]), $"Row {i} was modified");
        }

        var lastRowValid = changedHdrFileContent[numOfMeasurements]
            .GetRange(ogHeader.Count - 2, 4)
            .SequenceEqual(new List<string>()
                {
                    "","", numOfMeasurements.ToString(CultureInfo.InvariantCulture), numOfMeasurements.ToString(CultureInfo.InvariantCulture)
                }
            );
        Assert.That(lastRowValid, "Measurement with new keys was not written properly to file");
    }

    [Test]
    public void ShouldWriteDifferentDataTypesToFile()
    {
        // Arrange
        var measurementName = Guid.NewGuid().ToString();
        var maxNumOfRows = 3;
        _config.MeasurandConfigs.Add(new Listener.MeasurandConfig()
        {
            Name = measurementName,
            IsEnabled = true
        });
        _config.MaxNumOfRows = maxNumOfRows;

        var measurement1 = new Measurement(measurementName);
        measurement1.Add(new DataField("A", "XYZ"));
        measurement1.Add(new DataField("B", 1234));
        measurement1.Add(new DataField("C", 12.34));
        _spreadsheetsListener.MeasurementAdded(measurement1);

        var measurement2 = new Measurement(measurementName);
        measurement2.Add(new DataField("A", new DateTime(1987, 12, 31, 9, 8, 59)));
        measurement2.Add(new DataField("B", (float)56.78));
        measurement2.Add(new DataField("C", '@'));
        _spreadsheetsListener.MeasurementAdded(measurement2);

        // Act
        _spreadsheetsListener.Stop();

        // Assert
        var file = Directory.GetFiles(_path, measurementName + "*").Single();
        var fileContent = ReadInCsv(file);
        var columns = fileContent[0].Count;
        var contentIsValid = fileContent[0].GetRange(columns - 3, 3).SequenceEqual(new List<string>() { "A", "B", "C" })
                             && fileContent[1].GetRange(columns - 3, 3).SequenceEqual(new List<string>() { "XYZ", "1234", "12.34" })
                             && fileContent[2].GetRange(columns - 3, 3).SequenceEqual(new List<string>() { "1987-12-31T09:08:59.0000000", "56.78", "@" }); ;
        Assert.That(contentIsValid, "Not all types were written properly to file");
    }

    private List<List<string>> ReadInCsv(string absolutePath)
    {
        var result = new List<List<string>>();
        using TextReader fileReader = File.OpenText(absolutePath);
        using var csv = CsvReader.Create(fileReader, new CsvReaderOptions
        {
            Separator = _config.Separator[0],
            // The header is part of the read lines
            HasHeader = false
        });
        while (csv.Read())
        {
            var line = new List<string>();
            for (var i = 0; i < csv.FieldCount; i++)
            {
                line.Add(csv.GetString(i));
            }
            result.Add(line);
        }

        return result;
    }
}
