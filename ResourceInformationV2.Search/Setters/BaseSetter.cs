using OpenSearch.Client;
using ResourceInformationV2.Search.Models;

namespace ResourceInformationV2.Search.Setters {

    public abstract class BaseSetter<T>(OpenSearchClient? openSearchClient) where T : BaseObject, new() {
        internal readonly OpenSearchClient _openSearchClient = openSearchClient ?? default!;

        private static readonly char[] _trimChars = [' ', '\n', '\r'];
        public List<string> AddedAudience { get; private set; } = [];
        public List<string> AddedDepartment { get; private set; } = [];
        public List<string> AddedTag { get; private set; } = [];
        public List<string> AddedTag2 { get; private set; } = [];
        public List<string> AddedTag3 { get; private set; } = [];
        public List<string> AddedTag4 { get; private set; } = [];
        public List<string> AddedTopic { get; private set; } = [];
        internal abstract string IndexName { get; }

        public void ClearAddedTags() {
            AddedAudience = [];
            AddedDepartment = [];
            AddedTag = [];
            AddedTag2 = [];
            AddedTag3 = [];
            AddedTag4 = [];
            AddedTopic = [];
        }

        public async Task<string> DeleteItem(string id) {
            var response = await _openSearchClient.DeleteAsync<T>(id, i => i.Index(IndexName));
            return response.IsValid ? id : "";
        }

        public async Task<string> PublishDraftItem(T item) {
            var oldId = item.Id;
            item.Id = item.Id.Replace(item.Source + "!-", item.Source + "-");
            item.IsActive = true;
            item.IsDraftAvailable = false;
            item.CreatedOn = DateTime.Now;
            item.Prepare();
            var response = await _openSearchClient.IndexAsync(item, i => i.Index(IndexName));
            if (response.IsValid) {
                var responseDelete = await _openSearchClient.DeleteAsync<T>(oldId, i => i.Index(IndexName));
                return responseDelete.IsValid ? item.Id : "";
            }
            return "";
        }

        public async Task<string> SetItem(T item) {
            item.Prepare();
            var response = await _openSearchClient.IndexAsync(item, i => i.Index(IndexName));
            return response.IsValid ? item.Id : "";
        }

        public async Task<string> SetItemWithDraft(T item) {
            if (!string.IsNullOrWhiteSpace(item.Id)) {
                var responseOriginalItem = await _openSearchClient.GetAsync<T>(item.Id);
                if (responseOriginalItem != null && responseOriginalItem.IsValid && responseOriginalItem.Source != null && responseOriginalItem.Source.IsActive) {
                    var originalItem = responseOriginalItem.Source;
                    originalItem.IsDraftAvailable = true;
                    item.Id = item.Id.Replace(item.Source + "-", item.Source + "!-");
                    _ = await _openSearchClient.IndexAsync(originalItem, i => i.Index(IndexName));
                }
            }
            item.IsActive = false;
            item.CreatedOn = DateTime.Now;
            var response = await _openSearchClient.IndexAsync(item, i => i.Index(IndexName));
            return response.IsValid ? item.Id : "";
        }

        public async Task<string> UploadFile(string source, string file, bool validateTags, IEnumerable<string> tag1, IEnumerable<string> tag2, IEnumerable<string> tag3, IEnumerable<string> tag4, IEnumerable<string> audience, IEnumerable<string> topic, IEnumerable<string> department) {
            var countSuccess = 0;
            var lineNumber = 0;
            var headerErrorMessage = "";
            var badLineNumbers = new List<string>();
            var lines = file.Split('\n').Select(line => line.Trim(_trimChars));
            foreach (var line in lines) {
                lineNumber++;
                var item = new T();
                (var successful, var headerIssue, var message) = item.LoadFromString(source, line);
                if (successful) {
                    if (validateTags) {
                        item.TagList = item.TagList.Where(t => tag1.Contains(t));
                        item.Tag2List = item.Tag2List.Where(t => tag2.Contains(t));
                        item.Tag3List = item.Tag3List.Where(t => tag3.Contains(t));
                        item.Tag4List = item.Tag4List.Where(t => tag4.Contains(t));
                        item.AudienceList = item.AudienceList.Where(t => audience.Contains(t));
                        item.TopicList = item.TopicList.Where(t => topic.Contains(t));
                        item.DepartmentList = item.DepartmentList.Where(t => department.Contains(t));
                    } else {
                        AddedTag.AddRange(item.TagList.Where(t => !AddedTag.Contains(t)));
                        AddedTag2.AddRange(item.Tag2List.Where(t => !AddedTag2.Contains(t)));
                        AddedTag3.AddRange(item.Tag3List.Where(t => !AddedTag3.Contains(t)));
                        AddedTag4.AddRange(item.Tag4List.Where(t => !AddedTag4.Contains(t)));
                        AddedAudience.AddRange(item.AudienceList.Where(t => !AddedAudience.Contains(t)));
                        AddedTopic.AddRange(item.TopicList.Where(t => !AddedTopic.Contains(t)));
                        AddedTopic.AddRange(item.DepartmentList.Where(t => !AddedDepartment.Contains(t)));
                    }
                    _ = await SetItem(item);
                    countSuccess++;
                } else if (headerIssue && string.IsNullOrWhiteSpace(headerErrorMessage)) {
                    headerErrorMessage = message;
                } else {
                    badLineNumbers.Add(lineNumber.ToString());
                }
            }
            if (countSuccess == 0) {
                return string.IsNullOrWhiteSpace(headerErrorMessage) ? "No valid items found in file" : "Bad file: " + headerErrorMessage;
            }
            if (badLineNumbers.Count > 20) {
                return $"File loaded - number of items: {countSuccess}. Number of failures: {badLineNumbers.Count}";
            }
            return badLineNumbers.Count > 0
                ? $"File loaded - number of items: {countSuccess}. Failure at line numbers {string.Join(", ", badLineNumbers)}"
                : $"File loaded - number of items: {countSuccess}.";
        }
    }
}