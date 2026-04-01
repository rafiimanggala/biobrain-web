using System.Collections.Generic;
using DAL.Models.Interfaces;

namespace DAL.Repositorys.Interfaces
{
    public interface IGlossaryRepository : IBaseRepository<IGlossaryModel>
    {
        List<IGlossaryModel> GetAll();

        IGlossaryModel GetByID(int glossaryID);

        IGlossaryModel GetByRef(string refID);

        IGlossaryModel GetByTerm(string term);

        List<IGlossaryModel> GetByLetter(string letter);

        List<IGlossaryModel> GetByText(string text);
    }
}