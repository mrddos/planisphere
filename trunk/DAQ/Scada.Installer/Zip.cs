using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Scada.Installer
{
    class Zip
    {
        private byte[] buffer = new byte[1024 * 5];

        public bool UnZipFile(string zipFilePath, string unZipDir, Func<string, Stream, bool> cb, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (zipFilePath == string.Empty)
            {
                errorMessage = "压缩文件不能为空！";
                return false;
            }
            if (!File.Exists(zipFilePath))
            {
                errorMessage = "压缩文件不存在！";
                return false;
            }
            if (unZipDir == string.Empty)
                unZipDir = zipFilePath.Replace(Path.GetFileName(zipFilePath), Path.GetFileNameWithoutExtension(zipFilePath));
            if (!unZipDir.EndsWith("\\"))
                unZipDir += "\\";
            if (!Directory.Exists(unZipDir))
                Directory.CreateDirectory(unZipDir);

            try
            {
                using (ZipInputStream s = new ZipInputStream(File.OpenRead(zipFilePath)))
                {
                    ZipEntry theEntry;
                    while ((theEntry = s.GetNextEntry()) != null)
                    {
                        string directoryName = Path.GetDirectoryName(theEntry.Name);
                        string fileName = Path.GetFileName(theEntry.Name);
                        string relFileName = directoryName + "\\" + fileName;
                        bool compare = cb(relFileName, null);
                        if (directoryName.Length > 0)
                        {
                            Directory.CreateDirectory(unZipDir + directoryName);
                        }

                        if (!directoryName.EndsWith("\\"))
                            directoryName += "\\";
                        if (fileName != String.Empty)
                        {
                            MemoryStream ms = new MemoryStream();
                            while (true)
                            {
                                int r = s.Read(buffer, 0, buffer.Length);
                                if (r > 0)
                                {
                                    ms.Write(buffer, 0, r);
                                }
                                else
                                {
                                    break;
                                }
                            }

                            if (compare)
                            {
                                ms.Seek(0, SeekOrigin.Begin);
                                cb(relFileName, ms);
                            }

                            using (FileStream streamWriter = File.Create(unZipDir + theEntry.Name))
                            {
                                ms.Seek(0, SeekOrigin.Begin);
                                while (true)
                                {
                                    int r = ms.Read(buffer, 0, buffer.Length);
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
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
            return true;
        }

    }
}
