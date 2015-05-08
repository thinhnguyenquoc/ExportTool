using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ExportTool
{
    public partial class AlertForm : Form
    {
        public AlertForm()
        {
            InitializeComponent();
            label1.MaximumSize = new Size(300, 0);
            label1.AutoSize = true;
        }

        public void SetLabel(string str)
        {
            label1.Text = str;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
        }
    }
}
