using System.Collections.Generic;
using DAL.Models.Interfaces;

namespace DAL.Repositorys.Interfaces
{
    public interface IQuestionsRepository : IBaseRepository<IQuestionModel>
    {
        List<IQuestionModel> GetAll();

        IQuestionModel GetByID(int questionID);

        List<IQuestionModel> GetByMaterial(int materialID);

        List<IQuestionModel> GetByType(int type);

        ITopicModel GetTopic(int materialID);
    }
}