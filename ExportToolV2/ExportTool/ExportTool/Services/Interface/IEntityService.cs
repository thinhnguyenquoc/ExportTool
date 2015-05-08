using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExportTool.Services.Interface
{
    public interface IEntityService<T> where T : class
    {
        void Create(T entity);
        void Delete(T entity);
        IEnumerable<T> GetAll();
        void Update(T entity);
    }
}
