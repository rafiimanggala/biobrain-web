using System.Collections.Generic;
using DAL.Models.Interfaces;

namespace DAL.Repositorys.Interfaces
{
    public interface IDoneTopicsRepository : IBaseRepository<IDoneTopicModel>
    {
        List<IDoneTopicModel> GetAll();
        bool IsTopicDone(int topicID);
    }
}