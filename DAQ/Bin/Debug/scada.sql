
#
# ReadMe: Run Scada.DAQ.Installer.exe --init-database
# To Create Database and tables.
#


DROP TABLE IF EXISTS `weather`;
CREATE TABLE `weather` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Time` datetime DEFAULT NULL,
  `Windspeed` varchar(8) DEFAULT NULL,
  `Direction` varchar(8) DEFAULT NULL,
  `Temperature` varchar(8) DEFAULT NULL,
  `Humidity` varchar(8) DEFAULT NULL,
  `Pressure` varchar(8) DEFAULT NULL,
  `Raingauge` varchar(8) DEFAULT NULL,
  `Dewpoint` varchar(8) DEFAULT NULL,
  `IfRain` bit(1) DEFAULT NULL,
  `Alarm` bit(1) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

#
# Data for table "weather"
#

DROP TABLE IF EXISTS `hipc_rec`;
CREATE TABLE `hipc_rec` (
`Id` int(11) NOT NULL AUTO_INCREMENT, /*采样ID,唯一号*/
`Time` datetime DEFAULT NULL,
`Doserate` varchar(18)  DEFAULT NULL, /*剂量率值，单位：nGy/h，数据格式：N14.2*/
`Highvoltage` varchar(8)  DEFAULT NULL, /*高压值，单位：V，数据格式：N4.2*/
`Battery` varchar(8), /*电池电压，单位：V，数据格式：N4.2*/
`Temperature` varchar(8), /*探测器温度，单位：℃，数据格式：N4.2*/
`Alarm` bit(1), /*报警，单位：无；数据格式：0、1、2,，代表不同的报警类型，保留字段*/
PRIMARY KEY (`Id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `HVSampler_rec`;
CREATE TABLE `HVSampler_rec` (
`Id` int(11) NOT NULL AUTO_INCREMENT, /*采样ID,唯一号*/
`Time` datetime DEFAULT NULL,
`Flowrate` varchar(8), /*瞬时流速，单位：m/s，数据格式：N8*/
`Volume` varchar(8), /*总采样体积，单位：m3，数据格式：N8*/
`Status` bit, /*真空泵开关状态，单位：无；数据格式：0或1表示开关*/
`Alarm` bit, /*报警，单位：无；数据格式：0、1、2,，代表不同的报警类型，保留字段*/
`FilterId` int, /*滤膜ID，保留字段*/
`Begintime` datetime DEFAULT NULL,
`Endtime` datetime, /*开始时间，保留字段*/
PRIMARY KEY (`Id`)
)ENGINE=MyISAM DEFAULT CHARSET=utf8;


DROP TABLE IF EXISTS `ISampler_rec`;
CREATE TABLE `ISampler_rec` (
`Id` int(11) NOT NULL AUTO_INCREMENT, /*采样ID,唯一号*/
`Time` datetime DEFAULT NULL,
`Flowrate` varchar(8), /*瞬时流速，单位：m/s，数据格式：N8*/
`Volume` varchar(8), /*总采样体积，单位：m3，数据格式：N8*/
`Status` bit, /*真空泵开关状态，单位：无；数据格式：0或1表示开关*/
`Alarm` bit, /*报警，单位：无；数据格式：0、1、2,，代表不同的报警类型，保留字段*/
`FilterId1` int, /*滤膜ID，保留字段*/
`FilterId2` int, /*滤膜ID，保留字段*/
`Begintime` datetime DEFAULT NULL,
`Endtime` datetime, /*开始时间，保留字段*/
PRIMARY KEY (`Id`)
)ENGINE=MyISAM DEFAULT CHARSET=utf8;


DROP TABLE IF EXISTS `RDSampler_rec`;
CREATE TABLE `RDSampler_rec` (
`Id` int(11) NOT NULL AUTO_INCREMENT, /*采样ID,唯一号*/
`Time` datetime DEFAULT NULL,
`IfRain` bit, /*感雨，单位：无，数据格式：0或1表示是否下雨*/
`Barrel` bit, /*桶状态，单位：无，数据格式：0、1、2表示哪个桶正在使用*/
`Alarm` bit,/*报警，单位：无；数据格式：0、1、2,，代表不同的报警类型，保留字段*/
`IsLidOpen` bit,
`CurrentRainTime` varchar(10) ,
`BeginTime` datetime DEFAULT NULL,/*开始时间，保留字段*/
`Endtime` datetime DEFAULT NULL,/*开始时间，保留字段*/
PRIMARY KEY (`Id`)
)ENGINE=MyISAM DEFAULT CHARSET=utf8;



DROP TABLE IF EXISTS `Environment_rec`;
CREATE TABLE `Environment_rec` (
`Id` int(11) NOT NULL AUTO_INCREMENT, /*采样ID,唯一号*/
`Time` datetime DEFAULT NULL,
`Temperature` varchar(8), /*温度，单位：℃，数据格式：N8*/
`Humidity` varchar(8), /*湿度，单位：%，数据格式：N8*/
`IfSmoke` bit, /*烟感报警，单位：无，数据格式：0或1表示是否报警*/
`IfWater` bit, /*浸水报警，单位：无，数据格式：0或1表示是否报警*/
`IfDoorOpen` bit, /*门禁报警，单位：无，数据格式：0或1表示是否报警*/
`Alarm` bit, /*报警，单位：无；数据格式：0、1、2,，代表不同的报警类型，保留字段*/
PRIMARY KEY (`Id`)
)ENGINE=MyISAM DEFAULT CHARSET=utf8;
