using AZReport.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AZReport.Repositories.Interface
{
    public interface IStoredProcedureRepository : IGenericRepository<AZModel>
    {
        List<ProductivityViewModel> GetProductivity(DateTime start, DateTime end);
    }
}
