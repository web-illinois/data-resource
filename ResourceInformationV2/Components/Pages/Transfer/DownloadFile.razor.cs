using System.Text;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ResourceInformationV2.Components.Layout;
using ResourceInformationV2.Data.DataHelpers;
using ResourceInformationV2.Data.PageList;
using ResourceInformationV2.Search.Getters;
using ResourceInformationV2.Search.Models;

namespace ResourceInformationV2.Components.Pages.Transfer {

    public partial class DownloadFile {
        public List<string> Options = new();

        [CascadingParameter]
        public SidebarLayout Layout { get; set; } = default!;

        public string SelectedOption { get; set; } = "";

        [Inject]
        protected EventGetter EventGetter { get; set; } = default!;

        [Inject]
        protected FaqGetter FaqGetter { get; set; } = default!;

        [Inject]
        protected FilterHelper FilterHelper { get; set; } = default!;

        [Inject]
        protected IJSRuntime JsRuntime { get; set; } = default!;

        [Inject]
        protected NavigationManager NavigationManager { get; set; } = default!;

        [Inject]
        protected NoteGetter NoteGetter { get; set; } = default!;

        [Inject]
        protected PersonGetter PersonGetter { get; set; } = default!;

        [Inject]
        protected PublicationGetter PublicationGetter { get; set; } = default!;

        [Inject]
        protected ResourceGetter ResourceGetter { get; set; } = default!;

        [Inject]
        protected SourceHelper SourceHelper { get; set; } = default!;

        protected override async Task OnInitializedAsync() {
            Layout.SetSidebar(SidebarEnum.Transfer, "Transfer Items");
            var sourceCode = await Layout.CheckSource();
            Options = [.. (await SourceHelper.DoesSourceUseItemCheckAllAndConcatenate(sourceCode))];
            SelectedOption = Options.First();
        }

        private async Task Download() {
            await Layout.AddMessage("Text file being prepared -- this may take a while.");
            var source = await Layout.CheckSource();
            var text = "";
            _ = Enum.TryParse(SelectedOption, out UrlTypes urlType);

            switch (urlType) {
                case UrlTypes.People:
                    text = await PersonGetter.DownloadFile(source);
                    break;

                case UrlTypes.Publications:
                    text = await PublicationGetter.DownloadFile(source);
                    break;

                case UrlTypes.Resources:
                    text = await ResourceGetter.DownloadFile(source);
                    break;

                case UrlTypes.Faqs:
                    text = await FaqGetter.DownloadFile(source);
                    break;

                case UrlTypes.Notes:
                    text = await NoteGetter.DownloadFile(source);
                    break;

                case UrlTypes.Events:
                    text = await EventGetter.DownloadFile(source);
                    break;

                default:
                    break;
            }
            var fileStream = new MemoryStream(Encoding.ASCII.GetBytes(text));
            using var streamRef = new DotNetStreamReference(fileStream);
            await JsRuntime.InvokeVoidAsync("downloadFileFromStream", $"{source}_{DateTime.Now.ToString("yyyy_MM_dd")}_{SelectedOption.ToLowerInvariant()}.txt", streamRef);
            await Layout.AddMessage("Text file downloaded successfully.");
        }
    }
}