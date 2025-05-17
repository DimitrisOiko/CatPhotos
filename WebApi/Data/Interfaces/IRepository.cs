using System.Data;
using WebApi.Data.Models;

namespace WebApi.Data.Interfaces
{
    public interface IRepository
    {
        Task AddCat(Cat cat);
        Task<bool> CatExistsAsync(string id);
        Task AddTags(List<Tag> tags);
        Task<List<Tag>> FindExistingTagsAsync(HashSet<string> names);
        Task<List<Cat>> GetAllCatsAsync(int page, int pageSize);
        Task<Cat> GetCatByIdAsync(int id);
        Task<List<Cat>> GetCatsByTagAsync(string tag, int page, int pageSize);
        void BeginTransaction(IsolationLevel isolationlevel);
        void PersistTransaction();
        void RollbackTransaction();
        void SaveChanges();
    }
}
