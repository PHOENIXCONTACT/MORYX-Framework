using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using NUnit.Framework;

namespace Marvin.Communication.Sockets.IntegrationTests.DelimiterProtocol
{
    [TestFixture]
    public class DelimiterInterpreterTests
    {
        private IMessageInterpreter _interpreter;
        private DelimitedMessageContext _context;

        [SetUp]
        public void CaseSetup()
        {
            _interpreter = new TestDelimiterInterpreter();
            _context = (DelimitedMessageContext)_interpreter.CreateContext();
        }

        [Test]
        public void CreateContext()
        {
            var context = _context;

            // Base properties
            Assert.AreEqual(0, context.CurrentIndex);
            Assert.AreEqual(EndDelimiterOnlyInterpreter.TestReadSize, context.ReadSize);
            Assert.AreEqual(EndDelimiterOnlyInterpreter.TestBufferSize, context.ReadBuffer.Length);

            // Extended properties 
            context.StartFound = false;
        }

        [TestCase(true, Description = "Use start and end delimiter")]
        [TestCase(false, Description = "Use only end delimiter")]
        public void PartialMessage(bool useStartDelimiter)
        {
            if (!useStartDelimiter)
                _interpreter = new EndDelimiterOnlyInterpreter();

            var text = Encoding.Unicode.GetBytes("Wie passend, du kämpfst");

            // Arrange
            var message = new List<byte>();
            message.AddRange(TestDelimiterInterpreter.TestStartDelimiter);
            message.AddRange(text);
            var readMessage = message.ToArray();

            // Act
            Array.Copy(readMessage, 0, _context.ReadBuffer, 0, readMessage.Length);
            _interpreter.ProcessReadBytes(_context, readMessage.Length, m => { });

            // Assert
            Assert.IsTrue(_context.StartFound);
            Assert.AreEqual(readMessage.Length, _context.CurrentIndex);
        }

        [TestCase(0, Description = "No leading chunk, only the full message.")]
        [TestCase(4, Description = "4 bytes of leading chunk")]
        public void FullMessage(int leadingChunk)
        {
            var text = Encoding.Unicode.GetBytes("Wie passend, du kämpfst wie eine Kuh!");

            // Arrange
            var message = new List<byte>();
            for (int i = 1; i <= leadingChunk; i++)
            {
                message.Add((byte)i);
            }
            message.AddRange(TestDelimiterInterpreter.TestStartDelimiter);
            message.AddRange(text);
            message.AddRange(EndDelimiterOnlyInterpreter.TestEndDelimiter);
            var readMessage = message.ToArray();


            // Act
            BinaryMessage published = null;
            Array.Copy(readMessage, 0, _context.ReadBuffer, 0, readMessage.Length);
            _interpreter.ProcessReadBytes(_context, readMessage.Length, m => published = m);


            // Assert
            Assert.IsFalse(_context.StartFound);
            Assert.AreEqual(0, _context.CurrentIndex);

            Assert.NotNull(published);
            Assert.AreEqual(readMessage.Length - leadingChunk, published.Payload.Length);
            Assert.AreEqual(readMessage.Skip(leadingChunk).Sum(e => (short)e), published.Payload.Sum(e => (short)e));
        }

        [Test]
        public void MessageOverlap()
        {
            var text = Encoding.Unicode.GetBytes("Wie passend, du kämpfst wie eine Kuh!");
            var partialText = Encoding.Unicode.GetBytes("Wie passend");

            // Arrange
            var message = new List<byte>();
            message.AddRange(TestDelimiterInterpreter.TestStartDelimiter);
            message.AddRange(text);
            message.AddRange(EndDelimiterOnlyInterpreter.TestEndDelimiter);
            var fullmessage = message.ToArray();
            message.AddRange(TestDelimiterInterpreter.TestStartDelimiter);
            message.AddRange(partialText);
            var readMessage = message.ToArray();


            // Act
            BinaryMessage published = null;
            BinaryMessage notPublished = null;
            const int cut = EndDelimiterOnlyInterpreter.TestReadSize;

            Array.Copy(readMessage, 0, _context.ReadBuffer, 0, cut);
            _interpreter.ProcessReadBytes(_context, cut, m => published = m);

            var remain = readMessage.Length - cut;
            Array.Copy(readMessage, cut, _context.ReadBuffer, _context.CurrentIndex, remain);
            _interpreter.ProcessReadBytes(_context, remain, m => notPublished = m);


            // Assert
            Assert.IsTrue(_context.StartFound);
            Assert.AreEqual(readMessage.Length - fullmessage.Length, _context.CurrentIndex);
            // Check published message
            Assert.NotNull(published);
            Assert.AreEqual(fullmessage.Length, published.Payload.Length);
            Assert.AreEqual(fullmessage.Sum(e => (short)e), published.Payload.Sum(e => (short)e));
            // Check second was not published
            Assert.IsNull(notPublished);
        }

