using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExportTool.Services.Interface
{
    public interface ICategoryService : IEntityService<Category>
    {
        Category GetById(int Id);
        Category GetByName(string Name);
        void CreateList(List<Category> entity);
    }
}
