using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ExportTool
{
    public partial class Form1 : Form
    {
        private IWorkbook mabangWorkbook;
        public Form1()
        {
            InitializeComponent();
            Database.SetInitializer<ExportToolEntities>(new CreateDatabaseIfNotExists<ExportToolEntities>());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Excel Files|*.xlsx;*.xls";
            openFileDialog1.FileName = "MABANG";
            DialogResult result = openFileDialog1.ShowDialog(); // Show the dialog.
            if (result == DialogResult.OK) // Test result.
            {
                string file = openFileDialog1.FileName;
                textBox1.Text = file;
                if (!string.IsNullOrEmpty(textBox1.Text) && !string.IsNullOrWhiteSpace(textBox1.Text))
                    button2.Enabled = true;
                else
                    button2.Enabled = false;
                //dataGridView1.DataSource = null;
                //button3.Enabled = false;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                using (FileStream mb = new FileStream(textBox1.Text, FileMode.Open, FileAccess.Read))
                {
                    mabangWorkbook = WorkbookFactory.Create(mb);
                }
                //var numSheet = schedule.NumberOfSheets;
                //programList = new List<MyProgram>();
                //for (int i = numSheet - 1; i < numSheet; i++)
                //{
                //    ISheet sheet = schedule.GetSheetAt(i);
                //    for (int j = 4; j <= sheet.LastRowNum; j++)
                //    {
                //        var row = sheet.GetRow(j);
                //        MyProgram program = new MyProgram();
                //        if (row != null) //null is when the row only contains empty cells 
                //        {
                //            if (row.GetCell(2) == null || row.GetCell(2).StringCellValue.ToString() == "")
                //            {
                //                break;
                //            }
                //            program.TapeCode = row.GetCell(4).StringCellValue.ToString();
                //            program.Name = row.GetCell(2).StringCellValue.ToString();
                //            program.Duration = row.GetCell(3).DateCellValue;
                //            program.Frequency = 1;

                //            if (!checkDuplicate(programList, program))
                //            {
                //                programList.Add(program);
                //            }
                //        }
                //    }
                //}
                //programList = programList.OrderBy(x => x.Name).ToList();
                //int index = 1;
                //dataGridView1.DataSource = programList.Select(x => new { SNO = index++, x.TapeCode, Duration = x.Duration.Minute.ToString() + ":" + x.Duration.Second.ToString(), x.Name, x.Frequency }).ToList();
                //dataGridView1.AutoResizeColumns();
                //dataGridView1.AutoGenerateColumns = false;
                //dataGridView1.AllowUserToAddRows = false;
                //button3.Enabled = true;
            }
            catch (IOException ex)
            {
                AlertForm alert = new AlertForm();
                alert.SetLabel(ex.Message.ToString());
                alert.ShowDialog();
            }
        }
    }
}
