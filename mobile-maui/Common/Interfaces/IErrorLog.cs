using System.Diagnostics.Contracts;

namespace Common.Interfaces
{
    public interface IErrorLog
    {
        void Log(string text);
        string GetLog();
        bool OldLogFileExist();
        void Init();
    }
}