using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using ResourceInformationV2.Components.Layout;
using ResourceInformationV2.Data.Cache;
using ResourceInformationV2.Data.DataHelpers;
using ResourceInformationV2.Data.PageList;
using ResourceInformationV2.Helpers;

namespace ResourceInformationV2.Components.Pages.Configuration {

    public partial class Security {
        public bool ApiDraft { get; set; } = false;

        public string ApiGuid { get; set; } = "";

        public DateTime LastDateApiChanged { get; set; }

        [CascadingParameter]
        public SidebarLayout Layout { get; set; } = default!;

        public List<string> NetIds { get; set; } = [];
        public string NewNetId { get; set; } = "";
        public bool UseApi { get; set; } = false;

        [Inject]
        protected ApiHelper ApiHelper { get; set; } = default!;

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

        public async Task CreateApi() {
            ApiGuid = await ApiHelper.AdvanceApi(await Layout.CheckSource());
            UseApi = true;
            ApiDraft = true;
            LastDateApiChanged = DateTime.Now;
        }

        public async Task Draft(ChangeEventArgs args) {
            var force = bool.Parse(args.Value?.ToString() ?? "false");
            _ = await ApiHelper.SetApiToDraft(await Layout.CheckSource(), force);
            await Layout.AddMessage("The API draft has been changed");
        }

        public async Task InvalidateApi() {
            _ = await ApiHelper.InvalidateApi(await Layout.CheckSource());
            ApiGuid = "";
            UseApi = false;
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
            (UseApi, LastDateApiChanged, ApiDraft) = await ApiHelper.GetApi(source);
            Layout.SetSidebar(SidebarEnum.Configuration, "Configuration");
        }
    }
}