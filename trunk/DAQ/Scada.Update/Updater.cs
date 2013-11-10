﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace Scada.Update
{
    public enum WriteFileResult
    {
        SameFile,
        DiffFile
    }

    public class Updater
    {

        private string destPath;

        public Updater()
        {
        }

        public bool UnzipProgramFiles(string programZipFile, string destPath)
        {
            if (!File.Exists(programZipFile))
            {
                // Error: bin.zip NOT found.
                return false;
            }
            this.destPath = destPath;
            Zip zip = new Zip();
            string errorMessage;
            bool ret = zip.UnZipFile(programZipFile, destPath, UnzipFileHandler, out errorMessage);
            return ret;
        }

        private UnzipCode UnzipFileHandler(string fileName, Stream fileStream)
        {
            fileName = fileName.ToLower();
            if (fileName.EndsWith(".cfg") ||
                fileName.EndsWith("local.ip") ||
                fileName.EndsWith("password") ||
                fileName.EndsWith(".bat") ||
                fileName.EndsWith(".settings"))
            {
                if (fileStream != null)
                {
                    WriteFile(fileStream, this.destPath + "\\" + fileName);
                }
                return UnzipCode.Compare;
            }

            if (fileName.EndsWith("scada.update.exe") || fileName.EndsWith("icsharpcode.sharpziplib.dll"))
            {
                Console.WriteLine("File <" + fileName + "> In use:!");
                return UnzipCode.Ignore;
            }

            return UnzipCode.None;
            
        }

        private WriteFileResult WriteFile(Stream stream, string fileName)
        {
            string nzFileName = fileName;
            if (File.Exists(fileName))
            {
                nzFileName += ".n!";
            }
            
            using (FileStream streamWriter = File.Create(nzFileName))
            {
                stream.Seek(0, SeekOrigin.Begin);
                byte[] buffer = new byte[stream.Length];
                while (true)
                {
                    int r = stream.Read(buffer, 0, buffer.Length);
                    if (r > 0)
                    {
                        streamWriter.Write(buffer, 0, r);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            if (fileName == nzFileName)
            {
                return WriteFileResult.SameFile;
            }

            HashAlgorithm hash = HashAlgorithm.Create();
            if (GetFileHashString(fileName, hash) == GetFileHashString(nzFileName, hash))
            {
                File.Delete(nzFileName);
                return WriteFileResult.SameFile;
            }
            return WriteFileResult.DiffFile;
        }

        private string GetFileHashString(string fileName, HashAlgorithm hash)
        {
            using (FileStream stream = new FileStream(fileName, FileMode.Open))
            {
                byte[] hashbyte = hash.ComputeHash(stream);
                return BitConverter.ToString(hashbyte);
            }
        }

        private string GetUpdateBinZipPath()
        {
            string p = Assembly.GetExecutingAssembly().Location;
            return Path.GetDirectoryName(p);
        }

    }
}
