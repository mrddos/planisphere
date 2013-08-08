using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Scada.DAQ.Installer
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                args = new string[] { "--init-database" };
            }

            if (args.Length == 0)
            {
                Console.WriteLine("Args required");
            }
            string fa = args[0].ToLower();
            if (fa == "--init-database")
            {
                InitDataBase(args);
            }
            else if (fa == "--init-database-s")
            {
                // Debug.Assert(false);
                InitDataBaseSilent(args);
            }
            else if (fa == "--init-dirs")
            {
                InitDirectories(args);
            }
            else if (fa == "--repair-system")
            {
                RepairSystem(args);
            }
            else if (fa == "--m-hpic")
            {
                MockInsertData("hpic");
            }
            else if (fa == "--m-weather")
            {
                MockInsertData("weather");
            }
            else if (fa == "--m-shelter")
            {
                MockInsertData("shelter");
            }
            else if (fa == "--m-nai")
            {
                MockInsertData("nai");
            }
            else if (fa == "--m-nai-file")
            {
                MockCreateNaIFiles();
            }
            else if (fa == "--m-nai")
            {
                MockInsertData("nai");
            }

        }

        private static void MockCreateNaIFiles()
        {
            // throw new NotImplementedException();
        }

        static void InitDataBase(string[] args)
        {
            Console.WriteLine("Initialize the DataBase:");
            Console.WriteLine("Notice: This Command would clear all you record in tables!");
            Console.WriteLine("Tap 'Yes' to continue.");
            string input = Console.ReadLine();
            if (input == "Yes")
            {
                Type type = typeof(Program);
                string fn = type.Assembly.Location;
                string sqlFileName = string.Format("{0}\\..\\scada.sql", fn);
                DataBaseCreator creator = new DataBaseCreator(sqlFileName);
                creator.Execute();
            }
        }

        static void InitDataBaseSilent(string[] args)
        {
            Type type = typeof(Program);
            string fn = type.Assembly.Location;
            string sqlFileName = string.Format("{0}\\..\\scada.sql", fn);
            DataBaseCreator creator = new DataBaseCreator(sqlFileName);
            creator.Execute();
        }

        static void InitDirectories(string[] args)
        {

        }

        static void RepairSystem(string[] args)
        {

        }

        static void MockInsertData(string device)
        {
            DataBaseInsertion ins = new DataBaseInsertion(device);
            ins.Execute();
        }
    }
}
