using Microsoft.EntityFrameworkCore;
using ResourceInformationV2.Data.DataContext;
using ResourceInformationV2.Data.DataModels;

namespace ResourceInformationV2.Data.DataHelpers {

    public class SecurityHelper(ResourceRepository resourceRepository) {
        private readonly ResourceRepository _resourceRepository = resourceRepository;

        public async Task<string> AddName(string sourceName, string netId) {
            var sourceId = (await _resourceRepository.ReadAsync(c => c.Sources.FirstOrDefault(s => s.Code == sourceName)))?.Id ?? 0;
            var isSuccessful = sourceId == 0
                ? false
                : await _resourceRepository.CreateAsync(new SecurityEntry {
                    IsActive = true,
                    IsFullAdmin = false,
                    IsOwner = false,
                    IsPublic = false,
                    IsRequested = false,
                    SourceId = sourceId,
                    Email = UpdateNetId(netId),
                }) > 0;
            return isSuccessful ? UpdateNetId(netId) : "";
        }

        public async Task<bool> ConfirmNetIdCanAccessSource(string sourceName, string netId) {
            return await _resourceRepository.ReadAsync(c => c.SecurityEntries.Include(c => c.Source).Any(se => se.Source != null && se.Source.Code == sourceName && se.Email == netId));
        }

        public async Task<List<string>> GetNames(string sourceName) {
            return await _resourceRepository.ReadAsync(c => c.SecurityEntries.Include(c => c.Source).Where(se => se.Source != null && se.Source.Code == sourceName).OrderBy(se => se.Email).Select(se => se.Email).ToList());
        }

        public async Task<bool> RemoveName(string sourceName, string netId) {
            var deletedName = await _resourceRepository.ReadAsync(c => c.SecurityEntries.Include(c => c.Source).FirstOrDefault(se => se.Source != null && se.Source.Code == sourceName && se.Email == netId));
            return deletedName == null
                ? false
                : await _resourceRepository.DeleteAsync(deletedName) > 0;
        }

        private string UpdateNetId(string netId) => $"{netId.Replace("@illinois.edu", "").ToLowerInvariant()}@illinois.edu";
    }
}