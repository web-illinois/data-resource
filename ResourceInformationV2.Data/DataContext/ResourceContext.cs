using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using ResourceInformationV2.Data.DataModels;

namespace ResourceInformationV2.Data.DataContext {

    public class ResourceContext : DbContext {
        private readonly Guid _id;

        public ResourceContext() : base() {
            _id = Guid.NewGuid();
            Debug.WriteLine($"{_id} context created.");
        }

        public ResourceContext(DbContextOptions<ResourceContext> options) : base(options) {
            _id = Guid.NewGuid();
            Debug.WriteLine($"{_id} context created.");
        }

        public DbSet<Instruction> Instructions { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<SecurityEntry> SecurityEntries { get; set; }
        public DbSet<SourceEmail> SourceEmails { get; set; }
        public DbSet<Source> Sources { get; set; }
        public DbSet<Tag> Tags { get; set; }

        public override void Dispose() {
            Debug.WriteLine($"{_id} context disposed.");
            base.Dispose();
        }

        public override ValueTask DisposeAsync() {
            Debug.WriteLine($"{_id} context disposed async.");
            return base.DisposeAsync();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            Debug.WriteLine($"{_id} context starting initial setup.");
            modelBuilder.Entity<Source>().HasData(new List<Source>
            {
                new() { Id = -1, Code = "test", Title = "Test Entry", CreatedByEmail = "jonker@illinois.edu", IsTest = true },
            });
            modelBuilder.Entity<SecurityEntry>().HasData(new List<SecurityEntry>
            {
                new("jonker", -1) { Id = -1, IsOwner = true }
            });
            Debug.WriteLine($"{_id} context finishing initial setup.");
        }
    }
}