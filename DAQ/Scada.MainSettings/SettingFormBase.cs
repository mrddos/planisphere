using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Scada.MainSettings
{
    public class SettingFormBase : UserControl
    {
        private PropertyGrid propertyGrid = null;

        public SettingFormBase()
        {
        }

        public void Loaded(object settings)
        {
            this.Width = 640;
            this.Height = 400;

            this.propertyGrid = new PropertyGrid();
            this.propertyGrid.HelpVisible = false;
            this.propertyGrid.ToolbarVisible = false;
            this.propertyGrid.Dock = DockStyle.Fill;
            this.Controls.Add(this.propertyGrid);
            InitializePropertyGrid(settings);
        }

        private void InitializePropertyGrid(object settings)
        {
            this.propertyGrid.SelectedObject = settings;
        }
    }
}
