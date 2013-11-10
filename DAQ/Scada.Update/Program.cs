using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Scada.Update
{
    class Program
    {
        static string GetCurrentPath()
        {
            string p = Assembly.GetExecutingAssembly().Location;
            return Path.GetDirectoryName(p);
        }

        static void Main(string[] args)
        {
            Updater u = new Updater();
            string binZipPath = GetCurrentPath() + "\\update\\bin.zip";
            KillProcesses();
            bool r = u.UnzipProgramFiles(binZipPath, GetCurrentPath());
            if (!r)
            {
                Console.WriteLine("Failed.");
            }
            RestoreProcesses();
        }


        static void KillProcesses()
        {

        }

        static void RestoreProcesses()
        {

        }

    }
}
