using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExportTool.Repositories.Interface
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        //Category GetById(long id);
        //Category GetByName(string name);
    }
}
