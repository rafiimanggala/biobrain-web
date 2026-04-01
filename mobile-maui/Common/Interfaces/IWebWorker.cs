using System;

namespace Common.Interfaces
{
	public interface IWebWorker : IDisposable
	{
		bool IsBusy { get; }
		int Id { get; set; }
		void DownloadFileInTask(string url, string path);
		void DownloadFile(string url, string path);
	}
}