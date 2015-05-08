using ExportTool.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExportTool.Repositories
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(ExportToolEntities context)
            : base(context)
        {

        }

        //public override IEnumerable<Category> GetAll()
        //{
        //    return _entities.Set<Category>().AsEnumerable();
        //}

        //public Category GetById(long id)
        //{
        //    return _dbset.Where(x => x.Id == id).FirstOrDefault();
        //}

        //public Category GetByName(string Name)
        //{
        //    return _dbset.Where(x => x.Name.ToLower() == Name.ToLower()).FirstOrDefault();
        //}
    }
}
