// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Logging;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using Moryx.Tools;
using NUnit.Framework.Interfaces;
using Moq;

namespace Moryx.ProcessData.SpreadsheetsListener.Tests;

[TestFixture]
public class CsvStructureTest
{
    private CsvStructure _csvStructure;

    private CsvConfiguration _csvConfig = new(CultureInfo.InvariantCulture);
    private SpreadsheetsListenerConfig _slConfig;
    private IModuleLogger _logger;
    private Random _random = new();

    private readonly string _path = Path.Combine(Path.GetTempPath(), nameof(CsvStructure));
    private string _randomFileName;
    private string[] _files;
    private FileInfo _fileInfo;
    private DateTime _testTimeStamp;

    [SetUp]
    public void SetUp()
    {
        _csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture);
        _csvConfig.Delimiter = ";";
        _logger = new Mock<IModuleLogger>().Object;

        _slConfig = new SpreadsheetsListenerConfig();
        _slConfig.Path = _path;
        _slConfig.Delimiter = ";";

        _randomFileName = NewGuidString();
        _fileInfo = null;
        _files = null;
        _testTimeStamp = DateTime.Now;
    }

    [TearDown]
    public void TearDown()
    {
        if (_csvStructure != null)
        {
            _csvStructure.Close();
            _csvStructure = null;
        }

        if (TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Passed)
            foreach (var file in _files)
                File.Delete(file);
    }

    [Test]
    public void ShouldCreateEmptyCsvFile()
    {
        // Arrange

        // Act
        _csvStructure = new CsvStructure(_slConfig, _randomFileName, _logger);

        // Assert
        AssertFilesAreValid();
        Assert.That(_files, Has.Length.EqualTo(1), $"{_files.Length} files were created but one was expected");
        Assert.That(_fileInfo, Has.Length.EqualTo(0), "File is not empty");
    }

    [Test]
    public async Task ShouldWriteOnlyHeader([Values(true, false)] bool compareOrder)
    {
        if (compareOrder)
            await WriteMeasurementsTwiceToCsvCompare(numOfMeasurements: 0);
        else
            await WriteMeasurementsToCsvCheckContent(numOfMeasurements: 0);
    }

    [Test]
    public async Task ShouldWriteOnlyOneMeasurementToLastCsv([Values(true, false)] bool compareOrder, [Values(true, false)] bool overwriteHeader, [Random(5000, 15000, 1)] int maxNumOfRowsPerFile, [Values(1, 2, 3)] int numOfFiles)
    {
        var numOfMeasurements = 1 + (maxNumOfRowsPerFile - 1) * (numOfFiles - 1);
        if (compareOrder)
            await WriteMeasurementsTwiceToCsvCompare(overwriteHeader, numOfMeasurements, numOfFiles);
        else
            await WriteMeasurementsToCsvCheckContent(overwriteHeader, numOfMeasurements, numOfFiles, maxNumOfRowsPerFile);
    }

    [Test]
    public async Task ShouldWriteMaxMinusOneMeasurementsToLastCsv([Values(true, false)] bool compareOrder, [Values(true, false)] bool overwriteHeader, [Random(5000, 15000, 1)] int maxNumOfRowsPerFile, [Values(1, 2, 3)] int numOfFiles)
    {
        var numOfMeasurements = (maxNumOfRowsPerFile - 1) * numOfFiles - 1;
        if (compareOrder)
            await WriteMeasurementsTwiceToCsvCompare(overwriteHeader, numOfMeasurements, numOfFiles);
        else
            await WriteMeasurementsToCsvCheckContent(overwriteHeader, numOfMeasurements, numOfFiles, maxNumOfRowsPerFile);
    }

    [Test]
    public async Task ShouldWriteMaxMeasurementsToLastCsv([Values(true, false)] bool compareOrder, [Values(true, false)] bool overwriteHeader, [Random(5000, 15000, 1)] int maxNumOfRowsPerFile, [Values(1, 2, 3)] int numOfFiles)
    {
        var numOfMeasurements = (maxNumOfRowsPerFile - 1) * numOfFiles;
        if (compareOrder)
            await WriteMeasurementsTwiceToCsvCompare(overwriteHeader, numOfMeasurements, numOfFiles);
        else
            await WriteMeasurementsToCsvCheckContent(overwriteHeader, numOfMeasurements, numOfFiles, maxNumOfRowsPerFile);
    }

    [Test]
    public async Task ShouldWriteMeasurementsToCsv([Values(true, false)] bool compareOrder, [Values(true, false)] bool overwriteHeader, [Random(5000, 15000, 1)] int numOfMeasurements, [Values(1, 2, 3)] int numOfFiles)
    {
        if (compareOrder)
            await WriteMeasurementsTwiceToCsvCompare(overwriteHeader, numOfMeasurements, numOfFiles);
        else
            await WriteMeasurementsToCsvCheckContent(overwriteHeader, numOfMeasurements, numOfFiles);

    }

    public async Task WriteMeasurementsTwiceToCsvCompare(bool addOrRemoveRandom = false, int numOfMeasurements = 0, int numOfFiles = 1, int maxNumOfRowsPerFile = -1)
    {
        // Arrange
        Measurement firstMeasurement = RandomMeasurement(_random.Next(0, 8), _random.Next(0, 8));
        List<Measurement> randomMeasurements = new List<Measurement>() { firstMeasurement };
        if (maxNumOfRowsPerFile < 0) // unless maxNumOfRowsPerFile was specified, every file (except possibly the last one) has the same number of rows
            maxNumOfRowsPerFile = (int)Math.Ceiling(1 + (double)numOfMeasurements / numOfFiles);
        _slConfig.MaxNumOfRows = maxNumOfRowsPerFile;

        // Act
        _csvStructure = new CsvStructure(_slConfig, _randomFileName, _logger);
        await _csvStructure.AddMeasurement(firstMeasurement);
        for (int i = 1; i < numOfMeasurements; i++)
        {
            Measurement newRndMeasurement = RandomMeasurement(basedOn: firstMeasurement);
            await _csvStructure.AddMeasurement(newRndMeasurement);
            if (_random.NextDouble() < 0.1) // write rows at random and not after every measurement
                await _csvStructure.WriteRows();
            randomMeasurements.Add(newRndMeasurement);
        }
        if (numOfMeasurements > 0)
            await _csvStructure.WriteRows();

        // Assert
        AssertFilesAreValid();
        Assert.That(_files, Has.Length.EqualTo(numOfFiles), $"{_files.Length} files were created but {numOfFiles} was expected");

        // Act
        var firstFiles = _files.ToArray();
        _randomFileName = NewGuidString();
        _csvStructure = new CsvStructure(_slConfig, _randomFileName, _logger);
        await _csvStructure.AddMeasurement(firstMeasurement);
        for (int i = 1; i < randomMeasurements.Count; i++)
        {
            if (addOrRemoveRandom && _random.NextDouble() < 5.0 / numOfMeasurements)
            {
                // add a random field or tag which should trigger the overwriting of the header
                AddOrRemoveRandomFieldOrTag(randomMeasurements[i]);
            }
            await _csvStructure.AddMeasurement(randomMeasurements[i]);
        }
        if (numOfMeasurements > 0)
            await _csvStructure.WriteRows();

        // Assert
        AssertFilesAreValid();
        Assert.That(_files, Has.Length.EqualTo(numOfFiles), $"{_files.Length} files were created but {numOfFiles} was expected");
        AssertCsvIsValidResult(firstFiles, _files, randomMeasurements);

        // add files first created to _file so they can be deleted in tear down
        List<string> tempFiles = _files.ToList();
        tempFiles.AddRange(firstFiles);
        _files = tempFiles.ToArray();
    }

    private async Task WriteMeasurementsToCsvCheckContent(bool triggerOverwritingHeader = false, int numOfMeasurements = 0, int numOfFiles = 1, int maxNumOfRowsPerFile = -1)
    {
        // Arrange
        Measurement firstMeasurement = RandomMeasurement(_random.Next(0, 8), _random.Next(0, 8));
        List<Measurement> randomMeasurements = new List<Measurement>() { firstMeasurement };
        var headingsOfFirstMeasurement = GetNames(firstMeasurement);

        if (maxNumOfRowsPerFile < 0) // unless maxNumOfRowsPerFile was specified, every file (except possibly the last one) has the same number of rows
            maxNumOfRowsPerFile = (int)Math.Ceiling(1 + (double)numOfMeasurements / numOfFiles);
        _slConfig.MaxNumOfRows = maxNumOfRowsPerFile;
        var expectedNumOfRows = maxNumOfRowsPerFile;

        List<Tuple<int, string>> newHeadings = new List<Tuple<int, string>>();
        var numOfValidatedMeasurements = 0;

        // Act
        _csvStructure = new CsvStructure(_slConfig, _randomFileName, _logger);
        await _csvStructure.AddMeasurement(firstMeasurement);
        // add random measurements
        for (int i = 1; i < numOfMeasurements; i++)
        {
            Measurement newRndMeasurement = RandomMeasurement(basedOn: firstMeasurement);
            if (triggerOverwritingHeader && _random.NextDouble() < 1.0 / numOfMeasurements)
            {
                // add a random field or tag which should trigger the overwriting of the header
                var newHeading = NewGuidString();
                AddRandomFieldOrTag(newRndMeasurement, 0.5, newHeading);
                newHeadings.Add(Tuple.Create(i, newHeading));
            }
            randomMeasurements.Add(newRndMeasurement);
            await _csvStructure.AddMeasurement(newRndMeasurement);
            if (_random.NextDouble() < 0.1) // write rows at random and not after every measurement
                await _csvStructure.WriteRows();
        }
        if (numOfMeasurements > 0)
            await _csvStructure.WriteRows();

        // Assert
        AssertFilesAreValid();
        Assert.That(_files, Has.Length.EqualTo(numOfFiles), $"{_files.Length} files were created but {numOfFiles} was expected");

        for (var fileIndex = 0; fileIndex < _files.Length; fileIndex++)
        {
            if (fileIndex == _files.Length - 1) // last file is possibly not maxed out
                expectedNumOfRows = 1 + numOfMeasurements - numOfValidatedMeasurements;

            var actualWrittenCsv = ReadInCsv(_files[fileIndex]);
            var actualWrittenHeader = actualWrittenCsv[0];

            Assert.That(actualWrittenCsv, Has.Count.EqualTo(expectedNumOfRows), $"{actualWrittenCsv.Count} lines in csv file but {expectedNumOfRows} was expected");

            // written header should contain every heading of the first measurement and every new heading added
            var hasEveryHeader = headingsOfFirstMeasurement.All(actualWrittenHeader.Contains);
            var expectedNewHeadings = newHeadings.Where(h => h.Item1 < expectedNumOfRows + fileIndex * maxNumOfRowsPerFile && h.Item1 >= fileIndex * maxNumOfRowsPerFile).Select(s => s.Item2);
            var hasEveryNewHeader = expectedNewHeadings.All(actualWrittenHeader.Contains);

            Assert.That(hasEveryHeader && hasEveryNewHeader, "Header does not contain every field or tag");

            bool everyFieldAndTag = true;
            for (int i = 1; i < actualWrittenCsv.Count; i++)
            {
                // check if the measurement in current line contains the same fields and tags
                var toBeTestedMeasurement = randomMeasurements[numOfValidatedMeasurements + i - 1];
                var expectedValues = GetValues(toBeTestedMeasurement);
                everyFieldAndTag = expectedValues.All(h => actualWrittenCsv[i].Contains(h));
                if (!everyFieldAndTag)
                    break;
            }

            Assert.That(everyFieldAndTag, "File does not contain every field or tag of at least one measurement");

            numOfValidatedMeasurements += expectedNumOfRows - 1;
        }
    }

    private void AddOrRemoveRandomFieldOrTag(Measurement measurement)
    {
        if (_random.NextDouble() < 0.5)
        {
            Measurement editedMeasurement = new Measurement(_randomFileName, measurement.TimeStamp);
            var editedFields = measurement.Fields;
            var editedTags = measurement.Tags;
            if (_random.NextDouble() < 0.5 && editedFields.Count > 0)
            { // remove random field
                editedFields.RemoveAt(_random.Next(0, editedFields.Count));
                editedFields.ForEach(f => editedMeasurement.Add(new DataField(f.Name, f.Value)));
                measurement.Tags.ForEach(t => editedMeasurement.Add(t));
            }
            else if (editedTags.Count > 0)
            { // remove random tag
                editedTags.RemoveAt(_random.Next(0, editedTags.Count));
                editedTags.ForEach(t => editedMeasurement.Add(new DataTag(t.Name, t.Value)));
                measurement.Fields.ForEach(f => editedMeasurement.Add(f));
            }
            else
            {
                AddRandomFieldOrTag(measurement, 0.5);
                return;
            }
            measurement = editedMeasurement;
        }
        else
            AddRandomFieldOrTag(measurement, 0.5);
    }

    private void AddRandomFieldOrTag(Measurement measurement, double fieldProbability, string name = null, string value = null)
    {
        if (name == null)
            name = NewGuidString();
        if (value == null)
            value = NewGuidString();

        if (_random.NextDouble() < fieldProbability)
            measurement.Add(new DataField(name, value));
        else
            measurement.Add(new DataTag(name, value));
    }

    public void AssertCsvIsValidResult(string[] originalFiles, string[] resultFiles, IList<Measurement> measurements)
    {
        int numOfValidatedMeasurements = 0;
        for (int fileIndex = 0; fileIndex < resultFiles.Length; fileIndex++)
        {
            var firstWrittenCsv = ReadInCsv(originalFiles[fileIndex]);
            var secondWrittenCsv = ReadInCsv(resultFiles[fileIndex]);

            var firstHeader = firstWrittenCsv[0];
            var secondHeader = secondWrittenCsv[0];
            for (int i = 0; i < firstHeader.Count; i++)
            { // header of first file should be a sub list of the second header with the same order 
                Assert.That(firstHeader[0], Is.EqualTo(secondHeader[0]), "Headings are not the same");
            }

            for (int row = 1; row < secondWrittenCsv.Count; row++)
            {
                for (int col = 0; col < secondWrittenCsv[row].Count; col++)
                {
                    var valueInSecondCsv = secondWrittenCsv[row][col];
                    var valueInMeasurement = GetValueByName(measurements[numOfValidatedMeasurements], secondHeader[col]);

                    if (valueInMeasurement == null)
                    { // field or tag could have been removed or was added like the "time" column
                        if (col < firstWrittenCsv[row].Count)
                        {
                            var valueInFirstCsv = firstWrittenCsv[row][col];
                            if (!string.IsNullOrEmpty(valueInSecondCsv)) // values in both CSV files should be the same in this case
                                Assert.That(valueInFirstCsv, Is.EqualTo(valueInSecondCsv), $"Values in row {row + 1} column {col + 1} are not equal in both files");
                            else // value was removed before writing to second CSV, should not be empty in first CSV
                                Assert.That(string.IsNullOrEmpty(valueInFirstCsv), Is.False, $"Value in row {row + 1} column {col + 1} is null or empty in file {originalFiles[fileIndex]}");
                        }
                        else // value should be null because the header was overwritten but no corresponding value was inserted
                            Assert.That(string.IsNullOrEmpty(valueInSecondCsv), $"Value in row {row + 1} column {col + 1} is not null or empty");
                    }
                    else
                    {
                        Assert.That(valueInMeasurement, Is.EqualTo(valueInSecondCsv), $"Value in row {row + 1} column {col + 1} is not equal to value of measurement");
                        if (col < firstWrittenCsv[row].Count)
                        { // value should not be modified and should be in both CSV files
                            var valueInFirstCsv = firstWrittenCsv[row][col];
                            Assert.That(valueInMeasurement, Is.EqualTo(valueInFirstCsv), $"Value in row {row + 1} column {col + 1} is not equal to value of measurement");
                        }
                    }
                }
                numOfValidatedMeasurements++;
            }
        }
    }

    private void AssertFilesAreValid()
    {
        Assert.That(Directory.Exists(_path));
        _files = TestDirectory.GetOrderedFiles(_path, _randomFileName + "*");

        foreach (var file in _files)
        {
            _fileInfo = new FileInfo(file);

            var fileTimeStampStr = new String(_fileInfo.Name
                .Replace(_randomFileName, "")
                .Where(Char.IsDigit)
                .ToArray());
            var fileTimeStamp = DateTime.ParseExact(fileTimeStampStr, "yyyyMMddHHmmssfff", CultureInfo.InvariantCulture);

            var timeDiff = (fileTimeStamp - _testTimeStamp).TotalMilliseconds;
            Assert.That(timeDiff + 100, Is.Positive, "Time stamp in name of file is too far in the past");
            Assert.That(timeDiff, Is.LessThan(100000), "File was not created within a necessary time span");

            if (file.Equals(_files[_files.Length - 1]))
            {
                Assert.That(IsFileLocked(_fileInfo), "Stream should stay open but file was accessible");
                _csvStructure.Close();
                Assert.That(IsFileLocked(_fileInfo), Is.False, "Stream should be closed but file was not accessible");
            }
            else
                Assert.That(IsFileLocked(_fileInfo), Is.False, "Stream should be closed after new file was created but file was not accessible");
        }
    }

    private IEnumerable<string> GetNames(Measurement measurement)
    {
        return measurement.Fields.Select(f => f.Name).Concat(measurement.Tags.Select(t => t.Name));
    }

    private IEnumerable<object> GetValues(Measurement measurement)
    {
        return measurement.Fields.Select(f => f.Value).Concat(measurement.Tags.Select(t => t.Value));
    }

    private string GetValueByName(Measurement measurement, string name)
    {
        return (string)measurement.Fields.Where(f => f.Name.Equals(name)).Select(f => f.Value)
            .Concat(
                measurement.Tags.Where(t => t.Name.Equals(name)).Select(t => t.Value)
            ).SingleOrDefault();
    }

    private bool IsFileLocked(FileInfo file)
    {
        try
        {
            using (FileStream stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None))
                stream.Close();
        }
        catch (IOException) { return true; }
        return false;
    }

    private static string NewGuidString()
    {
        return Guid.NewGuid().ToString();
    }

    private Measurement RandomMeasurement(int numOfFields = 0, int numOfTags = 0, Measurement basedOn = null)
    {
        Measurement measurement = new Measurement(_randomFileName);
        if (basedOn != null)
        {
            basedOn.Fields.ForEach(f => measurement.Add(new DataField(f.Name, NewGuidString())));
            basedOn.Tags.ForEach(t => measurement.Add(new DataTag(t.Name, NewGuidString())));
        }
        else
        {
            for (int i = 0; i < numOfFields; i++)
            {
                AddRandomFieldOrTag(measurement, 1.0);
            }

            for (int i = 0; i < numOfTags; i++)
            {
                AddRandomFieldOrTag(measurement, 0.0);
            }
        }
        return measurement;
    }

    private List<List<string>> ReadInCsv(string absolutePath)
    {
        List<List<string>> result = new List<List<string>>();
        string value;
        using (TextReader fileReader = File.OpenText(absolutePath))
        {
            var csv = new CsvReader(fileReader, _csvConfig);
            while (csv.Read())
            {
                List<string> line = new List<string>();
                for (int i = 0; csv.TryGetField<string>(i, out value); i++)
                {
                    line.Add(value);
                }
                result.Add(line);
            }
        }
        return result;
    }
}