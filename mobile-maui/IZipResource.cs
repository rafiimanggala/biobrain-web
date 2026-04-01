using System.IO;

namespace BioBrain
{
	public interface IZipResource
	{
		void Unzip(Stream zipStream, string dstPath);
	}
}

