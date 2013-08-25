using Scada.Config;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Scada.MainSettings
{
    public class SerialPortSetting
    {
        private string serialPort = "COM1";

        [Category("串口")]
        [TypeConverter(typeof(SerialPortConverter))]
        [DisplayName("串口")]
        public string SerialPort
        {
            get 
            { 
                return this.serialPort; 
            }
            set
            {
                this.serialPort = value;
            }
        }
    }


    /// <summary>
    /// 1
    /// </summary>
    [DefaultProperty("AlertValue")]  
    public class HpicSettings : SerialPortSetting
    {

        [Category("高压电离室")]
        [DisplayName("剂量率因子")]
        [DefaultValue(1.0)]
        public double Factor
        {
            get;
            set;
        }

        [Category("报警")]
        [DisplayName("剂量率报警阀值")]
        [DefaultValue(100.0)]
        public double AlertValue
        {
            get;
            set;
        }

        [Category("高压电离室")]
        [DisplayName("采集频率")]
        [DefaultValue(60)]
        public int Frequence
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 2
    /// </summary>
    public class WeatherSettings : SerialPortSetting
    {
        [Category("高压电离室")]
        [DisplayName("采集频率")]
        [DefaultValue(60)]
        public int Frequence
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 3
    /// </summary>
    public class MdsSettings
    {
        [Category("高压电离室")]
        [DisplayName("采集频率")]
        [DefaultValue(60)]
        public int Frequence
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 4
    /// </summary>
    public class AisSettings
    {
        [Category("高压电离室")]
        [DisplayName("采集频率")]
        [DefaultValue(60)]
        public int Frequence
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 5
    /// </summary>
    public class NaISettings
    {
        [Category("高压电离室")]
        [DisplayName("采集频率")]
        [DefaultValue(60)]
        public int Frequence
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 6
    /// </summary>
    public class DwdSettings : SerialPortSetting
    {
        [Category("高压电离室")]
        [DisplayName("采集频率")]
        [DefaultValue(60)]
        public int Frequence
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 7
    /// </summary>
    public class ShelterSettings : SerialPortSetting
    {
        [Category("高压电离室")]
        [DisplayName("采集频率")]
        [DefaultValue(60)]
        public int Frequence
        {
            get;
            set;
        }
    }
}
