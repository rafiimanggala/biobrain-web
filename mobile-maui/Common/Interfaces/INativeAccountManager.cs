using System.Threading.Tasks;

namespace Common.Interfaces
{
    public interface INativeAccountManager
    {
        Task<string> GetEmail();
    }
}