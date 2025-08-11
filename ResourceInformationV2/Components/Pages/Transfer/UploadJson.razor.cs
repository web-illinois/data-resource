using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using ResourceInformationV2.Components.Layout;
using ResourceInformationV2.Data.DataHelpers;
using ResourceInformationV2.Data.PageList;
using ResourceInformationV2.Search.Helpers;
using ResourceInformationV2.Search.Models;

namespace ResourceInformationV2.Components.Pages.Transfer {

    public partial class UploadJson {
        public List<string> Options = [];
        private const int _maxFileSize = 2048000;
        private string _reader = "";

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

        private async Task LoadFile(InputFileChangeEventArgs e) {
            _reader = await new StreamReader(e.File.OpenReadStream(_maxFileSize)).ReadToEndAsync();
            await Layout.AddMessage("File loaded for " + SelectedOption + " and ready to be uploaded");
        }

        private async Task Upload() {
            if (!string.IsNullOrWhiteSpace(_reader)) {
                await Layout.AddMessage("JSON file being prepared -- this may take a while.");
                var source = await Layout.CheckSource();

                Enum.TryParse(SelectedOption, out UrlTypes urlType);
                await Layout.AddMessage(await JsonHelper.LoadJson(source, urlType, _reader));
            }
        }
    }
}