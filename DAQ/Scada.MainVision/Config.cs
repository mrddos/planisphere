using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Scada.MainVision
{
	class ConfigItem
	{
		public ConfigItem(string key)
		{
			this.Key = key;
		}

		public string Key
		{
			get;
			private set;
		}

		public string ColumnName
		{
			get;
			set;
		}

		public int FieldIndex
		{
			get;
			set;
		}

	}

	class ConfigEntry
	{
		private List<ConfigItem> items = new List<ConfigItem>();

		public void Add(ConfigItem item)
		{
			items.Add(item);
		}

		public IEnumerable<ConfigItem> ConfigItems
		{
			get
			{
				return items;
			}
		}

	}

	class Config
	{
		private static Config configInstance = new Config();

		private Dictionary<string, ConfigEntry> dict = new Dictionary<string, ConfigEntry>();

		private string currentParsedDevice;

		public static Config Instance()
		{
			return configInstance;
		}


		public ConfigEntry this[string deviceKey]
		{
			get
			{
				return dict[deviceKey];
			}
		}

		internal void Load(string fileName)
		{
			using (StreamReader sr = new StreamReader(fileName))
			{
				string line = sr.ReadLine();
				while (line != null)
				{
					line = line.Trim();
					if (line.Length > 0)
					{
						this.ParseLine(line);
					}
					// Next
					line = sr.ReadLine();
				}
			}
		}

		private void ParseLine(string line)
		{
			// Handle Section
			if (line.StartsWith("[") && line.EndsWith("]"))
			{
				string deviceKey = line.Substring(1, line.Length - 2);
				deviceKey = deviceKey.Trim().ToLower();
				this.currentParsedDevice = deviceKey;
				dict.Add(deviceKey, new ConfigEntry());
				return;
			}

			if (line.IndexOf('=') > 0)
			{
				string[] kv = line.Split('=');
				if (kv.Length > 0)
				{
					ConfigEntry entry = dict[this.currentParsedDevice];
					string key = kv[0].Trim();
					string value = kv[1].Trim();

					this.ProcessLine(key, value, entry);
				}


			}

		}

		private void ProcessLine(string key, string value, ConfigEntry entry)
		{
			string[] v = value.Split(';');
			int c = v.Length;

			string columnName = v[0].Trim();
			string fieldIndex = v[1].Trim();

			var item = new ConfigItem(key);
			item.ColumnName = columnName;
			item.FieldIndex = int.Parse(fieldIndex.TrimStart('#'));
			entry.Add(item);
		}


	}
}
