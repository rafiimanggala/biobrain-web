using System.Collections.Generic;
using DAL.Models.Interfaces;

namespace DAL.Repositorys.Interfaces
{
    public interface IAreasRepository : IBaseRepository<IAreaModel>
    {
        List<IAreaModel> GetAll();
        string GetNameByID(int areaID);
        IAreaModel GetByID(int areaID);
    }
}