using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NPOI;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Formula.Functions;

namespace ExportTool
{
    public partial class Form1 : Form
    {
        XSSFWorkbook schedule;
        List<MyProgram> programList;
        public Form1()
        {
            InitializeComponent();
            button2.Enabled = false;
            button3.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int size = -1;
            DialogResult result = openFileDialog1.ShowDialog(); // Show the dialog.
            if (result == DialogResult.OK) // Test result.
            {
                string file = openFileDialog1.FileName;
                textBox1.Text = file;
                button2.Enabled = true;
                //try
                //{
                //    string text = File.ReadAllText(file);
                //    size = text.Length;
                //}
                //catch (IOException)
                //{
                //}
            }
            Console.WriteLine(size); // <-- Shows file size in debugging mode.
            Console.WriteLine(result); // <-- For debugging use.
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (FileStream sch = new FileStream(textBox1.Text, FileMode.Open, FileAccess.Read))
            {
                schedule = new XSSFWorkbook(sch);

            }
            var numSheet = schedule.NumberOfSheets;
            programList = new List<MyProgram>();
            for (int i = 0; i < numSheet; i++)
            {
                ISheet sheet = schedule.GetSheetAt(i);
                for (int j = 4; j <= sheet.LastRowNum; j++)
                {
                    var row = sheet.GetRow(j);
                    MyProgram program = new MyProgram();
                    if (row != null) //null is when the row only contains empty cells 
                    {
                        if (row.GetCell(2) == null || row.GetCell(2).StringCellValue.ToString() == "")
                        {
                            break;
                        }
                        program.TapeCode = row.GetCell(4).StringCellValue.ToString();
                        program.Name = row.GetCell(2).StringCellValue.ToString();
                        program.Duration = row.GetCell(3).DateCellValue;

                        if (!checkDuplicate(programList, program))
                        {
                            programList.Add(program);
                        }
                    }
                }
            }
            int index = 1;
            dataGridView1.DataSource = programList.Select(x => new { SNO = index++, x.TapeCode, Duration = x.Duration.Minute.ToString()+":"+x.Duration.Second.ToString(), x.Name }).ToList();
            dataGridView1.AutoResizeColumns();
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.AllowUserToAddRows = false;
            button3.Enabled = true;
        } 

        private bool checkDuplicate(List<MyProgram> list, MyProgram pro)
        {
            for (int i = 0; i < list.Count(); i++)
            {
                if (list[i].TapeCode.Equals(pro.TapeCode))
                    return true;
            }
            return false;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            
            using (FileStream stream = new FileStream(@"D:\Product-Quantity-Standard.xlsx", FileMode.Create, FileAccess.Write))
            {
                IWorkbook wb = new XSSFWorkbook();
                // tab name
                ISheet sheet = wb.CreateSheet("Bao cao SL ban ra hang ngay");
                // header
                IRow row = sheet.CreateRow(0);
                ICell cell = row.CreateCell(0);
                cell.SetCellValue("BÁO CÁO SẢN PHẨM HÀNG NGÀY CÔNG TY ATZ");
                NPOI.SS.Util.CellRangeAddress cra = new NPOI.SS.Util.CellRangeAddress(0, 0, 0, 2);
                sheet.AddMergedRegion(cra);
                // column header 
                IRow row3 = sheet.CreateRow(2);
                ICell cell0 = row3.CreateCell(0);
                cell0.SetCellValue("STT");
                ICell cell1 = row3.CreateCell(1);
                cell1.SetCellValue("MÃ CHƯƠNG TRÌNH");
                ICell cell2 = row3.CreateCell(2);
                cell2.SetCellValue("CHƯƠNG TRÌNH");
                ICell cell3 = row3.CreateCell(3);
                cell3.SetCellValue("Duration");
                ICell cell4 = row3.CreateCell(4);
                cell4.SetCellValue("CATEGORY");
                ICell cell5 = row3.CreateCell(5);
                cell5.SetCellValue("MÃ SẢN PHẨM");
                ICell cell6 = row3.CreateCell(6);
                cell6.SetCellValue("SẢN PHẨM");
                ICell cell7 = row3.CreateCell(7);
                cell7.SetCellValue("GIÁ SẢN PHẨM");               
                // add Program Code
                int i = 3;
                foreach (var item in programList)
                {
                    IRow row_temp = sheet.CreateRow(i);
                    ICell cell_temp0 = row_temp.CreateCell(0);
                    cell_temp0.SetCellValue(i-2);
                    ICell cell_temp1 = row_temp.CreateCell(1);
                    cell_temp1.SetCellValue(item.TapeCode);
                    ICell cell_temp2 = row_temp.CreateCell(2);
                    cell_temp2.SetCellValue(item.Name);
                    ICell cell_temp3 = row_temp.CreateCell(3);
                    DateTime time = DateTime.Today;
                    time = time.AddMinutes(item.Duration.Minute).AddSeconds(item.Duration.Second);
                    cell_temp3.SetCellValue(time);
                    ICellStyle style = wb.CreateCellStyle();
                    cell_temp3.CellStyle = style;
                    IDataFormat dataFormatCustom = wb.CreateDataFormat();
                    cell_temp3.CellStyle.DataFormat = dataFormatCustom.GetFormat("HH:mm:ss");
                    i++;
                }

                sheet.AutoSizeColumn(0);
                sheet.AutoSizeColumn(1);
                sheet.AutoSizeColumn(2);
                sheet.AutoSizeColumn(3);
                sheet.AutoSizeColumn(4);
                sheet.AutoSizeColumn(5);
                sheet.AutoSizeColumn(6);
                sheet.AutoSizeColumn(7);

                wb.Write(stream);
            }
        }
    }
}
