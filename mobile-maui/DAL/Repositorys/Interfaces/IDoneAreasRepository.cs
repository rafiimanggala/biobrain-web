using System.Collections.Generic;
using DAL.Models.Interfaces;

namespace DAL.Repositorys.Interfaces
{
    public interface IDoneAreasRepository : IBaseRepository<IDoneAreaModel>
    {
        List<IDoneAreaModel> GetAll();
        bool IsAreaDone(int areaID);
    }
}