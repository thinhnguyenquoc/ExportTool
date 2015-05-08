using ExportTool.Repositories;
using ExportTool.Repositories.Interface;
using ExportTool.Services;
using ExportTool.Services.Interface;
using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExportTool
{
    public class NinjectBinding : NinjectModule
    {
        public override void Load()
        {
            Bind<ExportToolEntities>().ToSelf().InSingletonScope();
            Bind<IUnitOfWork>().To<UnitOfWork>();
            //Repository
            Bind<ICategoryRepository>().To<CategoryRepository>();
            Bind<IProgramRepository>().To<ProgramRepository>();
            //Service
            Bind<ICategoryService>().To<CategoryService>();
            //Bind<IProgramService>().To<ProgramService>();
        }
    }
}
