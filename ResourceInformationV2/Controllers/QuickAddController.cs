using Microsoft.AspNetCore.Mvc;
using ResourceInformationV2.Data.Cache;
using ResourceInformationV2.Data.DataHelpers;
using ResourceInformationV2.Data.DataModels;

namespace ResourceInformationV2.Controllers {
    [Route("[controller]")]
    public class QuickAddController(CacheHolder cacheHolder, SecurityHelper securityHelper, SourceHelper sourceHelper) : Controller {
        [Route("event/{source}")]
        [HttpGet]
        public async Task<IActionResult> Event(string source) => await Set(source, "/event/general", CategoryType.Event);

        [Route("faq/{source}")]
        [HttpGet]
        public async Task<IActionResult> Faq(string source) => await Set(source, "/faq/general", CategoryType.Faq);

        [Route("note/{source}")]
        [HttpGet]
        public async Task<IActionResult> Note(string source) => await Set(source, "/note/general", CategoryType.None);

        [Route("person/{source}")]
        [HttpGet]
        public async Task<IActionResult> Person(string source) => await Set(source, "/person/general", CategoryType.Person);

        [Route("publication/{source}")]
        [HttpGet]
        public async Task<IActionResult> Publication(string source) => await Set(source, "/publication/general", CategoryType.Publication);

        [Route("resource/{source}")]
        [HttpGet]
        public async Task<IActionResult> Resource(string source) => await Set(source, "/resource/general", CategoryType.Resource);

        private async Task<IActionResult> Set(string source, string url, CategoryType category) {
            if (string.IsNullOrEmpty(source)) {
                return Content("Error: source needs to be defined");
            }
            var netId = User.Identities.FirstOrDefault()?.Name;
            if (string.IsNullOrWhiteSpace(netId)) {
                return Content("Error: Net ID not found");
            }
            if (!await securityHelper.ConfirmNetIdCanAccessSource(source, netId)) {
                return Content($"Error: Net ID not allowed for source {source} / {netId}");
            }
            if (!await sourceHelper.DoesSourceUseItem(source, category)) {
                return Content($"Error: Source {source} not enabled for {category}");
            }

            var baseUrl = await sourceHelper.GetBaseUrlFromSource(source);
            cacheHolder.SetCacheSource(netId, source, baseUrl);
            return Redirect(url);
        }
    }
}
