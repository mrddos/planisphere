﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scada.Controls.Data
{
	public delegate void OnDataArrivalBegin();

	public delegate void OnDataArrival(Dictionary<string, object> data);

	public delegate void OnDataArrivalEnd();

    public class ColumnInfo
    {
        public string Header
        {
            get;
            set;
        }

        public string BindingName
        {
            get;
            set;
        }

        public double Width
        {
            get;
            set;
        }

        public bool DisplayInChart
        {
            get;
            set;
        }
    }

	public abstract class DataListener
	{
		public DataListener()
		{

		}

        public string DeviceKey
        {
            get;
            set;
        }

		public OnDataArrival OnDataArrival
		{
			get;
			set;
		}

		public OnDataArrivalBegin OnDataArrivalBegin
		{ 
			get;
			set;
		}

		public OnDataArrivalEnd OnDataArrivalEnd
		{
			get;
			set;
		}

        public abstract List<ColumnInfo> GetColumnsInfo();

    }
}
