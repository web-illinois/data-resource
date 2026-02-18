using ResourceInformationV2.Data.DataContext;
using ResourceInformationV2.Data.DataModels;
using ResourceInformationV2.Search.Getters;
using ResourceInformationV2.Search.Setters;
using System.Net;

namespace ResourceInformationV2.Data.DataHelpers {
    public class LinkCheckHelper(ResourceGetter resourceGetter, ResourceSetter resourceSetter, ResourceRepository resourceRepository) {
        private readonly ResourceGetter _resourceGetter = resourceGetter;
        private readonly ResourceSetter _resourceSetter = resourceSetter;
        private readonly ResourceRepository _resourceRepository = resourceRepository;

        public async Task<(int, bool)> AddResources(string source) {
            var sourceItem = await _resourceRepository.ReadAsync(context => context.Sources.FirstOrDefault(s => s.Code == source));
            if (sourceItem != null) {
                var existingLinkCount = await _resourceRepository.ReadAsync(context => context.LinkChecks.Count(lc => lc.SourceId == sourceItem.Id && lc.DateChecked == null));
                if (existingLinkCount > 0) {
                    return (existingLinkCount, false);
                }
                _ = await _resourceRepository.DeleteLinkCheckBySourceAsync(sourceItem.Id);
                var resources = await _resourceGetter.Search(source, "", [], [], [], [], [], [], [], 10000, 0, "", true);
                var links = resources.Items.Select(r => new LinkCheck { SourceId = sourceItem.Id, ItemGuid = r.Id, LastUpdated = DateTime.Now, Title = r.Title, Url = r.Url, EditLink = r.EditLink }).Where(l => !string.IsNullOrEmpty(l.Url)).OrderBy(l => l.Title);
                foreach (var link in links) {
                    await _resourceRepository.CreateAsync(link);
                }
                sourceItem.DateLastUrlCheck = DateTime.Now;
                _ = await _resourceRepository.UpdateAsync(sourceItem);
                return (links.Count(), true);
            }
            return (0, false);
        }

        public async Task<(int, DateTime?, List<LinkCheck>)> GetLinkCheckStatus(string source) {
            var sourceItem = await _resourceRepository.ReadAsync(context => context.Sources.FirstOrDefault(s => s.Code == source));
            if (sourceItem != null) {
                var linkCount = await _resourceRepository.ReadAsync(context => context.LinkChecks.Count(lc => lc.SourceId == sourceItem.Id && lc.DateChecked == null));
                var links = await _resourceRepository.ReadAsync(context => context.LinkChecks.Where(lc => lc.SourceId == sourceItem.Id && lc.DateChecked != null && !lc.IsSuccessful).OrderBy(lc => lc.Title).ToList());
                return (linkCount, sourceItem.DateLastUrlCheck, links);
            }
            return (0, null, new List<LinkCheck>());
        }

        public async Task<string> CheckLink(int count) {
            var links = await _resourceRepository.ReadAsync(context => context.LinkChecks.Where(lc => lc.DateChecked == null).OrderBy(lc => lc.LastUpdated).Take(count));
            if (links != null) {
                var returnValue = "";
                foreach (var link in links) {
                    try {
                        using var httpClient = new HttpClient();
                        var response = await httpClient.SendAsync(new HttpRequestMessage {
                            Version = HttpVersion.Version30,
                            RequestUri = new Uri(link.Url),
                            Method = HttpMethod.Get
                        });
                        link.ResponseStatusCode = response.StatusCode.ToString();
                        link.ResponseMessage = response.ReasonPhrase ?? "";
                        link.IsSuccessful = response.IsSuccessStatusCode;
                    } catch (Exception e) {
                        link.ResponseStatusCode = "Server Error";
                        link.ResponseMessage = e.Message;
                        link.IsSuccessful = false;
                    }
                    link.DateChecked = DateTime.Now;
                    if (!link.IsSuccessful) {
                        var item = await _resourceGetter.GetItem(link.ItemGuid);
                        if (item.UseManualCheck) {
                            link.ResponseStatusCode = "Manual Check required";
                            link.ResponseMessage += ": Manual Check required";
                        } else {
                            item.IsActive = false;
                        }
                        _ = await _resourceSetter.SetItem(item);
                    }
                    _ = await _resourceRepository.UpdateAsync(link);
                    returnValue += $"{link.Title} / {link.Source} / {link.Url}: {link.ResponseStatusCode}\r\n";
                }
                return returnValue;
            }
            return "";
        }
    }
}