        [TestCase(1, Description = "Reading byte wise")]
        [TestCase(3, Description = "Chunks smaller than start or end")]
        [TestCase(41, Description = "Chunks of half a message")]
        [TestCase(82, Description = "Chunk matches first message")]
        [TestCase(EndDelimiterOnlyInterpreter.TestReadSize, Description = "Chunks of full read size")]
        public void MessageChunks(int chunkSize)
        {
            var fullMessages = new byte[3][];

            // Arrange
            var stream = new List<byte>();
            stream.AddRange(TestDelimiterInterpreter.TestStartDelimiter);
            stream.AddRange(Encoding.Unicode.GetBytes("Wie passend, du kämpfst wie eine Kuh!"));
            stream.AddRange(EndDelimiterOnlyInterpreter.TestEndDelimiter);
            fullMessages[0] = stream.ToArray();

            stream.AddRange(TestDelimiterInterpreter.TestStartDelimiter);
            stream.AddRange(Encoding.Unicode.GetBytes("Geh mich ausm Weg oder ich schneid dir durch!"));
            stream.AddRange(EndDelimiterOnlyInterpreter.TestEndDelimiter);
            fullMessages[1] = stream.Skip(fullMessages[0].Length).ToArray();

            stream.AddRange(TestDelimiterInterpreter.TestStartDelimiter);
            stream.AddRange(Encoding.Unicode.GetBytes("Mein Name ist Guybrush Threepwood und ich will Pirat werden!"));
            stream.AddRange(EndDelimiterOnlyInterpreter.TestEndDelimiter);
            fullMessages[2] = stream.Skip(fullMessages[0].Length).Skip(fullMessages[1].Length).ToArray();

            stream.AddRange(TestDelimiterInterpreter.TestStartDelimiter);
            var readMessage = stream.ToArray();


            // Act
            List<BinaryMessage> published = new List<BinaryMessage>();
            var index = 0;
            while (index < readMessage.Length)
            {
                var diff = readMessage.Length - index;
                var copyRange = (diff > chunkSize) ? chunkSize : diff;
                Array.Copy(readMessage, index, _context.ReadBuffer, _context.CurrentIndex, copyRange);
                _interpreter.ProcessReadBytes(_context, copyRange, published.Add);

                index += chunkSize;
            }


            // Assert
            Assert.IsTrue(_context.StartFound);
            Assert.AreEqual(4, _context.CurrentIndex);

            Assert.AreEqual(3, published.Count);
            for (var i = 0; i < 3; i++)
            {
                Assert.AreEqual(fullMessages[i].Length, published[i].Payload.Length);
                Assert.AreEqual(fullMessages[i].Sum(e => (short)e), published[i].Payload.Sum(e => (short)e));
            }
        }

        [Test]
        public void MetronicTest()
        {
            // Arrange
            _interpreter = MetronicInterpreter.Instance;
            _context = (DelimitedMessageContext)_interpreter.CreateContext();

            var text = Encoding.UTF8.GetBytes(Input);

            // Act
            BinaryMessage published = null;
            Array.Copy(text, 0, _context.ReadBuffer, _context.CurrentIndex, text.Length);
            _interpreter.ProcessReadBytes(_context, text.Length, m => published = m);

            // Assert
            Assert.NotNull(published);
            Assert.AreEqual(published.Payload, Encoding.UTF8.GetBytes(Output));
        }

