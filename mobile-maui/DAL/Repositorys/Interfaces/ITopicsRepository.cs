using System.Collections.Generic;
using DAL.Models.Interfaces;

namespace DAL.Repositorys.Interfaces
{
    public interface ITopicsRepository : IBaseRepository<ITopicModel>
    {
        List<ITopicModel> GetAll();

        List<ITopicModel> GetForArea(int areaID);

        ITopicModel GetByID(int topicID);

        string GetNameByID(int topicID);

        int CountForArea(int areaID);

        int CountDoneForArea(int areaID);

        ITopicModel GetByAreaAndOrder(int areaID, int order);
    }
}