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
        XSSFWorkbook quantity;
        List<MyProgram> programList;
        public Form1()
        {
            InitializeComponent();
            button2.Enabled = false;
            button3.Enabled = false;
            button6.Enabled = false;
            button7.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog1.ShowDialog(); // Show the dialog.
            if (result == DialogResult.OK) // Test result.
            {
                string file = openFileDialog1.FileName;
                textBox1.Text = file;
                button2.Enabled = true;
            }
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
                        program.Frequency = 1;

                        if (!checkDuplicate(programList, program))
                        {
                            programList.Add(program);
                        }
                    }
                }
            }
            programList = programList.OrderBy(x => x.Name).ToList();
            int index = 1;
            dataGridView1.DataSource = programList.Select(x => new { SNO = index++, x.TapeCode, Duration = x.Duration.Minute.ToString()+":"+x.Duration.Second.ToString(), x.Name, x.Frequency }).ToList();
            dataGridView1.AutoResizeColumns();
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.AllowUserToAddRows = false;
            button3.Enabled = true;
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
                cell3.SetCellValue("DURATION");
                ICell cell4 = row3.CreateCell(4);
                cell4.SetCellValue("FREQUENCY");
                ICell cell5 = row3.CreateCell(5);
                cell5.SetCellValue("CATEGORY");
                //ICell cell6 = row3.CreateCell(6);
                //cell6.SetCellValue("MÃ SẢN PHẨM");
                //ICell cell7 = row3.CreateCell(7);
                //cell7.SetCellValue("SẢN PHẨM");
                ICell cell8 = row3.CreateCell(6);
                cell8.SetCellValue("GIÁ SẢN PHẨM");

                ISheet scheduleSheet = schedule.GetSheetAt(0);
                var row1 = scheduleSheet.GetRow(1);
                var year = row1.GetCell(0).StringCellValue.Split('/').LastOrDefault();
                var re = parseDate(scheduleSheet.SheetName, year);
                int k = 7;
                DateTime startTime = re[0];
                DateTime endTime = re[1];
                while (DateTime.Compare(startTime, endTime) <= 0)
                {
                    ICell cell9 = row3.CreateCell(k);
                    cell9.SetCellValue(startTime.ToString("MM/dd/yyyy"));
                    startTime = startTime.AddDays(1);
                    k++;
                }
                // add Program Code
                int i = 3;
                foreach (var item in programList)
                {
                    if (item.Duration.Minute > 3)
                    {
                        IRow row_temp = sheet.CreateRow(i);
                        ICell cell_temp0 = row_temp.CreateCell(0);
                        cell_temp0.SetCellValue(i - 2);
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
                        ICell cell_temp4 = row_temp.CreateCell(4);
                        cell_temp4.SetCellValue(item.Frequency);
                        i++;
                    }
                }

                for (int l = 0; l < row3.LastCellNum; l++)
                {
                    sheet.AutoSizeColumn(l);
                }
                wb.Write(stream);
            }
        }

        #region support function
        private List<DateTime> parseDate(string date, string year)
        {
            List<DateTime> list = new List<DateTime>();
            if (!date.Contains("&"))
            {
                string startDay = date.Split('-')[0];
                string endDay = date.Split('-')[1].Split('.')[0];
                string month = date.Split('-')[1].Split('.')[1];

                DateTime start = new DateTime(Convert.ToInt32(year), Convert.ToInt32(month), Convert.ToInt32(startDay));
                DateTime end = new DateTime(Convert.ToInt32(year), Convert.ToInt32(month), Convert.ToInt32(endDay));
                list.Add(start);
                list.Add(end);

            }
            else
            {
                string startDay = date.Split('&')[0].Split('-')[0];
                string endDay = date.Split('&')[1].Split('.')[0];
                string startmonth = date.Split('&')[0].Split('-')[1].Split('.')[1];
                string endmonth = date.Split('&')[1].Split('.')[1];

                DateTime start = new DateTime(Convert.ToInt32(year), Convert.ToInt32(startmonth), Convert.ToInt32(startDay));
                DateTime end = new DateTime(Convert.ToInt32(year), Convert.ToInt32(endmonth), Convert.ToInt32(endDay));
                list.Add(start);
                list.Add(end);
            }
            return list;
        }

        private bool checkDuplicate(List<MyProgram> list, MyProgram pro)
        {
            for (int i = 0; i < list.Count(); i++)
            {
                if (list[i].TapeCode.Equals(pro.TapeCode))
                {
                    list[i].Frequency += 1;
                    return true;
                }
            }
            return false;
        }

        private int countDay(XSSFWorkbook quantity)
        {
            ISheet sheet = quantity.GetSheetAt(0);
            int totalRow = sheet.LastRowNum;
            int totalCol = sheet.GetRow(2).LastCellNum;
            int result = 0;

            for (int j = 7; j < totalCol; j++){
                for (int i = 3; i < totalRow; i++)
                {
                    if (sheet.GetRow(i).GetCell(j)!= null && sheet.GetRow(i).GetCell(j).NumericCellValue != 0)
                    {
                        result++;
                        break;
                    }
                }
            }
            return result;
        }
        #endregion 

        private void button4_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog2.ShowDialog(); // Show the dialog.
            if (result == DialogResult.OK) // Test result.
            {
                string file = openFileDialog2.FileName;
                textBox2.Text = file;
                button6.Enabled = true;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog3.ShowDialog(); // Show the dialog.
            if (result == DialogResult.OK) // Test result.
            {
                string file = openFileDialog3.FileName;
                textBox3.Text = file;
                button6.Enabled = true;
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            using (FileStream sch = new FileStream(textBox2.Text, FileMode.Open, FileAccess.Read))
            {
                schedule = new XSSFWorkbook(sch);
            }
            using (FileStream qtt = new FileStream(textBox3.Text, FileMode.Open, FileAccess.Read))
            {
                quantity = new XSSFWorkbook(qtt);
            }
            int totalDay = countDay(quantity);
        }
    }
}
