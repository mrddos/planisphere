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
using System.Reflection;

// using IWshRuntimeLibrary;

namespace Scada.Installer
{
    public partial class InstallerForm : Form
    {
        public InstallerForm()
        {
            InitializeComponent();
        }

        private bool finished = false;

        private void SelectPath()
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.SelectedPath = @"C:\";
            DialogResult dr = fbd.ShowDialog();
            if (dr == DialogResult.OK)
            {
                this.installPath.Text = fbd.SelectedPath;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.installPath.Text = @"C:\Scada";
        }

        private void InstallOrUpdateClick(object sender, EventArgs e)
        {
            if (!this.finished)
            {
                this.buttonInstall.Enabled = false;
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

            if (!UnzipProgramFiles())
            {
                return false;
            }

            if (!CheckVersions())

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
                    LaunchMainSettings();
                }
                else
                {
                    this.AddLog("更新成功!");
                    LaunchMainSettings();
                }

                this.Invoke(new MyInvoke((object sender, string p) => 
                {
                    this.buttonInstall.Enabled = true;
                    this.buttonInstall.Text = "关闭";
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

        private bool CheckVersions()
        {
            // TODO: Main

            // TODO: MainVision
            return true;
        }

        private void LaunchMainSettings()
        {
            string fileName = "Scada.MainSettings.exe";
            string filePath = string.Format("{0}\\{1}", this.installPath.Text, fileName);
            using (Process process = new Process())
            {
                process.StartInfo.CreateNoWindow = false;           //设定不显示窗口
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.FileName = filePath;              //设定程序名  
                process.StartInfo.RedirectStandardInput = true;     //重定向标准输入
                process.StartInfo.RedirectStandardOutput = true;    //重定向标准输出
                process.StartInfo.RedirectStandardError = true;     //重定向错误输出

                process.StartInfo.Arguments = "--first-time";
                process.Start();
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
                shortcut.WorkingDirectory = this.installPath.Text;
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
                shortcut.WorkingDirectory = this.installPath.Text;
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
            return string.Format("{0}\\{1}", this.installPath.Text, fileName);
        }

        private bool CreateTables()
        {
            if (!this.resetCheckBox.Checked)
            {
                // Or use this.installMode to check.
                return true;
            }
            string fileName = "Scada.DAQ.Installer.exe";
            string filePath = string.Format("{0}\\{1}", this.installPath.Text, fileName);
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
            string programZipFile = string.Format("{0}\\{1}", GetInstallPath(), "\\bin.zip");
            string destPath = this.installPath.Text;
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

        // TODO:
        private bool CreateFolders()
        {
            try
            {
                string programPath = this.installPath.Text;
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
            string p = string.Format("{0}\\startup.bat", this.installPath.Text);
            if (File.Exists(p))
            {
                File.Delete(p);
            }
            FileStream fs = File.Create(p);
            using (StreamWriter sw = new StreamWriter(fs))
            {
                string binPath = this.installPath.Text;
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


        private string GetInstallPath()
        {
            string p = Assembly.GetExecutingAssembly().Location;
            return Path.GetDirectoryName(p);
        }

        private void SelectPathButtonClick(object sender, EventArgs e)
        {
            this.SelectPath();
        }
        
    }
}
