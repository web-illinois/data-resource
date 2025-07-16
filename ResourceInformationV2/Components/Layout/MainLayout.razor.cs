using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace ResourceInformationV2.Components.Layout {

    public partial class MainLayout {

        [Inject]
        protected IJSRuntime JsRuntime { get; set; } = default!;

        protected override async Task OnAfterRenderAsync(bool firstRender) {
            if (firstRender) {
                _ = await JsRuntime.InvokeAsync<bool>("blazorMenu");
            }
        }
    }
}