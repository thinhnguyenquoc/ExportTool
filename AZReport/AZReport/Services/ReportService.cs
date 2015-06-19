using AZReport.Repositories.Interface;
using AZReport.Services.IServices;
using AZReport.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AZReport.Services
{
    public class ReportService: IReportService
    {
        IStoredProcedureRepository _iStoredProcedureRepository;
        public ReportService(IStoredProcedureRepository iStoredProcedureRepository)
        {
            _iStoredProcedureRepository = iStoredProcedureRepository;
        }

        public List<ProductivityViewModel> GetProductivity(DateTime start, DateTime end)
        {
            var result = _iStoredProcedureRepository.GetProductivity(start, end);
            return result;
        }

        public List<ReportViewModel> GetQuantity(DateTime start, DateTime end)
        {
            var result = _iStoredProcedureRepository.GetQuantity(start, end);
            return result;
        }

        public List<ReportViewModel> GetFreq(DateTime start, DateTime end)
        {
            var result = _iStoredProcedureRepository.GetFreq(start, end);
            return result;
        }
    }
}
