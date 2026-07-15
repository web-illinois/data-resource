using Microsoft.EntityFrameworkCore;
using ResourceInformationV2.Data.DataModels;
using System.Diagnostics;

namespace ResourceInformationV2.Data.DataContext {

    public class ResourceContext : DbContext {
        public ResourceContext() : base() {
            Debug.WriteLine("Context created.");
        }

        public ResourceContext(DbContextOptions<ResourceContext> options) : base(options) {
            Debug.WriteLine("Context created.");
        }

        public DbSet<Instruction> Instructions { get; set; }
        public DbSet<LinkCheck> LinkChecks { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<SecurityEntry> SecurityEntries { get; set; }
        public DbSet<SourceEmail> SourceEmails { get; set; }
        public DbSet<Source> Sources { get; set; }
        public DbSet<Tag> Tags { get; set; }

        public override void Dispose() {
            Debug.WriteLine("Context disposed.");
            base.Dispose();
        }

        public override ValueTask DisposeAsync() {
            Debug.WriteLine("Context disposed async.");
            return base.DisposeAsync();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            Debug.WriteLine("Context starting initial setup.");
            Debug.WriteLine("Context finishing initial setup.");
        }
    }
}