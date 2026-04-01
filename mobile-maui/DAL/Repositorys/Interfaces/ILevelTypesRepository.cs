using System.Collections.Generic;
using DAL.Models.Interfaces;

namespace DAL.Repositorys.Interfaces
{
    public interface ILevelTypesRepository : IBaseRepository<ILevelTypeModel>
    {
        int FirstLevelID { get; }

        List<ILevelTypeModel> GetAll();

        ILevelTypeModel GetByID(int levelTypeID);

        int GetFirstLevelId();
    }
}