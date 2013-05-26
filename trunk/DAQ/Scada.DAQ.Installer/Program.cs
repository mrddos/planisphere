using System;
using System.Collections.Generic;
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
            else if (fa == "--init-dirs")
            {
                InitDirectories(args);
            }
            else if (fa == "--repair-system")
            {
                RepairSystem(args);
            }
            else if (fa == "--m-hipc")
            {
                MockHipcInsertData();
            }
        }

        static void InitDataBase(string[] args)
        {
            Console.WriteLine("Initialize the DataBase:");
            Console.WriteLine("Notice: This Command would clear all you record in tables!");
            Console.WriteLine("Tap 'Yes' to continue.");
            string input = Console.ReadLine();
            if (input == "Yes")
            {
                Console.WriteLine("Execute .\\scada.sql");
                DataBaseCreator creator = new DataBaseCreator(@".\scada.sql");
                creator.Execute();
            }

        }

        static void InitDirectories(string[] args)
        {

        }

        static void RepairSystem(string[] args)
        {

        }

        static void MockHipcInsertData()
        {
            DataBaseInsertion ins = new DataBaseInsertion();
            ins.Execute();
        }
    }
}
