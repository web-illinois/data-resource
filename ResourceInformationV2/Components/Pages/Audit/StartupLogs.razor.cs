using Microsoft.AspNetCore.Components;
using ResourceInformationV2.Data.DataHelpers;
using ResourceInformationV2.Data.DataModels;

namespace ResourceInformationV2.Components.Pages.Audit {
    public partial class StartupLogs {
        [Inject]
        public LogHelper LogHelper { get; set; } = default!;

        public IEnumerable<StartupLog> LogItems { get; set; } = [];

        protected override async Task OnInitializedAsync() {
            await base.OnInitializedAsync();
            LogItems = await LogHelper.GetStartupLog();
        }
    }
}
