using Microsoft.EntityFrameworkCore;
using ResourceInformationV2.Data.DataContext;
using ResourceInformationV2.Data.DataModels;

namespace ResourceInformationV2.Data.DataHelpers;

public class SecurityHelper(ResourceRepository resourceRepository) {

    public async Task<string> AddName(string sourceName, string netId, string departmentName) {
        var sourceId = (await resourceRepository.ReadAsync(c => c.Sources.FirstOrDefault(s => s.Code == sourceName)))?.Id ?? 0;
        var email = UpdateNetId(netId);
        if (sourceId == 0 || string.IsNullOrWhiteSpace(email)) {
            return "";
        }
        return await resourceRepository.CreateAsync(new SecurityEntry {
            IsActive = true,
            IsFullAdmin = false,
            IsOwner = false,
            IsPublic = false,
            IsRequested = false,
            SourceId = sourceId,
            DepartmentTag = departmentName,
            Email = email,
        }) > 0 ? email : "";
    }

    public async Task<string> GetDepartmentName(string sourceName, string netId) {
        var entry = await resourceRepository.ReadAsync(c => c.SecurityEntries.Include(c => c.Source).FirstOrDefault(se => se.Source != null && se.Source.Code == sourceName.Replace("!", "") && se.Email == netId));
        return entry?.DepartmentTag ?? "";
    }

    public async Task<bool> ConfirmNetIdCanAccessSource(string sourceName, string netId) {
        return await resourceRepository.ReadAsync(c => c.SecurityEntries.Include(c => c.Source).Any(se => se.Source != null && se.Source.Code == sourceName.Replace("!", "") && se.Email == netId));
    }

    public async Task<List<SecurityEntry>> GetNames(string sourceName) {
        return [.. await resourceRepository.ReadAsync(c => c.SecurityEntries.Include(c => c.Source).Where(se => se.Source != null && se.Source.Code == sourceName).OrderBy(se => se.Email))];
    }

    public async Task<bool> RemoveName(string sourceName, string netId) {
        var deletedName = await resourceRepository.ReadAsync(c => c.SecurityEntries.Include(c => c.Source).FirstOrDefault(se => se.Source != null && se.Source.Code == sourceName && se.Email == netId));
        if (deletedName == null || deletedName.IsOwner) {
            return false;
        }
        return await resourceRepository.DeleteAsync(deletedName) > 0;
    }

    private string UpdateNetId(string netId) => string.IsNullOrWhiteSpace(netId) ? "" : $"{netId.Replace("@illinois.edu", "").ToLowerInvariant().Trim()}@illinois.edu";
}