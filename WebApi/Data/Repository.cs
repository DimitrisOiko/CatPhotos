using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;
using WebApi.Data.Interfaces;
using WebApi.Data.Models;

namespace WebApi.Data
{
    public class Repository(AppDbContext context) : IRepository
    {
        private readonly AppDbContext _context = context;
        private IDbContextTransaction transaction = null;

        public async Task AddCat(Cat cat) => await _context.Cats.AddAsync(cat);

        public async Task<bool> CatExistsAsync(string id) => await _context.Cats.AnyAsync(x => x.CatId == id);

        public async Task AddTags(List<Tag> tags) => await _context.Tags.AddRangeAsync(tags);

        public async Task<List<Tag>> FindExistingTagsAsync(HashSet<string> names) => await _context.Tags.Where(tag => names.Contains(tag.Name)).ToListAsync() ?? [];

        public async Task<List<Cat>> GetAllCatsAsync(int page, int pageSize)
        {
            return await _context.Cats
                .Select(c => new Cat
                {
                    Id = c.Id,
                    CatId = c.CatId,
                    Width = c.Width,
                    Height = c.Height,
                    Created = c.Created,
                    Tags = c.Tags.Select(t => new Tag
                    {
                        Id = t.Id,
                        Name = t.Name,
                        Created = t.Created
                    }).ToList()
                })
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Cat> GetCatByIdAsync(int id)
        {
            return await _context.Cats
                .Include(c => c.Tags)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<List<Cat>> GetCatsByTagAsync(string tag, int page, int pageSize)
        {
            return await _context.Cats
                .Select(c => new Cat
                {
                    Id = c.Id,
                    CatId = c.CatId,
                    Width = c.Width,
                    Height = c.Height,
                    Created = c.Created,
                    Tags = c.Tags.Select(t => new Tag
                    {
                        Id = t.Id,
                        Name = t.Name,
                        Created = t.Created
                    }).ToList()
                })
                .Where(c => c.Tags.Any(t => t.Name == tag))
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();
        }

        public void BeginTransaction(IsolationLevel isolationlevel) => transaction ??= _context.Database.BeginTransaction(isolationlevel);

        public void PersistTransaction()
        {
            if (transaction == null)
                return;

            try
            {
                transaction.Commit();
                transaction.Dispose();
                transaction = null;
            }
            catch
            {
                RollbackTransaction();
            }
        }

        public void RollbackTransaction()
        {
            if (transaction == null)
                return;

            transaction.Rollback();
            transaction.Dispose();
            transaction = null;
        }

        public void SaveChanges() => _context.SaveChanges();
    }
}
