using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Ferry.Notification;
using Ferry;

namespace Ferry.Debug
{
	public partial class DebugForm : Form
	{
		public DebugForm()
		{
			InitializeComponent();
		}

		private void Reload_Click(object sender, EventArgs e)
		{


			Dictionary<string, ISystemElement>.KeyCollection collection = NotifySystem.Profile();

			int size = collection.Count;
			lblSize.Text = "Ferry Dictionary Entries: " + size;


			ReadDictionary(collection, ferryList);
		}

		private void ReadDictionary(Dictionary<string, ISystemElement>.KeyCollection collection, ListView lv)
		{

			foreach (string key in collection)
			{
				ISystemElement se = NotifySystem.GetSystemElement(key);
				string type = se.GetType().ToString();

			}
		}

		private void ferryList_SelectedIndexChanged(object sender, EventArgs e)
		{

		}

		private void DebugForm_Load(object sender, EventArgs e)
		{

		}
	}
}