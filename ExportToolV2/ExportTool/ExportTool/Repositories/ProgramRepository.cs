using ExportTool.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExportTool.Repositories
{
    public class ProgramRepository : GenericRepository<Program>, IProgramRepository
    {
        public ProgramRepository(ExportToolEntities context)
            : base(context)
        {

        }

        //public override IEnumerable<Program> GetAll()
        //{
        //    return _entities.Set<Program>().AsEnumerable();
        //}

        //public Program GetById(int id)
        //{
        //    return _dbset.Where(x => x.Id == id).FirstOrDefault();
        //}

        //public Program GetByName(string Name)
        //{
        //    return _dbset.Where(x => x.Name.ToLower() == Name.ToLower()).FirstOrDefault();
        //}

        //public Program GetByCode(string Code)
        //{
        //    return _dbset.Where(x => x.TapeCode == Code).FirstOrDefault();
        //}
    }
}