        private const string Input =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<GP>
  <CLEARLAB />
  <DFCLEAR />
  <SAVELAB aName=""label\Metronic.txt"">
    <LAB>
      <OBJ>
        <TYPE>text</TYPE>
        <X>10</X>
        <Y>3</Y>
        <SW>1</SW>
        <SS>0</SS>
        <MAG>1</MAG>
        <NEG>0</NEG>
        <BWD>0</BWD>
        <ANGLE>0</ANGLE>
        <TEXT>Q</TEXT>
        <FONT aFace=""a9x5"" aSize=""9"" aBld=""0"" aIt=""0"" aUl=""0"" aGap=""0""></FONT>
      </OBJ>
      <OBJ>
        <TYPE>text</TYPE>
        <X>34</X>
        <Y>4</Y>
        <SW>1</SW>
        <SS>0</SS>
        <MAG>1</MAG>
        <NEG>0</NEG>
        <BWD>0</BWD>
        <ANGLE>2</ANGLE>
        <TEXT>Q</TEXT>
        <FONT aFace=""a9x5"" aSize=""9"" aBld=""0"" aIt=""0"" aUl=""0"" aGap=""0""></FONT>
      </OBJ>
      <OBJ>
        <TYPE>text</TYPE>
        <X>59</X>
        <Y>5</Y>
        <SW>1</SW>
        <SS>0</SS>
        <MAG>1</MAG>
        <NEG>0</NEG>
        <BWD>0</BWD>
        <ANGLE>1</ANGLE>
        <TEXT>Q</TEXT>
        <FONT aFace=""a9x5"" aSize=""9"" aBld=""0"" aIt=""0"" aUl=""0"" aGap=""0""></FONT>
      </OBJ>
      <OBJ>
        <TYPE>text</TYPE>
        <X>86</X>
        <Y>3</Y>
        <SW>1</SW>
        <SS>0</SS>
        <MAG>1</MAG>
        <NEG>0</NEG>
        <BWD>0</BWD>
        <ANGLE>3</ANGLE>
        <TEXT>Q</TEXT>
        <FONT aFace=""a9x5"" aSize=""9"" aBld=""0"" aIt=""0"" aUl=""0"" aGap=""0""></FONT>
      </OBJ>
      <OBJ>
        <TYPE>text</TYPE>
        <X>112</X>
        <Y>0</Y>
        <SW>1</SW>
        <SS>0</SS>
        <MAG>1</MAG>
        <NEG>0</NEG>
        <BWD>0</BWD>
        <ANGLE>0</ANGLE>
        <TEXT>Q</TEXT>
        <FONT aFace=""a9x5"" aSize=""9"" aBld=""0"" aIt=""0"" aUl=""0"" aGap=""0""></FONT>
      </OBJ>
      <OBJ>
        <TYPE>text</TYPE>
        <X>136</X>
        <Y>-1</Y>
        <SW>1</SW>
        <SS>0</SS>
        <MAG>1</MAG>
        <NEG>0</NEG>
        <BWD>0</BWD>
        <ANGLE>2</ANGLE>
        <TEXT>Q</TEXT>
        <FONT aFace=""a9x5"" aSize=""9"" aBld=""0"" aIt=""0"" aUl=""0"" aGap=""0""></FONT>
      </OBJ>
      <OBJ>
        <TYPE>text</TYPE>
        <X>162</X>
        <Y>0</Y>
        <SW>1</SW>
        <SS>0</SS>
        <MAG>1</MAG>
        <NEG>0</NEG>
        <BWD>0</BWD>
        <ANGLE>1</ANGLE>
        <TEXT>Q</TEXT>
        <FONT aFace=""a9x5"" aSize=""9"" aBld=""0"" aIt=""0"" aUl=""0"" aGap=""0""></FONT>
      </OBJ>
      <OBJ>
        <TYPE>text</TYPE>
        <X>190</X>
        <Y>0</Y>
        <SW>1</SW>
        <SS>0</SS>
        <MAG>1</MAG>
        <NEG>0</NEG>
        <BWD>0</BWD>
        <ANGLE>3</ANGLE>
        <TEXT>Q</TEXT>
        <FONT aFace=""a9x5"" aSize=""9"" aBld=""0"" aIt=""0"" aUl=""0"" aGap=""0""></FONT>
      </OBJ>
      <OBJ>
        <TYPE>text</TYPE>
        <X>216</X>
        <Y>7</Y>
        <SW>1</SW>
        <SS>0</SS>
        <MAG>1</MAG>
        <NEG>0</NEG>
        <BWD>0</BWD>
        <ANGLE>0</ANGLE>
        <TEXT>Q</TEXT>
        <FONT aFace=""a9x5"" aSize=""9"" aBld=""0"" aIt=""0"" aUl=""0"" aGap=""0""></FONT>
      </OBJ>
      <OBJ>
        <TYPE>text</TYPE>
        <X>240</X>
        <Y>10</Y>
        <SW>1</SW>
        <SS>0</SS>
        <MAG>1</MAG>
        <NEG>0</NEG>
        <BWD>0</BWD>
        <ANGLE>2</ANGLE>
        <TEXT>Q</TEXT>
        <FONT aFace=""a9x5"" aSize=""9"" aBld=""0"" aIt=""0"" aUl=""0"" aGap=""0""></FONT>
      </OBJ>
      <OBJ>
        <TYPE>text</TYPE>
        <X>266</X>
        <Y>11</Y>
        <SW>1</SW>
        <SS>0</SS>
        <MAG>1</MAG>
        <NEG>0</NEG>
        <BWD>0</BWD>
        <ANGLE>1</ANGLE>
        <TEXT>Q</TEXT>
        <FONT aFace=""a9x5"" aSize=""9"" aBld=""0"" aIt=""0"" aUl=""0"" aGap=""0""></FONT>
      </OBJ>
      <OBJ>
        <TYPE>text</TYPE>
        <X>294</X>
        <Y>7</Y>
        <SW>1</SW>
        <SS>0</SS>
        <MAG>1</MAG>
        <NEG>0</NEG>
        <BWD>0</BWD>
        <ANGLE>3</ANGLE>
        <TEXT>Q</TEXT>
        <FONT aFace=""a9x5"" aSize=""9"" aBld=""0"" aIt=""0"" aUl=""0"" aGap=""0""></FONT>
      </OBJ>
      <OBJ>
        <TYPE>text</TYPE>
        <X>309</X>
        <Y>3</Y>
        <SW>1</SW>
        <SS>0</SS>
        <MAG>1</MAG>
        <NEG>0</NEG>
        <BWD>0</BWD>
        <ANGLE>0</ANGLE>
        <TEXT>Q</TEXT>
        <FONT aFace=""a9x5"" aSize=""9"" aBld=""0"" aIt=""0"" aUl=""0"" aGap=""0""></FONT>
      </OBJ>
      <OBJ>
        <TYPE>text</TYPE>
        <X>335</X>
        <Y>4</Y>
        <SW>1</SW>
        <SS>0</SS>
        <MAG>1</MAG>
        <NEG>0</NEG>
        <BWD>0</BWD>
        <ANGLE>2</ANGLE>
        <TEXT>Q</TEXT>
        <FONT aFace=""a9x5"" aSize=""9"" aBld=""0"" aIt=""0"" aUl=""0"" aGap=""0""></FONT>
      </OBJ>
      <OBJ>
        <TYPE>text</TYPE>
        <X>361</X>
        <Y>5</Y>
        <SW>1</SW>
        <SS>0</SS>
        <MAG>1</MAG>
        <NEG>0</NEG>
        <BWD>0</BWD>
        <ANGLE>1</ANGLE>
        <TEXT>Q</TEXT>
        <FONT aFace=""a9x5"" aSize=""9"" aBld=""0"" aIt=""0"" aUl=""0"" aGap=""0""></FONT>
      </OBJ>
      <OBJ>
        <TYPE>text</TYPE>
        <X>387</X>
        <Y>3</Y>
        <SW>1</SW>
        <SS>0</SS>
        <MAG>1</MAG>
        <NEG>0</NEG>
        <BWD>0</BWD>
        <ANGLE>3</ANGLE>
        <TEXT>Q</TEXT>
        <FONT aFace=""a9x5"" aSize=""9"" aBld=""0"" aIt=""0"" aUl=""0"" aGap=""0""></FONT>
      </OBJ>
      <OBJ>
        <TYPE>text</TYPE>
        <X>434</X>
        <Y>3</Y>
        <SW>1</SW>
        <SS>0</SS>
        <MAG>1</MAG>
        <NEG>0</NEG>
        <BWD>0</BWD>
        <ANGLE>0</ANGLE>
        <TEXT>Q</TEXT>
        <FONT aFace=""a9x5"" aSize=""9"" aBld=""0"" aIt=""0"" aUl=""0"" aGap=""0""></FONT>
      </OBJ>
      <OBJ>
        <TYPE>text</TYPE>
        <X>456</X>
        <Y>4</Y>
        <SW>1</SW>
        <SS>0</SS>
        <MAG>1</MAG>
        <NEG>0</NEG>
        <BWD>0</BWD>
        <ANGLE>2</ANGLE>
        <TEXT>Q</TEXT>
        <FONT aFace=""a9x5"" aSize=""9"" aBld=""0"" aIt=""0"" aUl=""0"" aGap=""0""></FONT>
      </OBJ>
      <OBJ>
        <TYPE>text</TYPE>
        <X>482</X>
        <Y>5</Y>
        <SW>1</SW>
        <SS>0</SS>
        <MAG>1</MAG>
        <NEG>0</NEG>
        <BWD>0</BWD>
        <ANGLE>1</ANGLE>
        <TEXT>Q</TEXT>
        <FONT aFace=""a9x5"" aSize=""9"" aBld=""0"" aIt=""0"" aUl=""0"" aGap=""0""></FONT>
      </OBJ>
      <OBJ>
        <TYPE>text</TYPE>
        <X>512</X>
        <Y>3</Y>
        <SW>1</SW>
        <SS>0</SS>
        <MAG>1</MAG>
        <NEG>0</NEG>
        <BWD>0</BWD>
        <ANGLE>3</ANGLE>
        <TEXT>Q</TEXT>
        <FONT aFace=""a9x5"" aSize=""9"" aBld=""0"" aIt=""0"" aUl=""0"" aGap=""0""></FONT>
      </OBJ>
    </LAB>
  </SAVELAB>
  <LOADLAB>label\Metronic.txt</LOADLAB>
</GP>
<?xml version=""1.0"" encoding=""utf-8""?>
<GP>
  <CLEARLAB />
  <DFCLEAR />
  <SAVELAB aName=""label\Metronic.txt"">
    <LAB>
      <OBJ>
        <TYPE>text</TYPE>
        <X>10</X>
        <Y>3</Y>
        <SW>1</SW>
        <SS>0</SS>
        <MAG>1</MAG>
        <NEG>0</NEG>
        <BWD>0</BWD>
        <ANGLE>0</ANGLE>
        <TEXT>Q</TEXT>
        <FONT aFace=""a9x5"" aSize=""9"" aBld=""0"" aIt=""0"" aUl=""0"" aGap=""0""></FONT>
      </OBJ>";

