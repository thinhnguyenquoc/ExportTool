using AZReport.Services;
using AZReport.Services.IServices;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AZReport
{
    public partial class Form1 : Form
    {
        IWorkbook _iWorkbook;
        IProgramService _iProgramService;
        IScheduleService _iScheduleService;
        ISaleService _iSaleService;
        IReportService _iReportService;
        DateTime start;
        DateTime end;
        DateTime reportStart;
        DateTime reportEnd;
        IFormatProvider culture;

        public Form1(IProgramService iProgramService, IScheduleService iScheduleService, ISaleService iSaleService, IReportService iReportService)
        {
            InitializeComponent();
            _iProgramService = iProgramService;
            _iScheduleService = iScheduleService;
            _iSaleService = iSaleService;
            _iReportService = iReportService;
            var all = _iProgramService.GetAll().ToList();
            start = DateTime.Today;
            end = DateTime.Today;
            reportStart = DateTime.Today;
            reportEnd = DateTime.Today;
            culture = new System.Globalization.CultureInfo("fr-FR", true);
        }

        private void ReadProgram(IWorkbook _iWordbook)
        {
            for (int i = 0; i < _iWorkbook.NumberOfSheets; i++)
            {
                if (_iWorkbook.GetSheetName(i).Equals("MABANG"))
                {
                    ISheet proSheet = _iWorkbook.GetSheetAt(i);
                    List<Program> listProgram = new List<Program>();
                    for (int j = 4; j <= proSheet.LastRowNum; j++)
                    {
                        var row = proSheet.GetRow(j);
                        Program program = new Program();
                        if (row.GetCell(0) == null || row.GetCell(0).NumericCellValue == 0)
                            break;
                        program.Name = row.GetCell(1).StringCellValue.ToString();
                        program.Duration = row.GetCell(2).DateCellValue.ToString();
                        program.Code = row.GetCell(3).StringCellValue.ToString();
                        program.Category = row.GetCell(4).StringCellValue.ToString();
                        program.Price = row.GetCell(5).NumericCellValue.ToString();
                        program.Note = row.GetCell(6).StringCellValue.ToString();
                        _iProgramService.CheckAndUpdate(program);
                    }
                    _iProgramService.Save();
                }
            }
        }

        private void ReadSchedule(IWorkbook _iWordbook)
        {
            for (int i = 0; i < _iWorkbook.NumberOfSheets; i++)
            {
                ISheet sheet = _iWorkbook.GetSheetAt(i);
                if (CheckSchedule(sheet))
                {
                    var listTime = GetTimeList(sheet);
                    var startDay = listTime.FirstOrDefault();
                    var lastDay = listTime.LastOrDefault();
                    var startPoint = StartPoint(sheet);
                    while (startDay <= lastDay)
                    {
                        for (int j = startPoint.First() + 1; j <= sheet.LastRowNum; j++)
                        {
                            var row = sheet.GetRow(j);
                            Schedule schedule = new Schedule();
                            if (row.GetCell(startPoint.Last()) == null || row.GetCell(startPoint.Last()).NumericCellValue == 0)
                                break;
                            schedule.Code = row.GetCell(startPoint.Last() + 4).StringCellValue.ToString();
                            var mytime = row.GetCell(startPoint.Last() + 1).DateCellValue;
                            schedule.Date = new DateTime(startDay.Year, startDay.Month, startDay.Day, mytime.Hour, mytime.Minute, mytime.Second);
                            _iScheduleService.CheckAndCreate(schedule);
                        }
                        startDay = startDay.AddDays(1);
                    }
                    _iScheduleService.Save();
                }
            }
        }

        private bool CheckSchedule(ISheet sheet)
        {            
            var IsSheet = false;
            for (int j = 0; j <= 4; j++)
            {
                var row = sheet.GetRow(j);
                if (row != null)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        if (row.GetCell(k) != null && row.GetCell(k).CellType == CellType.String)
                        {
                            var mystring = row.GetCell(k).StringCellValue.ToString();
                            if (!string.IsNullOrWhiteSpace(mystring))
                            {
                                if (mystring.Contains("LỊCH PHÁT SÓNG QUẢNG CÁO"))
                                {
                                    IsSheet = true;
                                    break;
                                }
                            }
                        }

                    }
                    if (IsSheet == true)
                        break;
                }
            }
            return IsSheet;
        }

        private List<DateTime> GetTimeList(ISheet sheet)
        {
            var datestring = "";
            for (int j = 0; j <= 4; j++)
            {
                var row = sheet.GetRow(j);
                if (row != null)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        if (row.GetCell(k) != null && row.GetCell(k).CellType == CellType.String)
                        {
                            var mystring = row.GetCell(k).StringCellValue.ToString();
                            if (!string.IsNullOrWhiteSpace(mystring))
                            {
                                if (mystring.Contains("Ngày phát sóng"))
                                {
                                    datestring = mystring;
                                    break;
                                }
                            }
                        }
                        if (!string.IsNullOrEmpty(datestring))
                        {
                            break;
                        }
                    }

                }
                
            }
            var date = datestring.Split(new string[] { "Ngày phát sóng:" }, StringSplitOptions.None).LastOrDefault();
            List<DateTime> TimeList = new List<DateTime>();
            DateTime startday = new DateTime(1990,1,1);
            DateTime lastday = new DateTime(1990, 1, 1);
            var format = "d/M/yyyy";
            var provider = new CultureInfo("fr-FR");
            if (date.Contains('-'))
            {
                var datelist = date.Split('-');                
                startday = DateTime.ParseExact(datelist.FirstOrDefault().Trim(), format, provider);
                lastday = DateTime.ParseExact(datelist.LastOrDefault().Trim(), format, provider);               
            }
            else if ((date.Contains(',')||date.Contains(';'))&&!date.Contains('&'))
            {
                var datelist = date.Split(new char[]{';',','});
                lastday = DateTime.ParseExact(datelist.LastOrDefault().Trim(), format, provider);
                startday = new DateTime(lastday.Year, lastday.Month, Convert.ToInt32(datelist.FirstOrDefault().Trim()));
            }
            else
            {
                var datelist = date.Split(new char[] { ';', ',','&' });
                if (datelist.Count() == 1)
                {
                    lastday = DateTime.ParseExact(datelist.LastOrDefault().Trim(), format, provider);
                    startday = DateTime.ParseExact(datelist.FirstOrDefault().Trim(), format, provider);
                }
                else if (datelist.Count() > 2)
                {
                    lastday = DateTime.ParseExact(datelist.LastOrDefault().Trim(), format, provider);
                    if (lastday.Month != 1)
                    {
                        startday = new DateTime(lastday.Year, lastday.Month - 1, Convert.ToInt32(datelist.FirstOrDefault().Trim()));
                    }
                    else
                    {
                        startday = new DateTime(lastday.Year-1, 12, Convert.ToInt32(datelist.FirstOrDefault().Trim()));
                    }
                }
                else
                {

                
                }
            }
            if (startday.Year != 1990)
            {
                while (startday <= lastday)
                {
                    TimeList.Add(startday);
                    startday = startday.AddDays(1);
                }
            }       
            return TimeList;
        }

        private List<int> StartPoint(ISheet sheet)
        {
            List<int> result = new List<int>();
            for (int j = 0; j <= 7; j++)
            {
                var row = sheet.GetRow(j);
                if (row != null)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        if (row.GetCell(k) != null && row.GetCell(k).CellType == CellType.String)
                        {
                            var mystring = row.GetCell(k).StringCellValue.ToString();
                            if (!string.IsNullOrWhiteSpace(mystring))
                            {
                                if (mystring.Contains("STT"))
                                {
                                    result.Add(j);
                                    result.Add(k);
                                    break;
                                }
                            }
                        }
                        if (result.Count() != 0)
                        {
                            break;
                        }
                    }

                }

            }
            return result;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Excel Files|*.xls;*.xlsx";
            DialogResult result = openFileDialog1.ShowDialog(); // Show the dialog.
            if (result == DialogResult.OK) // Test result.
            {
                string file = openFileDialog1.FileName;
                textBox1.Text = file;               
                using (FileStream pr = new FileStream(file, FileMode.Open, FileAccess.Read))
                {
                    if (file.Contains(".xlsx"))
                    {
                        _iWorkbook = new XSSFWorkbook(pr);
                    }
                    else if (file.Contains(".xls"))
                    {
                        _iWorkbook = new HSSFWorkbook(pr);
                    }
                    else
                    {
                        return;
                    }
                    ReadProgram(_iWorkbook);
                }                
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            openFileDialog2.Filter = "Excel Files|*.xls;*.xlsx";
            DialogResult result = openFileDialog2.ShowDialog(); // Show the dialog.
            if (result == DialogResult.OK) // Test result.
            {
                string file = openFileDialog2.FileName;
                textBox2.Text = file;
                using (FileStream pr = new FileStream(file, FileMode.Open, FileAccess.Read))
                {
                    if (file.Contains(".xlsx"))
                    {
                        _iWorkbook = new XSSFWorkbook(pr);
                    }
                    else if (file.Contains(".xls"))
                    {
                        _iWorkbook = new HSSFWorkbook(pr);
                    }
                    else
                    {
                        return;
                    }
                    ReadSchedule(_iWorkbook);
                }
            }
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            start = dateTimePicker1.Value;
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            end = dateTimePicker2.Value;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "Excel|*.xls;*.xlsx";
            saveFileDialog1.FileName = "Quantity";
            saveFileDialog1.DefaultExt = "xlsx";
            saveFileDialog1.ShowDialog();    
        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            var result = _iReportService.GetProductivity(new DateTime(start.Year, start.Month, start.Day, 0, 0, 0), new DateTime(end.Year, end.Month, end.Day, 23, 59, 59));
            string name = saveFileDialog1.FileName;
            var wb = new XSSFWorkbook();
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
            cell4.SetCellValue("CATEGORY");
            ICell cell5 = row3.CreateCell(5);
            cell5.SetCellValue("GIÁ SẢN PHẨM");
            ICell cell6 = row3.CreateCell(6);
            cell6.SetCellValue("Ghi chú");
            var tempStart = start;
            var k = 7;
            while (DateTime.Compare(tempStart, end) <= 0)
            {
                ICell cellk = row3.CreateCell(k);
                cellk.SetCellValue(tempStart.ToString("dd/MM/yyyy"));
                tempStart = tempStart.AddDays(1);
                k++;
            }
            // add Program Code
            int i = 3;
            foreach (var item in result)
            {
                var time = Convert.ToDateTime(item.Duration);
                if (time.Minute > 4)
                {
                    IRow row_temp = sheet.CreateRow(i);
                    ICell cell_temp0 = row_temp.CreateCell(0);
                    cell_temp0.SetCellValue(i - 2);
                    ICell cell_temp1 = row_temp.CreateCell(1);
                    cell_temp1.SetCellValue(item.Code);
                    ICell cell_temp2 = row_temp.CreateCell(2);
                    cell_temp2.SetCellValue(item.Name);
                    ICell cell_temp3 = row_temp.CreateCell(3);
                    DateTime time1 = DateTime.Today;
                    time1 = time1.AddMinutes(time.Minute).AddSeconds(time.Second);
                    cell_temp3.SetCellValue(time1);
                    ICellStyle style = wb.CreateCellStyle();
                    cell_temp3.CellStyle = style;
                    IDataFormat dataFormatCustom = wb.CreateDataFormat();
                    cell_temp3.CellStyle.DataFormat = dataFormatCustom.GetFormat("mm:ss");
                    ICell cell_temp4 = row_temp.CreateCell(4);
                    cell_temp4.SetCellValue(item.Category);
                    ICell cell_temp5 = row_temp.CreateCell(5);
                    cell_temp5.SetCellValue(item.Price);
                    ICell cell_temp6 = row_temp.CreateCell(6);
                    cell_temp6.SetCellValue(item.Note);
                    i++;
                }
            }

            for (int l = 0; l < row3.LastCellNum; l++)
            {
                sheet.AutoSizeColumn(l);
            }

            using (FileStream stream = new FileStream(name, FileMode.Create, FileAccess.Write))
            {
                wb.Write(stream);
                stream.Close();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            openFileDialog3.Filter = "Excel Files|*.xls;*.xlsx";
            DialogResult result = openFileDialog3.ShowDialog(); // Show the dialog.
            if (result == DialogResult.OK) // Test result.
            {
                string file = openFileDialog3.FileName;
                textBox3.Text = file;
                using (FileStream pr = new FileStream(file, FileMode.Open, FileAccess.Read))
                {                    
                    if (file.Contains(".xlsx"))
                    {
                        _iWorkbook = new XSSFWorkbook(pr);
                    }
                    else if (file.Contains(".xls"))
                    {
                        _iWorkbook = new HSSFWorkbook(pr);
                    }
                    else
                    {
                        return;
                    }
                    ReadQuantity(_iWorkbook);
                }
            }
        }

        private void ReadQuantity(IWorkbook _iWordbook)
        {
            for (int i = 0; i < _iWorkbook.NumberOfSheets; i++)
            {
                if (_iWorkbook.GetSheetName(i).Equals("Bao cao SL ban ra hang ngay"))
                {
                    ISheet proSheet = _iWorkbook.GetSheetAt(i);
                    List<Program> listProgram = new List<Program>();
                    for (int j = 3; j <= proSheet.LastRowNum; j++)
                    {
                        var row = proSheet.GetRow(j);
                        Sale sale = new Sale();
                        if (row.GetCell(0) == null || row.GetCell(0).NumericCellValue == 0)
                            break;
                        sale.Code = row.GetCell(1).StringCellValue.ToString();
                        for (int k = 7; k < proSheet.GetRow(2).LastCellNum; k++)
                        {
                            sale.Quantity = row.GetCell(k) != null ? row.GetCell(k).NumericCellValue.ToString() : "0";
                            sale.Date = DateTime.Parse(proSheet.GetRow(2).GetCell(k).StringCellValue.ToString(), culture, System.Globalization.DateTimeStyles.AssumeLocal);
                            _iSaleService.CheckAndUpdate(sale);
                            _iSaleService.Save();
                        }                        
                    }
                    
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            saveFileDialog2.Filter = "Excel|*.xls;*.xlsx";
            saveFileDialog2.FileName = "AZ_Efficiency_" + reportStart.ToString("dd_MM_yyyy");
            saveFileDialog2.DefaultExt = "xlsx";
            saveFileDialog2.ShowDialog();
        }

        private void saveFileDialog2_FileOk(object sender, CancelEventArgs e)
        {
            string name = saveFileDialog2.FileName;
            var wb = new XSSFWorkbook();
            using (FileStream stream = new FileStream(@"E:\AZReport\TemplateEfficiency.xlsx", FileMode.Open, FileAccess.Read))
            {
                wb = new XSSFWorkbook(stream);
                stream.Close();
            }
            ISheet sheet = wb.GetSheetAt(1);
            createItemList(sheet, reportStart, reportEnd, wb);
            using (FileStream stream = new FileStream(name, FileMode.Create, FileAccess.Write))
            {
                wb.Write(stream);
                stream.Close();
            }
        }

        private void createItemList(ISheet sheet, DateTime startDay, DateTime endDay, IWorkbook wb)
        {
            ICell topCel3 = sheet.GetRow(0).GetCell(3);
            topCel3.SetCellValue(startDay.ToString("dd/MM/yyyy") + " - " + endDay.ToString("dd/MM/yyyy"));

            var startDayTem = new DateTime(startDay.Year, startDay.Month, startDay.Day, 0, 0, 0);            
            IRow dayHeaderList = sheet.GetRow(1);
            for (int m = 0; m < 11; m++)
            {
                ICell dayHeader = dayHeaderList.GetCell(8 + m);
                dayHeader.SetCellValue(startDayTem.ToString("ddd"));
                startDayTem = startDayTem.AddDays(1);
            }
            var result = _iReportService.GetProductivity(new DateTime(startDay.Year, startDay.Month, startDay.Day, 0, 0, 0), new DateTime(endDay.Year, endDay.Month, endDay.Day, 23, 59, 59));
            var quantityList = _iReportService.GetQuantity(new DateTime(startDay.Year, startDay.Month, startDay.Day, 0, 0, 0), new DateTime(endDay.Year, endDay.Month, endDay.Day, 23, 59, 59));
            var freqList = _iReportService.GetFreq(new DateTime(startDay.Year, startDay.Month, startDay.Day, 0, 0, 0), new DateTime(endDay.Year, endDay.Month, endDay.Day, 23, 59, 59));
            IRow row2 = sheet.GetRow(1);
            int rowIndex = 2;
            foreach (var item in result)
            {
                DateTime time = Convert.ToDateTime(item.Duration);
                if (time.Minute > 4)
                {
                    IRow rowEff = sheet.GetRow(rowIndex);
                    ICell eff_cell0 = rowEff.GetCell(0);
                    eff_cell0.SetCellValue(rowIndex - 1);
                    ICell eff_cell1 = rowEff.GetCell(1);
                    eff_cell1.SetCellValue(item.Code);
                    ICell eff_cell2 = rowEff.GetCell(2);
                    eff_cell2.SetCellValue(item.Name);
                    ICell eff_cell4 = rowEff.GetCell(4);

                    eff_cell4.SetCellValue(time.Minute+ time.Second/60.0);
                    ICell eff_cell5 = rowEff.GetCell(5);
                    eff_cell5.SetCellValue(item.Category);
                    ICell eff_cell6 = rowEff.GetCell(6);
                    eff_cell6.SetCellValue(item.Price);                   

                    var tempDate = startDay;
                    int l = 1;
                    while (tempDate <= endDay)
                    {
                        ICell eff_cellweek = rowEff.GetCell(7 + l);
                        eff_cellweek.SetCellValue(freqList.Where(x=>x.Code == item.Code && x.Date == new DateTime(startDay.Year, startDay.Month, startDay.Day, 0, 0, 0)).FirstOrDefault().Freq);
                        tempDate = tempDate.AddDays(1);
                        l++;
                    }

                    int q = (int)quantityList.Where(x => x.Code == item.Code).FirstOrDefault().Quantity;
                    ICell eff_cell10 = rowEff.CreateCell(27);
                    eff_cell10.SetCellValue(q);
                    ICell eff_cell11 = rowEff.CreateCell(28);
                    var amount = q * Convert.ToInt32(item.Price);
                    eff_cell11.SetCellValue(amount);
                    ICell eff_cell12 = rowEff.CreateCell(29);
                    var totalTime = freqList.Where(x => x.Code == item.Code).Sum(x => x.Freq) * (time.Minute + time.Second / 60.0);
                    eff_cell12.SetCellValue(totalTime);

                    ICell eff_cell7 = rowEff.GetCell(7);
                    eff_cell7.SetCellValue(amount/totalTime);

                    ICell eff_cell13 = rowEff.CreateCell(3);
                    eff_cell13.SetCellValue(calculateGroup(amount / totalTime));
                    rowIndex++;
                }              
            }

            //for (int l = 0; l < row2.LastCellNum; l++)
            //{
            //    sheet.AutoSizeColumn(l);
            //}
        }

        private void dateTimePicker3_ValueChanged(object sender, EventArgs e)
        {
            reportStart = dateTimePicker3.Value;
        }

        private void dateTimePicker4_ValueChanged(object sender, EventArgs e)
        {
            reportEnd = dateTimePicker4.Value;
        }

        private string calculateGroup(double eff)
        {
            if (eff >= 200000)
                return "A";
            else if (eff >= 100000 && eff < 200000)
                return "B";
            else if (eff != 0)
                return "C";
            else
                return "";
        }
    }

    
}
