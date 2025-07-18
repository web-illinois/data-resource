using Microsoft.EntityFrameworkCore;
using ResourceInformationV2.Data.DataModels;

namespace ResourceInformationV2.Data.DataContext {

    public class ResourceRepository(IDbContextFactory<ResourceContext> factory) {
        private readonly IDbContextFactory<ResourceContext> _factory = factory;

        public int Create<T>(T item) where T : BaseDataItem {
            using var context = _factory.CreateDbContext();
            item.LastUpdated = DateTime.Now;
            _ = context.Add(item);
            return context.SaveChanges();
        }

        public async Task<int> CreateAsync<T>(T item) where T : BaseDataItem {
            using var context = _factory.CreateDbContext();
            item.LastUpdated = DateTime.Now;
            _ = context.Add(item);
            return await context.SaveChangesAsync();
        }

        public int Delete<T>(T item) {
            if (item == null) {
                return 0;
            }
            using var context = _factory.CreateDbContext();
            _ = context.Remove(item);
            return context.SaveChanges();
        }

        public async Task<int> DeleteAsync<T>(T item) {
            if (item == null) {
                return 0;
            }
            using var context = _factory.CreateDbContext();
            _ = context.Remove(item);
            return await context.SaveChangesAsync();
        }

        public async Task<int> DeleteBySourceAsync(Source source) {
            using var context = _factory.CreateDbContext();
            context.RemoveRange(context.Instructions.Where(i => i.SourceId == source.Id));
            context.RemoveRange(context.Tags.Where(i => i.SourceId == source.Id));
            context.RemoveRange(context.SecurityEntries.Where(i => i.SourceId == source.Id));
            context.Remove(source);
            return await context.SaveChangesAsync();
        }

        public T Read<T>(Func<ResourceContext, T> work) {
            var context = _factory.CreateDbContext();
            return work(context);
        }

        public async Task<T> ReadAsync<T>(Func<ResourceContext, T> work) {
            var context = _factory.CreateDbContext();
            return await Task.Run(() => work(context));
        }

        public int Update<T>(T item) where T : BaseDataItem {
            using var context = _factory.CreateDbContext();
            item.LastUpdated = DateTime.Now;
            _ = context.Update(item);
            return context.SaveChanges();
        }

        public int UpdateActive<T>(T item, bool active) where T : BaseDataItem {
            using var context = _factory.CreateDbContext();
            item.LastUpdated = DateTime.Now;
            item.IsActive = active;
            _ = context.Update(item);
            return context.SaveChanges();
        }

        public async Task<int> UpdateActiveAsync<T>(T item, bool active) where T : BaseDataItem {
            using var context = _factory.CreateDbContext();
            item.LastUpdated = DateTime.Now;
            item.IsActive = active;
            _ = context.Update(item);
            return await context.SaveChangesAsync();
        }

        public async Task<int> UpdateAsync<T>(T item) where T : BaseDataItem {
            using var context = _factory.CreateDbContext();
            item.LastUpdated = DateTime.Now;
            _ = context.Update(item);
            return await context.SaveChangesAsync();
        }
    }
}