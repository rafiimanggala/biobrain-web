using System.IO;

namespace CustomControls.Interfaces
{
    public interface IImageStream
    {
        Stream GetStream(string path);
    }
}