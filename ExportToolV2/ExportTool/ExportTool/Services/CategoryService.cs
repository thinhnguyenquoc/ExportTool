using ExportTool.Repositories.Interface;
using ExportTool.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExportTool.Services
{
    public class CategoryService : EntityService<Category>, ICategoryService
    {
        IUnitOfWork _unitOfWork;
        ICategoryRepository _categoryRepository;

        public CategoryService(IUnitOfWork unitOfWork, ICategoryRepository categoryRepository)
            : base(unitOfWork, categoryRepository)
        {
            _unitOfWork = unitOfWork;
            _categoryRepository = categoryRepository;
        }


        //public Category GetById(int Id)
        //{
        //    return _categoryRepository.GetById(Id);
        //}

        //public Category GetByName(string Name)
        //{
        //    return _categoryRepository.GetByName(Name);
        //}

        //public void CreateList(List<Category> entity)
        //{
        //    if (entity.Count() == 0)
        //    {
        //        throw new ArgumentNullException("entity");
        //    }
        //    foreach (var item in entity)
        //    {
        //        _categoryRepository.Add(item);
        //    }
        //    _unitOfWork.Commit();
        //}
    }
}
