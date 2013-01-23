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
            args = new string[] { "--init-database" };
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
  
        }


        static void InitDataBase(string[] args)
        {
            Console.WriteLine("Initialize the DataBase");
            DataBaseCreator creator = new DataBaseCreator(@"C:\Users\HealerKx\Desktop\daq2.sql");
            creator.Execute();
        }

        static void InitDirectories(string[] args)
        {

        }

        static void RepairSystem(string[] args)
        {

        }
    }
}
