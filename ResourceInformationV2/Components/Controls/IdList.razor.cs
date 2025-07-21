using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace ResourceInformationV2.Components.Controls {

    public partial class IdList {

        [Parameter]
        public string Description { get; set; } = "";

        [Parameter]
        public string Id { get; set; } = "";

        [Parameter]
        public string Title { get; set; } = "";

        [Inject]
        protected IJSRuntime JsRuntime { get; set; } = default!;

        public async Task CopyToClipboard() {
            _ = await JsRuntime.InvokeAsync<bool>("copyToClipboard", Id);
            _ = await JsRuntime.InvokeAsync<bool>("alertOnScreen", Title + " copied to clipboard and can be pasted to your document");
        }
    }
}