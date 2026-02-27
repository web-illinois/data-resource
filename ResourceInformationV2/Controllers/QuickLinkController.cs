using Microsoft.AspNetCore.Mvc;
using ResourceInformationV2.Data.Cache;
using ResourceInformationV2.Data.DataHelpers;
using ResourceInformationV2.Data.DataModels;
using ResourceInformationV2.Search.Getters;
using ResourceInformationV2.Search.Models;

namespace ResourceInformationV2.Controllers {

    [Route("[controller]")]
    public class QuickLinkController(CacheHolder cacheHolder, SecurityHelper securityHelper, SourceHelper sourceHelper,
        ResourceGetter resourceGetter, PublicationGetter publicationGetter, EventGetter eventGetter, PersonGetter personGetter,
        NoteGetter noteGetter, FaqGetter faqGetter) : Controller {
        [Route("event/{id}")]
        [HttpGet]
        public async Task<IActionResult> Event(string id) => await Set(id, "/event/general", CategoryType.Event);

        [Route("event/{id}/technical")]
        [HttpGet]
        public async Task<IActionResult> EventTechnical(string id) => await Set(id, "/event/technical", CategoryType.Event);

        [Route("faq/{id}")]
        [HttpGet]
        public async Task<IActionResult> Faq(string id) => await Set(id, "/faq/general", CategoryType.Faq);

        [Route("faq/{id}/technical")]
        [HttpGet]
        public async Task<IActionResult> FaqTechnical(string id) => await Set(id, "/faq/technical", CategoryType.Faq);

        [Route("note/{id}")]
        [HttpGet]
        public async Task<IActionResult> Note(string id) => await Set(id, "/note/general", CategoryType.Note);

        [Route("note/{id}/technical")]
        [HttpGet]
        public async Task<IActionResult> NoteTechnical(string id) => await Set(id, "/note/technical", CategoryType.Note);

        [Route("person/{id}")]
        [HttpGet]
        public async Task<IActionResult> Person(string id) => await Set(id, "/person/general", CategoryType.Person);

        [Route("person/{id}/technical")]
        [HttpGet]
        public async Task<IActionResult> PersonTechnical(string id) => await Set(id, "/person/technical", CategoryType.Person);

        [Route("publication/{id}")]
        [HttpGet]
        public async Task<IActionResult> Publication(string id) => await Set(id, "/publication/general", CategoryType.Publication);

        [Route("publication/{id}/technical")]
        [HttpGet]
        public async Task<IActionResult> PublicationTechnical(string id) => await Set(id, "/publication/technical", CategoryType.Publication);

        [Route("resource/{id}")]
        [HttpGet]
        public async Task<IActionResult> Resource(string id) => await Set(id, "/resource/general", CategoryType.Resource);

        [Route("resource/{id}/technical")]
        [HttpGet]
        public async Task<IActionResult> ResourceTechnical(string id) => await Set(id, "/resource/technical", CategoryType.Resource);

        private async Task<IActionResult> Set(string id, string url, CategoryType category) {
            if (string.IsNullOrEmpty(id)) {
                return Content("Error: ID needs to be added");
            }
            var netId = User.Identities.FirstOrDefault()?.Name;
            if (string.IsNullOrWhiteSpace(netId)) {
                return Content("Error: Net ID not found");
            }

            BaseObject? item = category switch {
                CategoryType.Resource => await resourceGetter.GetItem(id),
                CategoryType.Publication => await publicationGetter.GetItem(id),
                CategoryType.Event => await eventGetter.GetItem(id),
                CategoryType.Person => await personGetter.GetItem(id),
                CategoryType.Faq => await faqGetter.GetItem(id),
                CategoryType.Note => await noteGetter.GetItem(id),
                _ => null
            };
            if (item == null || item.Source == "") {
                return Content($"Error: Item cannot be found {id}");
            }
            var sourceName = id.Split('-')[0].Replace("!", "");
            if (!await securityHelper.ConfirmNetIdCanAccessSource(sourceName, netId)) {
                return Content($"Error: Net ID not allowed for source {sourceName} / {netId}");
            }

            var department = await securityHelper.GetDepartmentName(sourceName, netId);
            if (!string.IsNullOrWhiteSpace(department) && !item.DepartmentList.Contains(department)) {
                return Content($"Error: Net ID {netId} is restricted to department {department}");
            }
            var baseUrl = await sourceHelper.GetBaseUrlFromSource(sourceName);
            cacheHolder.SetCacheSource(netId, sourceName, baseUrl);
            cacheHolder.SetCacheItem(netId, id);
            return Redirect(url);
        }
    }
}