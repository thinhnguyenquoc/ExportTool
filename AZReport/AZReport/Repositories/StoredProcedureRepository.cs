using AZReport.Repositories.Interface;
using AZReport.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace AZReport.Repositories
{
    public class StoredProcedureRepository : GenericRepository<AZModel>, IStoredProcedureRepository
    {
        private AZModelContainer _context;
        public StoredProcedureRepository(AZModelContainer context)
            : base(context)
        {
            _context = context;
        }

        public List<ProductivityViewModel> GetProductivity(DateTime start, DateTime end)
        {
            var startTime = new SqlParameter
            {
                ParameterName = "param1",
                Value = start.ToString()
            };
            var endTime = new SqlParameter
            {
                ParameterName = "param2",
                Value = end.ToString()
            };
            var courseList = _context.Database.SqlQuery<ProductivityViewModel>("GetProductivity @param1, @param2 ", startTime, endTime).ToList<ProductivityViewModel>();

            return courseList;
        }

        public List<ReportViewModel> GetQuantity(DateTime start, DateTime end)
        {
            var startTime = new SqlParameter
            {
                ParameterName = "param1",
                Value = start.ToString()
            };
            var endTime = new SqlParameter
            {
                ParameterName = "param2",
                Value = end.ToString()
            };
            var courseList = _context.Database.SqlQuery<ReportViewModel>("GetTotalQuantity @param1, @param2 ", startTime, endTime).ToList<ReportViewModel>();

            return courseList;
        }

        public List<ReportViewModel> GetFreq(DateTime start, DateTime end)
        {
            var startTime = new SqlParameter
            {
                ParameterName = "param1",
                Value = start.ToString()
            };
            var endTime = new SqlParameter
            {
                ParameterName = "param2",
                Value = end.ToString()
            };
            var courseList = _context.Database.SqlQuery<ReportViewModel>("GetTotalFrequency @param1, @param2 ", startTime, endTime).ToList<ReportViewModel>();

            return courseList;
        }
              
    }
}
