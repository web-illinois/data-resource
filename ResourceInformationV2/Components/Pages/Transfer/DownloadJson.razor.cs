using System.Text;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ResourceInformationV2.Components.Layout;
using ResourceInformationV2.Data.DataHelpers;
using ResourceInformationV2.Data.PageList;
using ResourceInformationV2.Search.Helpers;
using ResourceInformationV2.Search.Models;

namespace ResourceInformationV2.Components.Pages.Transfer {

    public partial class DownloadJson {
        public List<string> Options = [];

        [CascadingParameter]
        public SidebarLayout Layout { get; set; } = default!;

        public string SelectedOption { get; set; } = "";

        [Inject]
        protected JsonHelper JsonHelper { get; set; } = default!;

        [Inject]
        protected IJSRuntime JsRuntime { get; set; } = default!;

        [Inject]
        protected NavigationManager NavigationManager { get; set; } = default!;

        [Inject]
        protected SourceHelper SourceHelper { get; set; } = default!;

        protected override async Task OnInitializedAsync() {
            Layout.SetSidebar(SidebarEnum.Transfer, "Transfer Items");
            var sourceCode = await Layout.CheckSource();
            Options = [.. (await SourceHelper.DoesSourceUseItemCheckAllAndConcatenate(sourceCode))];
            SelectedOption = Options.First();
        }

        private async Task Download() {
            await Layout.AddMessage("JSON file being prepared -- this may take a while.");
            var source = await Layout.CheckSource();

            Enum.TryParse(SelectedOption, out UrlTypes urlType);
            var text = await JsonHelper.GetJsonFull(source, urlType);
            var fileStream = new MemoryStream(Encoding.ASCII.GetBytes(text));
            using var streamRef = new DotNetStreamReference(fileStream);
            await JsRuntime.InvokeVoidAsync("downloadFileFromStream", $"{source}_{DateTime.Now.ToString("yyyy_MM_dd")}_{SelectedOption.ToLowerInvariant()}.json", streamRef);
            await Layout.AddMessage("JSON file downloaded successfully.");
        }
    }
}