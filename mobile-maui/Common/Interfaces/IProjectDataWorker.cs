using System.IO;

namespace Common.Interfaces
{
    public interface IProjectDataWorker
    {
        bool IsUpdateNeeded(int newVersion);
        bool IsStructUpdateNeeded(int newStructVersion);
        bool IsStructChanged(int newStructVersion);
        bool FullDataBaseExist();
        bool DemoDataBaseExist();
        void DeleteExistingDemo();
        void DeleteFile(string filename);
        void StoreVersionFile(Stream fileStream);
        void StoreStructureVersionFile(Stream fileStream);
        bool CheckFileSize(string path, int size);
        bool ReplaceFileText(string path, string text);
        void MoveFile(string fromPath, string toPath);
        void CreateDiectory(string path);
        string GetDataStoredVersion();
    }
}