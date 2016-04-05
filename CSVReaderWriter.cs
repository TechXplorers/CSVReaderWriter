using System;
using System.IO;

namespace Symphony
{
    /// <summary>
    /// A junior developer was tasked with writing a reusable solution for an application to read and write text files that hold tab separated data.
    /// His implementation, although it works and meets the needs of the application, is of very low quality.
    /// Your task:
    ///     -  Identify and annotate the shortcomings in the current implementation as if you were doing a code review, using comments in this file.
    ///     -  In a fresh solution, refactor this implementation into clean, elegant, rock-solid & well performing code, without over-engineering.
    ///     -  Where you make trade offs, comment & explain.”
    ///     -  Assume this code is in production and it needs to be backwards compatible. Therefore if you decide to change the public interface, 
    ///        please deprecate the existing methods. Feel free to evolve the code in other ways though.

    /// </summary>
    //You should implement interface ICSVReaderWriter for IoC and unit testing
    //You should add comments on all methods
    public class CSVReaderWriter
    {
        private StreamReader _readerStream = null;
        private StreamWriter _writerStream = null;

        //This enum should not be marked as Flags. There is no bitwise operation to be performed. There also is no need for this enum at all. See comments on Open method.
        [Flags]
        public enum Mode { Read = 1, Write = 2 };

        //There should be no need to call Open method twice to create reader and writer. There should be two arguments for inputFile and outputFile in a constructor with outputfile being empty.
        public void Open(string fileName, Mode mode)
        {
            if (mode == Mode.Read)
            {
                _readerStream = File.OpenText(fileName);
            }
            else if (mode == Mode.Write)
            {
                //Why do you need to create FileInfo first?
                FileInfo fileInfo = new FileInfo(fileName);
                _writerStream = fileInfo.CreateText();
            }
            else
            {
                throw new Exception("Unknown file mode for " + fileName);
            }
        }

        public void Write(params string[] columns)
        {
            //depending on company standards, should be "", string.Empty or String.Empty. There is no difference in performance or memory management but it's good to keep standards
            string outPut = "";

            //instead of joining string in a loop it's better to use String.Join("\t", columns);
            for (int i = 0; i < columns.Length; i++)
            {
                //It's better to use StringBuiler. Saves memory, important especially for big files
                outPut += columns[i];
                if ((columns.Length - 1) != i)
                {
                    outPut += "\t";
                }
            }

            WriteLine(outPut);
        }

        //Why do you limit this method to two columns, when in "Write" method columns are as params?
        //If parameters of this method are strings, this method only checks if the line could be read and had two columns in line
        public bool Read(string column1, string column2)
        {
            //What are those consts for? What if we'll need three or more columns?
            const int FIRST_COLUMN = 0;
            const int SECOND_COLUMN = 1;

            //There is no need to declare those variables here. You should do that while assigning values
            string line;
            string[] columns;

            //This separator should be set as method parameter or put directly in Split method
            char[] separator = { '\t' };

            line = ReadLine();
            //if ReadLine will return null, there will be an unhandled exception here
            columns = line.Split(separator);

            //If we have one or more than two columns in a file, is it an error?
            if (columns.Length == 2)
            {
                //These assignments are pointess
                column1 = columns[FIRST_COLUMN];
                column2 = columns[SECOND_COLUMN];

                return true;
            }
            else if (columns.Length == 0)
            {
                //These assignments are pointess
                column1 = null;
                column2 = null;

                return false;
            }

            //you should create a custom exception
            throw new Exception("Expected 2 columns in csv input file");
        }

        //If a file format will change (e.g. new column added), new method would have to be added. This one should be refactored and return array of strings.
        public bool Read(out string column1, out string column2)
        {
            //those consts are not needed, amount of columns should be taken from method parameter or from file content
            const int FIRST_COLUMN = 0;
            const int SECOND_COLUMN = 1;

            //There is no need to declare those variables here. You should do that while assigning values
            string line;
            string[] columns;

            //This separator should be set as method parameter or put directly in Split method
            char[] separator = { '\t' };

            line = ReadLine();

            if (line == null)
            {
                //return null. See below.
                column1 = null;
                column2 = null;

                return false;
            }

            columns = line.Split(separator);

            //if you would add int expectedColumnnsNumber as a method parameter, all you need to do here is to check if columns.Length == expectedColumnnsNumber. If yes, return columns, if no, return null
            if (columns.Length == 2)
            {
                column1 = columns[FIRST_COLUMN];
                column2 = columns[SECOND_COLUMN];

                return true;
            }
            else if (columns.Length == 0)
            {
                column1 = null;
                column2 = null;

                return false;
            }

            //you should create a custom exception
            throw new Exception("Expected 2 columns in csv input file");
        }

        //This method should be public, but not in an interface. This would allow mocking for unit tests
        private void WriteLine(string line)
        {
            //what if _writerStream is null?
            _writerStream.WriteLine(line);
        }

        //This method should be public, but not in an interface. This would allow mocking for unit tests
        private string ReadLine()
        {
            //what if _readerStream is null?
            return _readerStream.ReadLine();
        }

        //What if no one calls this method? The strams will stay open. This class should implement IDisposable and you should close streams there
        public void Close()
        {
            if (_writerStream != null)
            {
                _writerStream.Close();
            }

            if (_readerStream != null)
            {
                _readerStream.Close();
            }
        }
    }
}
