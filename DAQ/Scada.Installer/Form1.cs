using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ICSharpCode.SharpZipLib.Zip;
using System.Diagnostics;

namespace Scada.Installer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.SelectedPath = @"C:\";
            DialogResult dr = fbd.ShowDialog();
            if (dr == DialogResult.OK)
            {
                this.textBox1.Text = fbd.SelectedPath;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.textBox1.Text = @"d:\Scada";
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            this.textBox2.Text = this.textBox1.Text + "\\MySQL";
            this.textBox3.Text = this.textBox1.Text + "\\Bin";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            StartInstallProcess();
        }


        private void StartInstallProcess()
        {
            if (!CreateFolders())
            {
                return;
            }

            if (!UnzipMySQLFiles())
            {
                return;
            }

            if (!PrepareMySQLConfigFile())
            {
                return;
            }

            if (!UnzipProgramFiles())
            {
                return;
            }

            if (!CreateTables())
            {
                return;
            }
        }

        private bool CreateTables()
        {
            string fileName = "Scada.DAQ.Installer.exe";
            string filePath = string.Format("{0}\\{1}", this.textBox3.Text, fileName);
            using (Process process = new Process())
            {
                process.StartInfo.CreateNoWindow = false;    //设定不显示窗口
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.FileName = filePath; //设定程序名  
                process.StartInfo.RedirectStandardInput = true;   //重定向标准输入
                process.StartInfo.RedirectStandardOutput = true;  //重定向标准输出
                process.StartInfo.RedirectStandardError = true;//重定向错误输出

                process.StartInfo.Arguments = "--init-database";
                return process.Start();
            }
        }

        private bool UnzipProgramFiles()
        {
            string programZipFile = string.Format("{0}\\{1}", System.Environment.CurrentDirectory, "install\\program.zip");
            string destPath = this.textBox3.Text;
            if (!File.Exists(programZipFile))
            {
                return false;
            }
            string errorMessage;
            return UnZipFile(programZipFile, destPath, out errorMessage);
        }

        private bool PrepareMySQLConfigFile()
        {
            throw new NotImplementedException();
        }

        private bool UnzipMySQLFiles()
        {
            string mySqlZipFile = string.Format("{0}\\{1}", System.Environment.CurrentDirectory, "install\\mysql.zip");
            string destPath = this.textBox2.Text;
            if (!File.Exists(mySqlZipFile))
            {
                return false;
            }
            string errorMessage;
            return UnZipFile(mySqlZipFile, destPath, out errorMessage);
        }

        private static bool UnZipFile(string zipFilePath, string unZipDir, out string errorMessage)
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
                        if (directoryName.Length > 0)
                        {
                            Directory.CreateDirectory(unZipDir + directoryName);
                        }
                        if (!directoryName.EndsWith("\\"))
                            directoryName += "\\";
                        if (fileName != String.Empty)
                        {
                            using (FileStream streamWriter = File.Create(unZipDir + theEntry.Name))
                            {

                                int size = 2048;
                                byte[] data = new byte[2048];
                                while (true)
                                {
                                    size = s.Read(data, 0, data.Length);
                                    if (size > 0)
                                    {
                                        streamWriter.Write(data, 0, size);
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

        // TODO:
        private bool CreateFolders()
        {
            string mySqlPath = this.textBox2.Text;
            string programPath = this.textBox3.Text;

            Directory.CreateDirectory(mySqlPath);
            Directory.CreateDirectory(programPath);

            return true;
        }


        
    }
}
