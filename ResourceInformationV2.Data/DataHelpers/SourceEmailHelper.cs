using ResourceInformationV2.Data.DataContext;
using ResourceInformationV2.Data.DataModels;

namespace ResourceInformationV2.Data.DataHelpers {

    public class SourceEmailHelper(ResourceRepository resourceRepository) {
        private readonly ResourceRepository _resourceRepository = resourceRepository;

        public async Task<int> DeleteSourceEmail(SourceEmail sourceEmail) => sourceEmail.Id == 0 ? 0 : await _resourceRepository.DeleteAsync(sourceEmail);

        public async Task<SourceEmail> GetSourceEmail(string sourceName, EmailType email) {
            var sourceId = (await _resourceRepository.ReadAsync(c => c.Sources.FirstOrDefault(s => s.Code == sourceName)))?.Id ?? 0;
            var sourceEmail = await _resourceRepository.ReadAsync(c => c.SourceEmails.FirstOrDefault(se => se.SourceId == sourceId && se.EmailType == email));
            return sourceEmail ?? new SourceEmail { SourceId = sourceId, EmailType = email, IsActive = false };
        }

        public async Task<int> SaveSourceEmail(SourceEmail sourceEmail) => sourceEmail.Id == 0 ? await _resourceRepository.CreateAsync(sourceEmail) : await _resourceRepository.UpdateAsync(sourceEmail);
    }
}