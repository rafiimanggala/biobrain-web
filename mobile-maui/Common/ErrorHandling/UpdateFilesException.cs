using System;

namespace Common.ErrorHandling
{
    public class UpdateFilesException : Exception
    {
        public string FileName { get; }
        public override string Message { get; }

        public UpdateFilesException(string fileName)
        {
            FileName = fileName;
        }

        public UpdateFilesException(string fileName, string message)
        {
            FileName = fileName;
            Message = message;
        }
    }
}