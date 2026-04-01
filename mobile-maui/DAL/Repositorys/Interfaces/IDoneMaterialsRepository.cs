using System.Collections.Generic;
using DAL.Models.Interfaces;

namespace DAL.Repositorys.Interfaces
{
    public interface IDoneMaterialsRepository : IBaseRepository<IDoneMaterialModel>
    {
        List<IDoneMaterialModel> GetAll();

        IDoneMaterialModel GetByID(int doneMaterialID);

        bool IsMaterialDone(int materilaID);

        List<IDoneMaterialModel> GetEntriesForMaterial(int materialID);

        void Clean();
    }
}