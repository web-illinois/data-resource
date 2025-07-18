using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using ResourceInformationV2.Components.Layout;
using ResourceInformationV2.Data.Cache;
using ResourceInformationV2.Data.DataHelpers;
using ResourceInformationV2.Data.PageList;
using ResourceInformationV2.Helpers;

namespace ResourceInformationV2.Components.Pages.Configuration {

    public partial class Security {

        [CascadingParameter]
        public SidebarLayout Layout { get; set; } = default!;

        public List<string> NetIds { get; set; } = [];
        public string NewNetId { get; set; } = "";

        [Inject]
        protected AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;

        [Inject]
        protected CacheHolder CacheHolder { get; set; } = default!;

        [Inject]
        protected NavigationManager NavigationManager { get; set; } = default!;

        [Inject]
        protected SecurityHelper SecurityHelper { get; set; } = default!;

        public async Task Add() {
            if (!string.IsNullOrWhiteSpace(NewNetId)) {
                var source = await Layout.CheckSource();
                var newId = await SecurityHelper.AddName(source, NewNetId);
                if (!string.IsNullOrWhiteSpace(newId)) {
                    NetIds.Add(newId);
                }
                NewNetId = "";
            }
        }

        public async Task Remove(string netId) {
            var source = await Layout.CheckSource();
            if (await SecurityHelper.RemoveName(source, netId)) {
                NetIds.Remove(netId);
            }
            var email = await UserHelper.GetUser(AuthenticationStateProvider);
            if (netId == email) {
                CacheHolder.ClearCache(email);
                NavigationManager.NavigateTo("/", true);
            }
            NewNetId = "";
        }

        protected override async Task OnInitializedAsync() {
            await base.OnInitializedAsync();
            var source = await Layout.CheckSource();
            NetIds = await SecurityHelper.GetNames(source);
            Layout.SetSidebar(SidebarEnum.Configuration, "Configuration");
        }
    }
}