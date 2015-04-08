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
        List<MyQuantity> quantityList;
        public Form1()
        {
            InitializeComponent();
            textBox1.Text = @"C:\Users\thinhnguyen.DICENTRAL\Desktop\Schedule-Standard.xlsx";
            textBox2.Text = @"C:\Users\thinhnguyen.DICENTRAL\Desktop\Schedule-Standard.xlsx";
            textBox3.Text = @"C:\Users\thinhnguyen.DICENTRAL\Desktop\Product-Quantity-Standard.xlsx";
            //textBox1.Text = @"C:\Users\thinh\Desktop\doc\Schedule-Standard (1).xlsx";
            //textBox2.Text = @"C:\Users\thinh\Desktop\doc\Schedule-Standard (1).xlsx";
            //textBox3.Text = @"C:\Users\thinh\Desktop\doc\Product-Quantity-Standard.xlsx";
            
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
            int totalRow = sheet.LastRowNum + 1;
            int totalCol = sheet.GetRow(2).LastCellNum + 1;
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

        private List<MyProgram> getProgram(XSSFWorkbook schedule, int hour)
        {
            ISheet sheet = schedule.GetSheetAt(0);
            List<MyProgram> programList = new List<MyProgram>();
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
                    if (row.GetCell(1).DateCellValue.Hour == hour && row.GetCell(3).DateCellValue.Minute > 3)
                    {
                        program.TapeCode = row.GetCell(4).StringCellValue.ToString();
                        program.Name = row.GetCell(2).StringCellValue.ToString();
                        program.Duration = row.GetCell(3).DateCellValue;                       
                        programList.Add(program);                        
                    }
                }
            }
            return programList;
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
            quantityList = new List<MyQuantity>();
            ISheet sheet = quantity.GetSheetAt(0);
            int totalDay = countDay(quantity);
            int totalRow = sheet.LastRowNum + 1;
            for (int i = 3; i < totalRow; i++)
            {
                MyQuantity mq = new MyQuantity();
                mq.TapeCode = sheet.GetRow(i).GetCell(1).StringCellValue;
                mq.Name = sheet.GetRow(i).GetCell(2).StringCellValue;
                mq.Duration = sheet.GetRow(i).GetCell(3).DateCellValue;
                mq.Frequency = (int) sheet.GetRow(i).GetCell(4).NumericCellValue;
                mq.Category = sheet.GetRow(i).GetCell(5).StringCellValue;
                mq.Price = (int)sheet.GetRow(i).GetCell(6).NumericCellValue;
                for (int j = 7; j < 7 + totalDay; j++)
                {                
                    if (sheet.GetRow(i).GetCell(j) != null && sheet.GetRow(i).GetCell(j).NumericCellValue != 0)
                    {
                        mq.Quantity += (int)sheet.GetRow(i).GetCell(j).NumericCellValue;
                    }
                }
                if (mq.Quantity != null && mq.Quantity > 0)
                {
                    mq.TotalMinutes = (mq.Duration.Minute + mq.Duration.Second / 60) * totalDay;
                    mq.Amount = mq.Quantity * mq.Price;
                    mq.Efficiency = (int)(mq.Amount / mq.TotalMinutes);
                }
                quantityList.Add(mq);
            }
            quantityList = quantityList.OrderBy(x => x.Efficiency).ToList();
            int index = 1;
            dataGridView2.DataSource = quantityList.Select(x => new { SNO = index++, x.TapeCode, Duration = x.Duration.Minute.ToString() + ":" + x.Duration.Second.ToString(), x.Name, x.Frequency, x.Price, x.Efficiency }).ToList();
            dataGridView2.AutoResizeColumns();
            dataGridView2.AutoGenerateColumns = false;
            dataGridView2.AllowUserToAddRows = false;
            button7.Enabled = true;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            ISheet quantitySheet = quantity.GetSheetAt(0);
            int totalDay = countDay(quantity);
            DateTime startDay = Convert.ToDateTime(quantitySheet.GetRow(2).GetCell(7).StringCellValue);
            DateTime endDay = Convert.ToDateTime(quantitySheet.GetRow(2).GetCell(7 + totalDay).StringCellValue);
            using (FileStream stream = new FileStream(@"D:\Efficiency("+startDay.ToString("dd.MM")+ "_"+endDay.ToString("dd.MM.yyyy")+").xlsx", FileMode.Create, FileAccess.Write))
            {
                IWorkbook wb = new XSSFWorkbook();
                #region
                // tab name
                
                ISheet sheet = wb.CreateSheet("item list");
                ISheet sheetStandard = wb.CreateSheet("standard");
                ISheet sheetCategories = wb.CreateSheet("categories");
                ISheet sheetDuration = wb.CreateSheet("duration");

                // header
                IRow row = sheet.CreateRow(0);
                ICell cell = row.CreateCell(0);
                cell.SetCellValue("VNN Chanel");
                NPOI.SS.Util.CellRangeAddress cra = new NPOI.SS.Util.CellRangeAddress(0, 0, 0, 1);
                sheet.AddMergedRegion(cra);
                // column header 
                IRow row2 = sheet.CreateRow(1);
                ICell cell0 = row2.CreateCell(0);
                cell0.SetCellValue("STT");
                ICell cell1 = row2.CreateCell(1);
                cell1.SetCellValue("Item code");
                ICell cell2 = row2.CreateCell(2);
                cell2.SetCellValue("Product Name (E)");
                ICell cell3 = row2.CreateCell(3);
                cell3.SetCellValue("Group");
                ICell cell4 = row2.CreateCell(4);
                cell4.SetCellValue("Dur");
                ICell cell5 = row2.CreateCell(5);
                cell5.SetCellValue("CATEGORY");
                ICell cell6 = row2.CreateCell(6);
                cell6.SetCellValue("Price");
                ICell cell7 = row2.CreateCell(7);
                cell7.SetCellValue("EFF");  
                var tempDate = startDay;
                for (int p = 1; p < 12; p++)
                {
                    var startWeek =  tempDate.ToString("ddd");
                    ICell cellWeek = row2.CreateCell(7+p);
                    cellWeek.SetCellValue(startWeek.ToUpper());
                    tempDate = tempDate.AddDays(1);
                }
                int i = row2.LastCellNum;
                ICell cell8 = row2.CreateCell(i++);
                cell8.SetCellValue("Guide");
                ICell cell9 = row2.CreateCell(i++);
                cell9.SetCellValue("Evaluation");
                ICell cell10 = row2.CreateCell(i++);
                cell10.SetCellValue("Quantity");
                ICell cell11 = row2.CreateCell(i++);
                cell11.SetCellValue("Amount");
                ICell cell12 = row2.CreateCell(i++);
                cell12.SetCellValue("Total minutes"); 


                int rowIndex = 2;
                foreach (var item in quantityList)
                {
                    IRow rowEff = sheet.CreateRow(rowIndex);
                    ICell eff_cell0 = rowEff.CreateCell(0);
                    eff_cell0.SetCellValue(rowIndex -1);
                    ICell eff_cell1 = rowEff.CreateCell(1);
                    eff_cell1.SetCellValue(0);
                    ICell eff_cell2 = rowEff.CreateCell(2);
                    eff_cell2.SetCellValue(item.Name);
                    ICell eff_cell3 = rowEff.CreateCell(3);
                    eff_cell3.SetCellValue("");
                    ICell eff_cell4 = rowEff.CreateCell(4);
                    DateTime time = DateTime.Today;
                    time = time.AddMinutes(item.Duration.Minute).AddSeconds(item.Duration.Second);
                    eff_cell4.SetCellValue(time);
                    ICellStyle style = wb.CreateCellStyle();
                    eff_cell4.CellStyle = style;
                    IDataFormat dataFormatCustom = wb.CreateDataFormat();
                    eff_cell4.CellStyle.DataFormat = dataFormatCustom.GetFormat("HH:mm:ss");
                    ICell eff_cell5 = rowEff.CreateCell(5);
                    eff_cell5.SetCellValue(item.Category);
                    ICell eff_cell6 = rowEff.CreateCell(6);
                    eff_cell6.SetCellValue(item.Price);
                    ICell eff_cell7 = rowEff.CreateCell(7);
                    eff_cell7.SetCellValue(item.Efficiency);

                    for (int ii = 0; ii < totalDay; ii++)
                    {
                        ICell eff_cellweek = rowEff.CreateCell(7+ii+1);
                        eff_cellweek.SetCellValue(item.Frequency);
                    }

                    ICell eff_cell10 = rowEff.CreateCell(21);
                    eff_cell10.SetCellValue(item.Quantity);
                    ICell eff_cell11 = rowEff.CreateCell(22);
                    eff_cell11.SetCellValue(item.Amount);
                    ICell eff_cell12 = rowEff.CreateCell(23);
                    eff_cell12.SetCellValue(item.TotalMinutes); 
                    rowIndex++;

                }

                for (int l = 0; l < row2.LastCellNum; l++)
                {
                    sheet.AutoSizeColumn(l);
                }

                
                // header
                IRow categoryRow = sheetCategories.CreateRow(0);
                ICell category_cell1 = categoryRow.CreateCell(0);
                category_cell1.SetCellValue("No.");
                ICell category_cell2 = categoryRow.CreateCell(1);
                category_cell2.SetCellValue("Category");
                ICell category_cell3 = categoryRow.CreateCell(2);
                category_cell3.SetCellValue("Color");
               
                // content
                string fileName = "Category.xlsx";
                string path = Path.Combine(Environment.CurrentDirectory,fileName);
                using (FileStream ct = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    var cate = new XSSFWorkbook(ct);
                    ISheet category = cate.GetSheetAt(0);
                    for (int j = 1; j <= category.LastRowNum; j++)
                    {
                        var row_temp = category.GetRow(j);
                        IRow categoryRow_temp = sheetCategories.CreateRow(j);

                        ICell cat_cell1 = categoryRow_temp.CreateCell(0);
                        cat_cell1.SetCellValue(j);

                        ICell cat_cell2 = categoryRow_temp.CreateCell(1);
                        cat_cell2.SetCellValue(row_temp.GetCell(1).StringCellValue.ToString());

                        ICell cat_cell3 = categoryRow_temp.CreateCell(2);  
                     
                        var oldStyle = row_temp.GetCell(2).CellStyle;
                        ICellStyle newStyle = wb.CreateCellStyle();
                        newStyle.FillBackgroundColor = (short)j;
                        newStyle.FillPattern = oldStyle.FillPattern;

                        cat_cell3.CellStyle = newStyle;
                    }
                    for (int l = 0; l < category.GetRow(0).LastCellNum; l++)
                    {
                        category.AutoSizeColumn(l);
                    }
                }
                #endregion
                ISheet sheetTime = wb.CreateSheet("time table");
                #region header
                IRow rowTime0 = sheetTime.CreateRow(0);
                IRow rowTime1 = sheetTime.CreateRow(1);
                IRow rowTime2 = sheetTime.CreateRow(2);
                IRow rowTime3 = sheetTime.CreateRow(3);
                ICell cellr3 = rowTime3.CreateCell(1);
                cellr3.SetCellValue("Hour");
                tempDate = startDay;
                int  colIndex = 2;
                for (int p = 1; p < 12; p++)
                {
                    var startWeek = tempDate.ToString("ddd");
                    ICellStyle styleTimeHeader = wb.CreateCellStyle(); 
                    styleTimeHeader.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    ICell cellWeek1 = null;
                    ICell cellWeek2 = null;
                    ICell cellWeek3 = null;
                    cellWeek1 = rowTime1.CreateCell(colIndex);
                    cellWeek2 = rowTime2.CreateCell(colIndex);
                    cellWeek3 = rowTime3.CreateCell(colIndex);
                    cellWeek3.SetCellValue("Min.");
                    cellWeek1.CellStyle = styleTimeHeader;
                    cellWeek2.CellStyle = styleTimeHeader;
                    cellWeek1.SetCellValue(tempDate.ToString("dd.MM"));
                    cellWeek2.SetCellValue(startWeek.ToUpper());
                    for (int l = 1; l < 6 ; l++)
                    {
                        ++colIndex;
                        cellWeek1 = rowTime1.CreateCell(colIndex);
                        cellWeek2 = rowTime2.CreateCell(colIndex);
                        cellWeek3 = rowTime3.CreateCell(colIndex);
                        if (l == 1)
                        {
                            cellWeek3.SetCellValue("Dur.");
                        }
                        else if (l == 2)
                        {
                            cellWeek3.SetCellValue("Group");
                        }
                        else if (l == 3)
                        {
                            cellWeek3.SetCellValue("Category");
                        }
                        else if (l == 4)
                        {
                            cellWeek3.SetCellValue("");
                        }
                        else
                        {
                            cellWeek3.SetCellValue("Product Name (E)");
                        }
                    }
                    
                    NPOI.SS.Util.CellRangeAddress cra1 = new NPOI.SS.Util.CellRangeAddress(1, 1, colIndex-5, colIndex);
                    sheetTime.AddMergedRegion(cra1);
                    NPOI.SS.Util.CellRangeAddress cra2 = new NPOI.SS.Util.CellRangeAddress(2, 2, colIndex-5, colIndex);
                    sheetTime.AddMergedRegion(cra2);
                   
                    tempDate = tempDate.AddDays(1);
                    ++colIndex;
                }
                #endregion
                
                int indexRowTime = 4;
                totalDay = countDay(quantity);
                //using (FileStream sch = new FileStream(textBox1.Text, FileMode.Open, FileAccess.Read))
                //{
                //    schedule = new XSSFWorkbook(sch);
                //}
                for (int g = 6; g < 24; g++)
                {
                    IRow rowTime4 = sheetTime.CreateRow(indexRowTime);
                    ICell cellWeek4 = rowTime4.CreateCell(1);
                    cellWeek4.SetCellValue(g);
                    IRow rowTime5 = sheetTime.CreateRow(++indexRowTime);
                    IRow rowTime6 = sheetTime.CreateRow(++indexRowTime);
                    IRow rowTime7 = sheetTime.CreateRow(++indexRowTime);

                    var listPro = getProgram(schedule, g);
                    int internalIndexRow = indexRowTime - 3;
                    foreach(var item in listPro){
                       var myRow = sheetTime.GetRow(internalIndexRow);
                       for (var m = 0; m < totalDay; m++)
                       {
                           var myCell = myRow.CreateCell(1 + (m+1)*6);
                           myCell.SetCellValue(item.Name);
                           var myCell2 = myRow.CreateCell(3 + (m) * 6);
                           myCell2.SetCellValue(System.Math.Round(item.Duration.Minute/1.0+item.Duration.Second/60.0, 1, MidpointRounding.AwayFromZero));
                       }                       
                       internalIndexRow++;
                    }
                    while (internalIndexRow <= indexRowTime)
                    {
                        var myRow = sheetTime.GetRow(internalIndexRow);
                        for (var m = 0; m < totalDay; m++)
                        {
                            var myCell = myRow.CreateCell(1 + (m + 1) * 6);
                            myCell.SetCellValue(0);
                            var myCell2 = myRow.CreateCell(3 + (m) * 6);
                            myCell2.SetCellValue("#N/A");
                        }                         
                        internalIndexRow++;
                    }
                    ++indexRowTime;
                }

                wb.Write(stream);
            }
        }
    }
}
