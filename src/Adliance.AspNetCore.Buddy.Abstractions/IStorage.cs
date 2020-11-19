using System.IO;
using System.Threading.Tasks;

namespace Adliance.AspNetCore.Buddy.Abstractions
{
    public interface IStorage
    {
        Task Save(byte[] bytes, params string[] path);
        Task Save(Stream stream, params string[] path);
        
        Task<byte[]?> Load(params string[] path);
        Task Load(Stream stream, params string[] path);
        
        Task<bool> Exists(params string[] path);
        Task Delete(params string[] path);
    }
}
