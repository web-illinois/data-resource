using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using ResourceInformationV2.Components.Layout;
using ResourceInformationV2.Data.Cache;
using ResourceInformationV2.Data.DataHelpers;
using ResourceInformationV2.Data.PageList;
using ResourceInformationV2.Helpers;
using ResourceInformationV2.Search.Helpers;

namespace ResourceInformationV2.Components.Pages.Configuration {

    public partial class Deletion {
        private string _sourceCode = "";

        [Inject]
        public AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;

        [Inject]
        public BulkEditor BulkEditor { get; set; } = default!;

        [Inject]
        protected CacheHolder CacheHolder { get; set; } = default!;
        [Inject]
        public NavigationManager NavigationManager { get; set; } = default!;
        [CascadingParameter]
        public SidebarLayout Layout { get; set; } = default!;

        [Inject]
        public SourceHelper SourceHelper { get; set; } = default!;

        protected async Task DeleteItems() {
            await Layout.AddMessage(await BulkEditor.DeleteAllItems(_sourceCode));
        }

        protected async Task DeleteSource() {
            var message = await BulkEditor.DeleteAllItems(_sourceCode);
            message += await SourceHelper.DeleteSource(_sourceCode, await UserHelper.GetUser(AuthenticationStateProvider));
            var email = await UserHelper.GetUser(AuthenticationStateProvider);
            CacheHolder.ClearCache(email);
            NavigationManager.NavigateTo("/");
        }

        protected override async Task OnInitializedAsync() {
            _sourceCode = await Layout.CheckSource();
            Layout.SetSidebar(SidebarEnum.Configuration, "Configuration");
            base.OnInitialized();
        }
    }
}