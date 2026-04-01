using System.Collections.Generic;
using DAL.Models.Interfaces;

namespace DAL.Repositorys.Interfaces
{
    public interface IDoneQuestionsRepository : IBaseRepository<IDoneQuestionModel>
    {
        List<IDoneQuestionModel> GetAll();

        IDoneQuestionModel GetByID(int doneQuestionID);

        bool IsQuestionDone(int questionID);

        void Add(IDoneQuestionModel model);

        IDoneQuestionModel GetByQuestionID(int doneQuestionID);

        void DeleteByQuestionId(int questionID);
    }
}