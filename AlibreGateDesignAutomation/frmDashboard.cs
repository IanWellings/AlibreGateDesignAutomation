using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AlibreGateDesignAutomation.DesignTemplates;

namespace AlibreGateDesignAutomation
{
    public partial class frmDashboard : Form
    {
        public frmDashboard()
        {
            InitializeComponent();
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            //Set the correct design template to use
            string designTemplate = "TallGardenGate";
            if (designTemplate == "TallGardenGate")
            {
                TallGardenGate.AlibreAutomation(Int32.Parse(txtHeightInput.Text), Int32.Parse(txtWidthInput.Text));
            }
        }
    }
}
