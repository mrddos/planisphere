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
using System.Threading;

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
            this.textBox1.Text = @"C:\Scada";

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

            this.UpdateUIStauts();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            this.textBox2.Text = this.textBox1.Text + "\\MySQL";
            this.textBox3.Text = this.textBox1.Text + "\\Bin";

            this.UpdateUIStauts();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!this.finished)
            {
                this.button1.Enabled = false;
                Thread thread = new Thread(new ThreadStart(() => 
                {
                    this.StartInstallProcess();
                }));
                thread.Start();
            }
            else
            {
                this.Close();
            }
        }

        // Start the Install process
        private bool StartInstallProcess()
        {
            if (!CreateFolders())
            {
                return false;
            }

            if (!UnzipMySQLFiles())
            {
                return false;
            }

            if (!PrepareMySQLConfigFile())
            {
                return false;
            }

            if (!RunMySQL())
            {
                return false;
            }

            if (!UnzipProgramFiles())
            {
                return false;
            }

            if (!CreateTables())
            {
                return false;
            }

            if (!CreateStartupMenu())
            {
                return false;
            }

            if (CreateDesktopIcons("Scada.Main.exe", "系统设备管理器") &&
                CreateDesktopIcons("Scada.MainVision.exe", "Nuclover - SCADA"))
            {
                if (this.installMode)
                {
                    this.AddLog("安装成功!");
                }
                else
                {
                    this.AddLog("更新成功!");
                }

                this.Invoke(new MyInvoke((object sender, string p) => 
                {
                    this.button1.Enabled = true;
                    this.button1.Text = "关闭";
                }), null, "");
                
                this.finished = true;
                return true;
            }
            else
            {
                this.AddLog("安装未完成!");
                return false;
            }
        }

        private bool RunMySQL()
        {
            try
            {
                Process[] mysqlds = Process.GetProcessesByName("mysqld");
                if (mysqlds != null && mysqlds.Length > 0)
                {
                    return true;
                }

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
                        this.AddLog("MySQL启动成功");
                    }
                    else
                    {
                        this.AddLog("MySQL启动失败");
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

                this.AddLog("启动组快捷方式创建成功");
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

                this.AddLog("桌面快捷方式创建成功");
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
            if (!this.resetCheckBox.Checked)
            {
                // Or use this.installMode to check.
                return true;
            }
            string fileName = "Scada.DAQ.Installer.exe";
            string filePath = string.Format("{0}\\{1}", this.textBox3.Text, fileName);
            using (Process process = new Process())
            {
                process.StartInfo.CreateNoWindow = true;    //设定不显示窗口
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.FileName = filePath; //设定程序名  
                process.StartInfo.RedirectStandardInput = true;   //重定向标准输入
                process.StartInfo.RedirectStandardOutput = true;  //重定向标准输出
                process.StartInfo.RedirectStandardError = true;//重定向错误输出

                process.StartInfo.Arguments = "--init-database-s";
                bool ret = process.Start();
                if (ret)
                {
                    this.AddLog("数据库初始化成功");
                }
                else
                {
                    this.AddLog("数据库初始化失败");
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
                this.AddLog("Error: 未找到文件: bin.zip!");
                return false;
            }
            this.AddLog("解压缩程序中... (请关闭相关进程，否则解压会失败!)");
            string errorMessage;

            bool ret = Zip.UnZipFile(programZipFile, destPath, out errorMessage);
            if (ret)
            {
                this.AddLog("解压缩程序成功!");
            }
            else
            {
                this.AddLog("解压缩程序失败!");
            }
            return ret;
        }

        private bool PrepareMySQLConfigFile()
        {
            return true;
        }

        private bool MySQLExists()
        {
            return this.MySQLExistsAt(this.textBox2.Text);
        }

        private bool MySQLExistsAt(string mysqlPath)
        {
            string mysqld = string.Format("{0}\\{1}\\bin\\mysqld.exe", mysqlPath, this.mySqlVersion);
            return File.Exists(mysqld);
        }

        private bool UnzipMySQLFiles()
        {
            if (!this.checkBoxMySQL.Checked)
            {
                string mysqld = string.Format("{0}\\{1}\\bin\\mysqld.exe", this.textBox2.Text, this.mySqlVersion);
                if (File.Exists(mysqld))
                {
                    return true;
                }
                else
                {
                    MessageBox.Show(mysqld + "不存在，请把MySQL安装到正确的位置后再点击'确定'。");
                    return true;
                }
            }

            string mySqlZipFile = string.Format("{0}\\{1}", System.Environment.CurrentDirectory, "mysql.zip");

            string destPath = this.textBox2.Text;
            if (!File.Exists(mySqlZipFile))
            {
                this.AddLog("Error: 未找到文件 mysql.zip");
                return false;
            }
            this.AddLog("解压MySQL...");
            string errorMessage;
            bool ret = Zip.UnZipFile(mySqlZipFile, destPath, out errorMessage);
            if (ret)
            {
                this.AddLog("解压缩数据库成功");
            }
            else
            {
                this.AddLog("解压缩数据库失败");
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

                this.AddLog("目录创建成功");
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
                string startMysqlScript = string.Format("start {0}\\{1}\\bin\\mysqld.exe", this.textBox2.Text, this.mySqlVersion);
                sw.WriteLine(startMysqlScript);
                sw.WriteLine();

                string binPath = this.textBox3.Text;
                // Run MDS.exe
                string startMDSScript = string.Format("start {0}\\mds.exe", binPath);
                sw.WriteLine(startMDSScript);
                sw.WriteLine("ping -n 5 127.0.0.1");

                // Run AIS.exe
                string startAISScript = string.Format("start {0}\\ais.exe", binPath);
                sw.WriteLine(startAISScript);
                sw.WriteLine("ping -n 5 127.0.0.1");

                // Run Scada.Main.exe
                string startMainScript = string.Format("start {0}\\Scada.Main.exe /ALL", binPath);
                sw.WriteLine(startMainScript);
                sw.WriteLine("ping -n 30 127.0.0.1");
                sw.WriteLine();

                // Run Scada.DataCenterAgent.exe
                string startAgentScript = string.Format("start {0}\\Scada.DataCenterAgent.exe --start", binPath);
                sw.WriteLine(startAgentScript);
                sw.WriteLine();
            }
            fs.Close();

            return p;
        }

        /// <summary>
        /// /////////////////////////////////////////
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="msg"></param>
        public delegate void MyInvoke(object sender, string msg);

        private void AddLog(string msg)
        {
            this.Invoke(new MyInvoke(this.AddString), this, msg);
        }

        private void AddString(object sender, string line)
        {
            this.progressBox.Items.Add(line);
        }

        private void resetCheckBox_CheckedChanged(object sender, EventArgs e)
        {

        }

        private bool installMode = true;
        private void UpdateUIStauts()
        {
            if (MySQLExists())
            {
                installMode = false;
                this.button1.Text = "更新";
                this.resetCheckBox.Checked = false;
            }
            else
            {
                installMode = true;
                this.button1.Text = "安装";
                this.resetCheckBox.Checked = true;
            }
            
        }
        
    }
}
