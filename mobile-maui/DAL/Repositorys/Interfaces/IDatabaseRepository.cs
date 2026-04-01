using System;

namespace DAL.Repositorys.Interfaces
{
    public interface IDatabaseRepository
    {
        //bool CheckIsDemoDatabase();
        void AddFileError();
        bool CheckIsFilesError();
        void AddLastUpdate();
        DateTime? GetLastUpdate();
        string GetDatabaseDate();
    }
}