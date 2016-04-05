using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//I didn't set any method as deprecated as all of them would be, and I didn't want to copy the code or fix it not to obscure my solution. Please, let me know if you want me to.
namespace Symphony
{
    public interface ICSVReaderWriterNew
    {
        /// <summary>
        /// Reads a line from a file. Splits with a delimeter set in constructor. Defaults to '\t'
        /// </summary>
        /// <param name="expectedColumnsNumber">Optional parameter. Number of columns expected in a line being read. </param>
        /// <returns>Array of strings from columns in a line</returns>
        /// <exception cref="IncorrectColumnsNumberException">Thrown if a number of columns is different and not zero</exception>
        string[] Read(int expectedColumnsNumber = 0);

        /// <summary>
        /// Writes to a file a line with delimited values from columns array
        /// </summary>
        /// <param name="columns">Columns to be written</param>
        /// <exception cref="WriterNotConfiguredException">Thrown when writer is not being set up</exception>
        void Write(params string[] columns);

        /// <summary>
        /// Writes to a specified StreamWriter a line with delimited values from columns array
        /// </summary>
        /// <param name="writer">WriterStream where values will be written to</param>
        /// <param name="columns">Columns to be written</param>
        void Write(StreamWriter writer, params string[] columns);
    }

    public class CSVReaderWriterNew : ICSVReaderWriterNew, IDisposable
    {
        private StreamReader _readerStream = null;
        private StreamWriter _writerStream = null;
        private const char _defaultDelimeter = '\t';
        private char _delimeter;

        /// <summary>
        /// Creates new instance of CSVReaderWriterNew. All arguments are obligatory
        /// </summary>
        /// <param name="reader">Reader stream</param>
        /// <param name="writer">Writer stream</param>
        /// <param name="delimeter">Delimeter</param>
        public CSVReaderWriterNew(StreamReader reader, StreamWriter writer, char delimeter)
        {
            if (reader == null)
            {
                throw new ArgumentException("Parameter reader must not be null");
            }
            if (writer == null)
            {
                throw new ArgumentException("Parameter writer must not be null");
            }
            if (Char.IsWhiteSpace(delimeter))
            {
                throw new ArgumentException("Parameter delimeter must not be white space");
            }

            _readerStream = reader;
            _writerStream = writer;
            _delimeter = delimeter;
        }

        /// <summary>
        /// Creates new instance of CSVReaderWriterNew.
        /// </summary>
        /// <param name="inputFile">Input file name</param>
        /// <param name="outputFile">Output file name</param>
        /// <param name="delimeter">Delimeter</param>
        public CSVReaderWriterNew(string inputFile, string outputFile, char delimeter)
        {
            if (String.IsNullOrEmpty(inputFile))
            {
                throw new ArgumentException("Parameter inputFile must not be null or empty");
            }

            _readerStream = _readerStream = File.OpenText(inputFile);
            _delimeter = delimeter;

            if (!String.IsNullOrEmpty(outputFile))
            {
                _writerStream = new StreamWriter(outputFile);
            }
        }

        /// <summary>
        /// Creates new instance of CSVReaderWriterNew. Default delimeter is '\t'
        /// </summary>
        /// <param name="inputFile">Input file name</param>
        /// <param name="outputFile">Output file name</param>
        public CSVReaderWriterNew(string inputFile, string outputFile)
            : this(inputFile, outputFile, _defaultDelimeter)
        {
        }

        /// <summary>
        /// Creates new instance of CSVReaderWriterNew.
        /// </summary>
        /// <param name="inputFile">Input file name</param>
        /// <param name="delimeter">Delimeter</param>
        public CSVReaderWriterNew(string inputFile, char delimeter)
            : this(inputFile, String.Empty, delimeter)
        {
        }

        /// <summary>
        /// Creates new instance of CSVReaderWriterNew. Default delimeter is '\t'
        /// </summary>
        /// <param name="inputFile">Input file name</param>
        public CSVReaderWriterNew(string inputFile)
            : this(inputFile, String.Empty, _defaultDelimeter)
        {
        }

        public string[] Read(int expectedColumnsNumber = 0)
        {
            var line = _readerStream.ReadLine();

            if (line == null)
            {
                return null;
            }

            var tokens = line.Split(_delimeter);

            if (expectedColumnsNumber != 0 && tokens.Length != expectedColumnsNumber)
            {
                throw new IncorrectColumnsNumberException();
            }

            return tokens;
        }

        public void Write(params string[] columns)
        {
            if (_writerStream == null)
            {
                throw new WriterNotConfiguredException();
            }

            Write(_writerStream, columns);
        }

        //I would consider passing StreamWriter. For example depending on line content you might want to write to a different stream.
        public void Write(StreamWriter writer, params string[] columns)
        {
            var text = String.Join("\t", columns);

            writer.WriteLine(text);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }

    public class IncorrectColumnsNumberException : Exception
    { }

    public class WriterNotConfiguredException : Exception
    { }
}
