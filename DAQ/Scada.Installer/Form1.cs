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

// using IWshRuntimeLibrary;

namespace Scada.Installer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private string mySqlVersion = "";

        private bool finished = false;

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
            this.textBox1.Text = @"C:\Users\HealerKx\Projects\DAQ-Proj\DAQ\Bin\install";

            this.textBox4.Text = "."; 
            this.textBox5.Text = ".";
            if (File.Exists("mysql.version"))
            {
                using (StreamReader sr = new StreamReader("mysql.version"))
                {
                    this.mySqlVersion = sr.ReadLine();
                }
            }
            else
            {
                this.mySqlVersion = "mysql-5.1.60-win32";
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            this.textBox2.Text = this.textBox1.Text + "\\MySQL";
            this.textBox3.Text = this.textBox1.Text + "\\Bin";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!this.finished)
            {
                StartInstallProcess();
            }
            else
            {
                this.Close();
            }
        }

        // Start the Install process
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

            if (!RunMySQL())
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

            if (!CreateStartupMenu())
            {
                return;
            }

            if (CreateDesktopIcons("Scada.Main.exe", "系统设备管理器") &&
                CreateDesktopIcons("Scada.MainVision.exe", "Nuclover - SCADA"))
            {
                this.progressBox.Items.Add("安装成功!");
                this.button1.Text = "关闭";
                this.finished = true;
            }
        }

        private bool RunMySQL()
        {
            try
            {
                string mysqld = string.Format("{0}\\{1}\\bin\\mysqld.exe", this.textBox2.Text, this.mySqlVersion);
                using (Process process = new Process())
                {
                    process.StartInfo.CreateNoWindow = false;    //设定不显示窗口
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.FileName = mysqld; //设定程序名  
                    process.StartInfo.RedirectStandardInput = true;   //重定向标准输入
                    process.StartInfo.RedirectStandardOutput = true;  //重定向标准输出
                    process.StartInfo.RedirectStandardError = true;//重定向错误输出

                    process.StartInfo.Arguments = "";
                    bool ret = process.Start();
                    if (ret)
                    {
                        this.progressBox.Items.Add("MySQL启动成功");
                    }
                    else
                    {
                        this.progressBox.Items.Add("MySQL启动失败");
                    }
                    return ret;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }

        private bool CreateStartupMenu()
        {
            try
            {
                string p = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
                string s = this.CreateStartupBatFile();
                IWshRuntimeLibrary.WshShell shell = new IWshRuntimeLibrary.WshShell();

                IWshRuntimeLibrary.WshShortcut shortcut = (IWshRuntimeLibrary.WshShortcut)shell.CreateShortcut(p + "\\startup.lnk");
                shortcut.TargetPath = s;
                shortcut.Arguments = "";
                shortcut.Description = "启动";
                shortcut.WorkingDirectory = this.textBox3.Text;
                shortcut.IconLocation = string.Format("{0},0", s);
                shortcut.Save();

                this.progressBox.Items.Add("启动组快捷方式创建成功");
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        private bool CreateDesktopIcons(string fileName, string linkName)
        {
            try
            {
                string p = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string s = this.GetBinFile(fileName);
                IWshRuntimeLibrary.WshShell shell = new IWshRuntimeLibrary.WshShell();

                IWshRuntimeLibrary.WshShortcut shortcut = (IWshRuntimeLibrary.WshShortcut)shell.CreateShortcut(p + "\\" + linkName + ".lnk");
                shortcut.TargetPath = s;
                shortcut.Arguments = "";
                shortcut.Description = fileName;
                shortcut.WorkingDirectory = this.textBox3.Text;
                shortcut.IconLocation = string.Format("{0},0", s);
                shortcut.Save();

                this.progressBox.Items.Add("桌面快捷方式创建成功");
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        private string GetBinFile(string fileName)
        {
            return string.Format("{0}\\{1}", this.textBox3.Text, fileName);
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

                process.StartInfo.Arguments = "--init-database-s";
                bool ret = process.Start();
                if (ret)
                {
                    this.progressBox.Items.Add("数据库初始化成功");
                }
                else
                {
                    this.progressBox.Items.Add("数据库初始化失败");
                }
                return ret;
            }
        }

        private bool UnzipProgramFiles()
        {
            string programZipFile = string.Format("{0}\\{1}", System.Environment.CurrentDirectory, "\\bin.zip");
            string destPath = this.textBox3.Text;
            if (!File.Exists(programZipFile))
            {
                this.progressBox.Items.Add("Error: 未找到文件: bin.zip");
                return false;
            }
            this.progressBox.Items.Add("解压缩程序中...");
            string errorMessage;

            bool ret = Zip.UnZipFile(programZipFile, destPath, out errorMessage);
            if (ret)
            {
                this.progressBox.Items.Add("解压缩程序成功");
            }
            else
            {
                this.progressBox.Items.Add("解压缩程序失败");
            }
            return ret;
        }

        private bool PrepareMySQLConfigFile()
        {
            return true;
        }

        private bool UnzipMySQLFiles()
        {
            //return true;
            string mySqlZipFile = string.Format("{0}\\{1}", System.Environment.CurrentDirectory, "mysql.zip");

            string destPath = this.textBox2.Text;
            if (!File.Exists(mySqlZipFile))
            {
                this.progressBox.Items.Add("Error: 未找到文件 mysql.zip");
                return false;
            }
            this.progressBox.Items.Add("解压缩数据库中...");
            string errorMessage;
            bool ret = Zip.UnZipFile(mySqlZipFile, destPath, out errorMessage);
            if (ret)
            {
                this.progressBox.Items.Add("解压缩数据库成功");
            }
            else
            {
                this.progressBox.Items.Add("解压缩数据库失败");
            }
            return ret;
        }

        // TODO:
        private bool CreateFolders()
        {
            try
            {
                string mySqlPath = this.textBox2.Text;
                string programPath = this.textBox3.Text;

                Directory.CreateDirectory(mySqlPath);
                Directory.CreateDirectory(programPath);

                this.progressBox.Items.Add("目录创建成功");
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }


        private string CreateStartupBatFile()
        {
            string p = string.Format("{0}\\startup.bat", this.textBox3.Text);
            if (File.Exists(p))
            {
                File.Delete(p);
            }
            FileStream fs = File.Create(p);
            using (StreamWriter sw = new StreamWriter(fs))
            {
                // Run MySQL
                string edb = string.Format("start {0}\\{1}\\bin\\mysqld.exe", this.textBox2.Text, this.mySqlVersion);
                sw.WriteLine(edb);
                sw.WriteLine();

                // Run MDS.exe
                string emds = string.Format("start {0}\\mds.exe", this.textBox4.Text);
                sw.WriteLine(emds);
                sw.WriteLine();

                // Run AIS.exe
                string eais = string.Format("start {0}\\mds.exe", this.textBox5.Text);
                sw.WriteLine(eais);
                sw.WriteLine();

                // Run Scada.Main.exe
                string emain = string.Format("start {0}\\Scada.Main.exe /ALL", this.textBox3.Text);
                sw.WriteLine(emain);
                sw.WriteLine();
            }
            fs.Close();

            return p;
        }

        private void buttonMds_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.SelectedPath = @"C:\";
            DialogResult dr = fbd.ShowDialog();
            if (dr == DialogResult.OK)
            {
                this.textBox4.Text = fbd.SelectedPath;
            }
        }

        private void buttonAis_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.SelectedPath = @"C:\";
            DialogResult dr = fbd.ShowDialog();
            if (dr == DialogResult.OK)
            {
                this.textBox5.Text = fbd.SelectedPath;
            }
        }


        
    }
}
