using Microsoft.AspNetCore.Mvc;
using ResourceInformationV2.Data.Cache;
using ResourceInformationV2.Data.DataHelpers;

namespace ResourceInformationV2.Controllers {

    [Route("[controller]")]
    public class QuickLinkController(CacheHolder cacheHolder, SecurityHelper securityHelper, SourceHelper sourceHelper) : Controller {
        private readonly CacheHolder _cacheHolder = cacheHolder;
        private readonly SecurityHelper _securityHelper = securityHelper;
        private readonly SourceHelper _sourceHelper = sourceHelper;

        [Route("event/{id}")]
        [HttpGet]
        public async Task<IActionResult> Event(string id) => await Set(id, "/event/general");

        [Route("event/{id}/technical")]
        [HttpGet]
        public async Task<IActionResult> EventTechnical(string id) => await Set(id, "/event/technical");

        [Route("faq/{id}")]
        [HttpGet]
        public async Task<IActionResult> Faq(string id) => await Set(id, "/faq/general");

        [Route("faq/{id}/technical")]
        [HttpGet]
        public async Task<IActionResult> FaqTechnical(string id) => await Set(id, "/faq/technical");

        [Route("note/{id}")]
        [HttpGet]
        public async Task<IActionResult> Note(string id) => await Set(id, "/note/general");

        [Route("note/{id}/technical")]
        [HttpGet]
        public async Task<IActionResult> NoteTechnical(string id) => await Set(id, "/note/technical");

        [Route("person/{id}")]
        [HttpGet]
        public async Task<IActionResult> Person(string id) => await Set(id, "/person/general");

        [Route("person/{id}/technical")]
        [HttpGet]
        public async Task<IActionResult> PersonTechnical(string id) => await Set(id, "/person/technical");

        [Route("publication/{id}")]
        [HttpGet]
        public async Task<IActionResult> Publication(string id) => await Set(id, "/publication/general");

        [Route("publication/{id}/technical")]
        [HttpGet]
        public async Task<IActionResult> PublicationTechnical(string id) => await Set(id, "/publication/technical");

        [Route("resource/{id}")]
        [HttpGet]
        public async Task<IActionResult> Resource(string id) => await Set(id, "/resource/general");

        [Route("resource/{id}/technical")]
        [HttpGet]
        public async Task<IActionResult> ResourceTechnical(string id) => await Set(id, "/resource/technical");

        private async Task<IActionResult> Set(string id, string url) {
            if (string.IsNullOrEmpty(id)) {
                return Content("Error: ID needs to be added");
            }
            var netId = User.Identities.FirstOrDefault()?.Name;
            if (string.IsNullOrWhiteSpace(netId)) {
                return Content("Error: Net ID not found");
            }
            var sourceName = id.Split('-')[0].Replace("!", "");
            if (!await _securityHelper.ConfirmNetIdCanAccessSource(sourceName, netId)) {
                return Content($"Error: Net ID not allowed for source {sourceName} / {netId}");
            }
            var baseUrl = await _sourceHelper.GetBaseUrlFromSource(sourceName);
            _cacheHolder.SetCacheSource(netId, sourceName, baseUrl);
            _cacheHolder.SetCacheItem(netId, id);
            return Redirect(url);
        }
    }
}