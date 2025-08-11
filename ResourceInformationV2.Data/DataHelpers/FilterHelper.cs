using Microsoft.EntityFrameworkCore;
using ResourceInformationV2.Data.DataContext;
using ResourceInformationV2.Data.DataModels;
using ResourceInformationV2.Search.Helpers;

namespace ResourceInformationV2.Data.DataHelpers {

    public class FilterHelper(ResourceRepository? resourceRepository, BulkEditor? bulkEditor) {
        private readonly BulkEditor? _bulkEditor = bulkEditor;
        private readonly ResourceRepository _resourceRepository = resourceRepository ?? throw new ArgumentNullException("resourceRepository");

        public async Task<IEnumerable<IGrouping<TagType, Tag>>> GetAllFilters(string source) =>
            await _resourceRepository.ReadAsync(c => c.Tags.Include(c => c.Source).Where(ts => ts.Source != null && ts.Source.Code == source).OrderBy(ts => ts.Order).GroupBy(rv => rv.TagType));

        public async Task<List<Tag>> GetFilterListForExport(string source) => [.. (await GetAllFilters(source)).Select(t => new Tag {
                Title = t.Key.ToString(),
            })];

        public async Task<(List<Tag> TagSources, int SourceId)> GetFilters(string source, TagType tagType) {
            var returnValue = await _resourceRepository.ReadAsync(c => c.Tags.Include(c => c.Source).Where(ts => ts.Source != null && ts.Source.Code == source && ts.TagType == tagType).OrderBy(ts => ts.Order).ToList());
            var sourceId = 0;
            foreach (var item in returnValue) {
                item.OldTitle = item.Title;
                sourceId = item.SourceId;
            }
            if (sourceId == 0) {
                sourceId = (await _resourceRepository.ReadAsync(c => c.Sources.FirstOrDefault(s => s.Code == source)))?.Id ?? 0;
            }
            return (returnValue, sourceId);
        }

        public async Task<bool> ReplaceFilters(IEnumerable<Tag> tags, IEnumerable<Tag> tagsForDeletion, string sourceName) {
            var i = 1;

            foreach (var tag in tagsForDeletion.ToList().Where(t => t.Id != 0)) {
                _ = await _resourceRepository.DeleteAsync(tag);
            }
            foreach (var tag in tags.Where(t => t.Title != "")) {
                tag.Order = i++;
                _ = await _resourceRepository.CreateAsync(tag);
            }
            return true;
        }

        public async Task<bool> SaveFilters(IEnumerable<Tag> tags, IEnumerable<Tag> tagsForDeletion, string sourceName) {
            var i = 1;
            var tagsForDeletionList = tagsForDeletion.ToList();
            var duplicateTags = tags.GroupBy(t => new { t.Title, t.TagType }).Where(g => g.Count() > 1).ToList();
            if (duplicateTags.Count > 0) {
                foreach (var duplicateTag in duplicateTags) {
                    bool firstOne = true;
                    foreach (var tag in duplicateTag) {
                        if (tag.Id != 0 && !string.IsNullOrWhiteSpace(tag.OldTitle) && tag.Title != tag.OldTitle && _bulkEditor != null) {
                            _ = await _bulkEditor.UpdateTags(sourceName, tag.TagTypeSourceName, tag.OldTitle, tag.Title);
                        }
                        if (!firstOne) {
                            tag.Title = "";
                            tag.OldTitle = "";
                            tagsForDeletionList.Add(tag);
                        }
                        firstOne = false;
                    }
                }
            }

            foreach (var tag in tags.Where(t => t.Title != "")) {
                tag.Order = i++;
                if (tag.Id == 0) {
                    _ = await _resourceRepository.CreateAsync(tag);
                } else {
                    _ = await _resourceRepository.UpdateAsync(tag);
                    if (tag.Title != tag.OldTitle && _bulkEditor != null) {
                        _ = await _bulkEditor.UpdateTags(sourceName, tag.TagTypeSourceName, tag.OldTitle, tag.Title);
                    }
                }
            }
            foreach (var tag in tagsForDeletionList.Where(t => t.Id != 0)) {
                _ = await _resourceRepository.DeleteAsync(tag);
                if (_bulkEditor != null && !string.IsNullOrWhiteSpace(tag.OldTitle)) {
                    _ = await _bulkEditor.DeleteTags(sourceName, tag.TagTypeSourceName, tag.OldTitle);
                }
            }
            return true;
        }
    }
}