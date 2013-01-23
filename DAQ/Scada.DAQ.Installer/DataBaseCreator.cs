using MySql.Data.MySqlClient;
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

        private MySqlConnection conn;

        private string connectionString;

        public DataBaseCreator(string dataBaseFile)
        {
            this.dataBaseFile = dataBaseFile;
            this.conn = new MySqlConnection(connectionString);
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
                MySqlCommand cmd = this.conn.CreateCommand();
                string line = sr.ReadLine();
                while (line != null)
                {
                    string statement = parser.Add(line);
                    if (statement.Length > 0)
                    {
                        this.ExecuteSQL(cmd, statement);
                    }
                    line = sr.ReadLine();
                }

            }
        }

        internal void ExecuteSQL(MySqlCommand cmd, string statement)
        {
            string log = string.Format("Execute SQL: {0}", statement);
            Console.WriteLine(log);
            cmd.CommandText = statement;
            cmd.ExecuteNonQuery();
        }
    }
}
