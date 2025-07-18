using Microsoft.EntityFrameworkCore;
using ResourceInformationV2.Data.DataContext;
using ResourceInformationV2.Data.DataModels;

namespace ResourceInformationV2.Data.DataHelpers {

    public class SourceHelper(ResourceRepository resourceRepository) {
        private readonly ResourceRepository _resourceRepository = resourceRepository;

        public async Task<string> CreateSource(string newSourceCode, string newTitle, string email) {
            var source = await _resourceRepository.ReadAsync(c => c.Sources.FirstOrDefault(s => s.Code == newSourceCode.ToLowerInvariant() || s.Title == newTitle));
            if (source != null) {
                return "Source code or name is in use";
            }
            _ = await _resourceRepository.CreateAsync(new Source { Code = newSourceCode.ToLowerInvariant(), CreatedByEmail = email, IsActive = true, IsTest = false, Title = newTitle });
            var newSource = await _resourceRepository.ReadAsync(pr => pr.Sources.FirstOrDefault(s => s.Code == newSourceCode.ToLowerInvariant()));
            if (newSource != null) {
                _ = await _resourceRepository.CreateAsync(new SecurityEntry { SourceId = newSource.Id, IsActive = true, IsFullAdmin = true, IsOwner = true, IsPublic = true, IsRequested = false, Email = email });
            }
            return $"Added source {newTitle} with code {newSourceCode}";
        }

        public async Task<string> DeleteSource(string sourceCode, string email) {
            var source = await _resourceRepository.ReadAsync(c => c.Sources.FirstOrDefault(s => s.Code == sourceCode.ToLowerInvariant()));
            if (source == null) {
                return "Source Code not found";
            } else if (source.IsTest) {
                return "Source Code in test";
            } else if (source.CreatedByEmail != email) {
                return "Source must be deleted by creator " + source.CreatedByEmail;
            }
            _ = await _resourceRepository.DeleteBySourceAsync(source);
            return $"Removed source {sourceCode}";
        }

        public async Task<bool> DoesSourceUseItem(string sourceCode, CategoryType categoryType) {
            var source = await _resourceRepository.ReadAsync(c => c.Sources.FirstOrDefault(s => s.Code == sourceCode));
            if (source == null) {
                return false;
            }
            switch (categoryType) {
                case CategoryType.Faq:
                    return source.UseFaqs;

                case CategoryType.Note:
                    return source.UseNotes;

                case CategoryType.Resource:
                    return source.UseResources;

                case CategoryType.Person:
                    return source.UsePeople;

                case CategoryType.Publication:
                    return source.UsePublications;
            }
            return false;
        }

        public async Task<string> GetBaseUrlFromSource(string sourceCode) {
            var source = await _resourceRepository.ReadAsync(c => c.Sources.FirstOrDefault(s => s.Code == sourceCode.ToLowerInvariant()));
            return source?.BaseUrl ?? "";
        }

        public async Task<Source?> GetSourceByCode(string sourceCode) => await _resourceRepository.ReadAsync(c => c.Sources.FirstOrDefault(s => s.Code == sourceCode));

        public async Task<string> GetSourceFilterName(string sourceCode, TagType tagType) {
            var source = await _resourceRepository.ReadAsync(c => c.Sources.FirstOrDefault(s => s.Code == sourceCode));
            if (source == null) {
                return "";
            }
            switch (tagType) {
                case TagType.Tag1:
                    return source.FilterTag1Title;

                case TagType.Tag2:
                    return source.FilterTag2Title;

                case TagType.Tag3:
                    return source.FilterTag3Title;

                case TagType.Tag4:
                    return source.FilterTag4Title;

                case TagType.Topic:
                    return source.FilterTopicTitle;

                case TagType.Audience:
                    return source.FilterAudienceTitle;
            }
            return "";
        }

        public async Task<Dictionary<string, string>> GetSources(string netId) => await _resourceRepository.ReadAsync(c => c.SecurityEntries.Include(se => se.Source).Where(se => se.IsActive && !se.IsRequested && se.Email == netId).ToDictionary(se => se.Source?.Code ?? "", se2 => se2.Source?.Title ?? ""));

        public async Task<IEnumerable<Tuple<string, string>>> GetSourcesAndOwners() => await _resourceRepository.ReadAsync(c => c.Sources.Where(s => s.IsActive).OrderBy(s => s.Title).Select(s => new Tuple<string, string>(s.CreatedByEmail, $"{s.Title} ({s.Code})")));

        public async Task<string> RequestAccess(string sourceCode, string email) {
            var source = await _resourceRepository.ReadAsync(c => c.Sources.FirstOrDefault(s => s.Code == sourceCode));
            if (source == null) {
                return "Source Code not found";
            }

            var existingItem = await _resourceRepository.ReadAsync(c => c.SecurityEntries.FirstOrDefault(s => s.SourceId == source.Id && s.Email == email));
            if (existingItem != null) {
                if (existingItem.IsActive) {
                    return "You already have access";
                } else if (existingItem.IsRequested) {
                    return "You entry is pending";
                } else {
                    return "You entry has been rejected -- please contact the owner for more information";
                }
            }

            var value = await _resourceRepository.CreateAsync(new SecurityEntry(email, source.Id, true));
            return $"Requested access to code {sourceCode}";
        }

        public async Task<int> SaveSource(Source source) => await _resourceRepository.UpdateAsync(source);

        public async Task<int> SetSourceFilterName(string sourceCode, TagType tagType, string title) {
            var source = await _resourceRepository.ReadAsync(c => c.Sources.FirstOrDefault(s => s.Code == sourceCode));
            if (source == null) {
                return 0;
            }
            switch (tagType) {
                case TagType.Tag1:
                    source.FilterTag1Title = title;
                    break;

                case TagType.Tag2:
                    source.FilterTag2Title = title;
                    break;

                case TagType.Tag3:
                    source.FilterTag3Title = title;
                    break;

                case TagType.Tag4:
                    source.FilterTag4Title = title;
                    break;

                case TagType.Topic:
                    source.FilterTopicTitle = title;
                    break;

                case TagType.Audience:
                    source.FilterAudienceTitle = title;
                    break;
            }
            _ = await _resourceRepository.UpdateAsync(source);
            return source.Id;
        }

        public async Task<int> SetSourceItem(string sourceCode, CategoryType categoryType, bool isUsed) {
            var source = await _resourceRepository.ReadAsync(c => c.Sources.FirstOrDefault(s => s.Code == sourceCode));
            if (source == null) {
                return 0;
            }
            switch (categoryType) {
                case CategoryType.Faq:
                    source.UseFaqs = isUsed;
                    break;

                case CategoryType.Note:
                    source.UseNotes = isUsed;
                    break;

                case CategoryType.Resource:
                    source.UseResources = isUsed;
                    break;

                case CategoryType.Person:
                    source.UsePeople = isUsed;
                    break;

                case CategoryType.Publication:
                    source.UsePublications = isUsed;
                    break;
            }
            _ = await _resourceRepository.UpdateAsync(source);
            return source.Id;
        }
    }
}