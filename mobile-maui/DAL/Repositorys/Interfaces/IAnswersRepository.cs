using System.Collections.Generic;
using DAL.Models.Interfaces;

namespace DAL.Repositorys.Interfaces
{
    public interface IAnswersRepository : IBaseRepository<IAnswerModel>
    {
        List<IAnswerModel> GetAll();

        List<IAnswerModel> GetForQuestion(int questionID);

        IAnswerModel GetByID(int topicID);
    }
}