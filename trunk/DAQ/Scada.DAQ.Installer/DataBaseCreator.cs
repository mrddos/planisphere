using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Scada.DAQ.Installer
{
    internal class DataBaseCreator
    {
        private string dataBaseFile;

        public DataBaseCreator(string dataBaseFile)
        {
            this.dataBaseFile = dataBaseFile;
        }


        internal void Execute()
        {
            if (dataBaseFile != null && dataBaseFile.Length > 0)
            {
                if (File.Exists(dataBaseFile))
                {
                    using (FileStream fs = new FileStream(dataBaseFile, FileMode.Open))
                    {
                        long size = fs.Length;
                        byte[] buffer = new byte[size];
                        int r = fs.Read(buffer, 0, (int)size);
                        string content = Encoding.UTF8.GetString(buffer);
                        this.Execute(content);
                    }
                }
            }
        }

        internal void Execute(string content)
        {
            SQLStatementParser parser = new SQLStatementParser();
            using(StringReader sr = new StringReader(content))
            {
                string line = sr.ReadLine();
                while (line != null)
                {
                    string statement = parser.Add(line);
                    if (statement.Length > 0)
                    {
                        this.ExecuteSQL(statement);
                    }
                    line = sr.ReadLine();
                }

            }
        }

        internal void ExecuteSQL(string statement)
        {
            string log = string.Format("Execute SQL: {0}", statement);
            Console.WriteLine(log);
        }
    }
}
