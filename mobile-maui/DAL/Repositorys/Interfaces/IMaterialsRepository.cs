using System.Collections.Generic;
using DAL.Models.Interfaces;

namespace DAL.Repositorys.Interfaces
{
    public interface IMaterialsRepository : IBaseRepository<IMaterialModel>
    {
        List<IMaterialModel> GetAll();

        List<IMaterialModel> GetForTopic(int topicID);

        IMaterialModel GetByID(int materialID);

        IMaterialModel GetForLevelAndTopic(int levelID, int topicID);

        int CountDoneForTopic(int topicID);

        int CountAllForTopic(int topicID);
    }
}