        private const string Output = @"<GP>
  <CLEARLAB />
  <DFCLEAR />
  <SAVELAB aName=""label\Metronic.txt"">
    <LAB>
      <OBJ>
        <TYPE>text</TYPE>
        <X>10</X>
        <Y>3</Y>
        <SW>1</SW>
        <SS>0</SS>
        <MAG>1</MAG>
        <NEG>0</NEG>
        <BWD>0</BWD>
        <ANGLE>0</ANGLE>
        <TEXT>Q</TEXT>
        <FONT aFace=""a9x5"" aSize=""9"" aBld=""0"" aIt=""0"" aUl=""0"" aGap=""0""></FONT>
      </OBJ>
      <OBJ>
        <TYPE>text</TYPE>
        <X>34</X>
        <Y>4</Y>
        <SW>1</SW>
        <SS>0</SS>
        <MAG>1</MAG>
        <NEG>0</NEG>
        <BWD>0</BWD>
        <ANGLE>2</ANGLE>
        <TEXT>Q</TEXT>
        <FONT aFace=""a9x5"" aSize=""9"" aBld=""0"" aIt=""0"" aUl=""0"" aGap=""0""></FONT>
      </OBJ>
      <OBJ>
        <TYPE>text</TYPE>
        <X>59</X>
        <Y>5</Y>
        <SW>1</SW>
        <SS>0</SS>
        <MAG>1</MAG>
        <NEG>0</NEG>
        <BWD>0</BWD>
        <ANGLE>1</ANGLE>
        <TEXT>Q</TEXT>
        <FONT aFace=""a9x5"" aSize=""9"" aBld=""0"" aIt=""0"" aUl=""0"" aGap=""0""></FONT>
      </OBJ>
      <OBJ>
        <TYPE>text</TYPE>
        <X>86</X>
        <Y>3</Y>
        <SW>1</SW>
        <SS>0</SS>
        <MAG>1</MAG>
        <NEG>0</NEG>
        <BWD>0</BWD>
        <ANGLE>3</ANGLE>
        <TEXT>Q</TEXT>
        <FONT aFace=""a9x5"" aSize=""9"" aBld=""0"" aIt=""0"" aUl=""0"" aGap=""0""></FONT>
      </OBJ>
      <OBJ>
        <TYPE>text</TYPE>
        <X>112</X>
        <Y>0</Y>
        <SW>1</SW>
        <SS>0</SS>
        <MAG>1</MAG>
        <NEG>0</NEG>
        <BWD>0</BWD>
        <ANGLE>0</ANGLE>
        <TEXT>Q</TEXT>
        <FONT aFace=""a9x5"" aSize=""9"" aBld=""0"" aIt=""0"" aUl=""0"" aGap=""0""></FONT>
      </OBJ>
      <OBJ>
        <TYPE>text</TYPE>
        <X>136</X>
        <Y>-1</Y>
        <SW>1</SW>
        <SS>0</SS>
        <MAG>1</MAG>
        <NEG>0</NEG>
        <BWD>0</BWD>
        <ANGLE>2</ANGLE>
        <TEXT>Q</TEXT>
        <FONT aFace=""a9x5"" aSize=""9"" aBld=""0"" aIt=""0"" aUl=""0"" aGap=""0""></FONT>
      </OBJ>
      <OBJ>
        <TYPE>text</TYPE>
        <X>162</X>
        <Y>0</Y>
        <SW>1</SW>
        <SS>0</SS>
        <MAG>1</MAG>
        <NEG>0</NEG>
        <BWD>0</BWD>
        <ANGLE>1</ANGLE>
        <TEXT>Q</TEXT>
        <FONT aFace=""a9x5"" aSize=""9"" aBld=""0"" aIt=""0"" aUl=""0"" aGap=""0""></FONT>
      </OBJ>
      <OBJ>
        <TYPE>text</TYPE>
        <X>190</X>
        <Y>0</Y>
        <SW>1</SW>
        <SS>0</SS>
        <MAG>1</MAG>
        <NEG>0</NEG>
        <BWD>0</BWD>
        <ANGLE>3</ANGLE>
        <TEXT>Q</TEXT>
        <FONT aFace=""a9x5"" aSize=""9"" aBld=""0"" aIt=""0"" aUl=""0"" aGap=""0""></FONT>
      </OBJ>
      <OBJ>
        <TYPE>text</TYPE>
        <X>216</X>
        <Y>7</Y>
        <SW>1</SW>
        <SS>0</SS>
        <MAG>1</MAG>
        <NEG>0</NEG>
        <BWD>0</BWD>
        <ANGLE>0</ANGLE>
        <TEXT>Q</TEXT>
        <FONT aFace=""a9x5"" aSize=""9"" aBld=""0"" aIt=""0"" aUl=""0"" aGap=""0""></FONT>
      </OBJ>
      <OBJ>
        <TYPE>text</TYPE>
        <X>240</X>
        <Y>10</Y>
        <SW>1</SW>
        <SS>0</SS>
        <MAG>1</MAG>
        <NEG>0</NEG>
        <BWD>0</BWD>
        <ANGLE>2</ANGLE>
        <TEXT>Q</TEXT>
        <FONT aFace=""a9x5"" aSize=""9"" aBld=""0"" aIt=""0"" aUl=""0"" aGap=""0""></FONT>
      </OBJ>
      <OBJ>
        <TYPE>text</TYPE>
        <X>266</X>
        <Y>11</Y>
        <SW>1</SW>
        <SS>0</SS>
        <MAG>1</MAG>
        <NEG>0</NEG>
        <BWD>0</BWD>
        <ANGLE>1</ANGLE>
        <TEXT>Q</TEXT>
        <FONT aFace=""a9x5"" aSize=""9"" aBld=""0"" aIt=""0"" aUl=""0"" aGap=""0""></FONT>
      </OBJ>
      <OBJ>
        <TYPE>text</TYPE>
        <X>294</X>
        <Y>7</Y>
        <SW>1</SW>
        <SS>0</SS>
        <MAG>1</MAG>
        <NEG>0</NEG>
        <BWD>0</BWD>
        <ANGLE>3</ANGLE>
        <TEXT>Q</TEXT>
        <FONT aFace=""a9x5"" aSize=""9"" aBld=""0"" aIt=""0"" aUl=""0"" aGap=""0""></FONT>
      </OBJ>
      <OBJ>
        <TYPE>text</TYPE>
        <X>309</X>
        <Y>3</Y>
        <SW>1</SW>
        <SS>0</SS>
        <MAG>1</MAG>
        <NEG>0</NEG>
        <BWD>0</BWD>
        <ANGLE>0</ANGLE>
        <TEXT>Q</TEXT>
        <FONT aFace=""a9x5"" aSize=""9"" aBld=""0"" aIt=""0"" aUl=""0"" aGap=""0""></FONT>
      </OBJ>
      <OBJ>
        <TYPE>text</TYPE>
        <X>335</X>
        <Y>4</Y>
        <SW>1</SW>
        <SS>0</SS>
        <MAG>1</MAG>
        <NEG>0</NEG>
        <BWD>0</BWD>
        <ANGLE>2</ANGLE>
        <TEXT>Q</TEXT>
        <FONT aFace=""a9x5"" aSize=""9"" aBld=""0"" aIt=""0"" aUl=""0"" aGap=""0""></FONT>
      </OBJ>
      <OBJ>
        <TYPE>text</TYPE>
        <X>361</X>
        <Y>5</Y>
        <SW>1</SW>
        <SS>0</SS>
        <MAG>1</MAG>
        <NEG>0</NEG>
        <BWD>0</BWD>
        <ANGLE>1</ANGLE>
        <TEXT>Q</TEXT>
        <FONT aFace=""a9x5"" aSize=""9"" aBld=""0"" aIt=""0"" aUl=""0"" aGap=""0""></FONT>
      </OBJ>
      <OBJ>
        <TYPE>text</TYPE>
        <X>387</X>
        <Y>3</Y>
        <SW>1</SW>
        <SS>0</SS>
        <MAG>1</MAG>
        <NEG>0</NEG>
        <BWD>0</BWD>
        <ANGLE>3</ANGLE>
        <TEXT>Q</TEXT>
        <FONT aFace=""a9x5"" aSize=""9"" aBld=""0"" aIt=""0"" aUl=""0"" aGap=""0""></FONT>
      </OBJ>
      <OBJ>
        <TYPE>text</TYPE>
        <X>434</X>
        <Y>3</Y>
        <SW>1</SW>
        <SS>0</SS>
        <MAG>1</MAG>
        <NEG>0</NEG>
        <BWD>0</BWD>
        <ANGLE>0</ANGLE>
        <TEXT>Q</TEXT>
        <FONT aFace=""a9x5"" aSize=""9"" aBld=""0"" aIt=""0"" aUl=""0"" aGap=""0""></FONT>
      </OBJ>
      <OBJ>
        <TYPE>text</TYPE>
        <X>456</X>
        <Y>4</Y>
        <SW>1</SW>
        <SS>0</SS>
        <MAG>1</MAG>
        <NEG>0</NEG>
        <BWD>0</BWD>
        <ANGLE>2</ANGLE>
        <TEXT>Q</TEXT>
        <FONT aFace=""a9x5"" aSize=""9"" aBld=""0"" aIt=""0"" aUl=""0"" aGap=""0""></FONT>
      </OBJ>
      <OBJ>
        <TYPE>text</TYPE>
        <X>482</X>
        <Y>5</Y>
        <SW>1</SW>
        <SS>0</SS>
        <MAG>1</MAG>
        <NEG>0</NEG>
        <BWD>0</BWD>
        <ANGLE>1</ANGLE>
        <TEXT>Q</TEXT>
        <FONT aFace=""a9x5"" aSize=""9"" aBld=""0"" aIt=""0"" aUl=""0"" aGap=""0""></FONT>
      </OBJ>
      <OBJ>
        <TYPE>text</TYPE>
        <X>512</X>
        <Y>3</Y>
        <SW>1</SW>
        <SS>0</SS>
        <MAG>1</MAG>
        <NEG>0</NEG>
        <BWD>0</BWD>
        <ANGLE>3</ANGLE>
        <TEXT>Q</TEXT>
        <FONT aFace=""a9x5"" aSize=""9"" aBld=""0"" aIt=""0"" aUl=""0"" aGap=""0""></FONT>
      </OBJ>
    </LAB>
  </SAVELAB>
  <LOADLAB>label\Metronic.txt</LOADLAB>
</GP>";
    }
}