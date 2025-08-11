using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;
using ResourceInformationV2.Data.Cache;
using ResourceInformationV2.Data.DataHelpers;
using ResourceInformationV2.Data.DataModels;
using ResourceInformationV2.Data.PageList;
using ResourceInformationV2.Helpers;
using ResourceInformationV2.Search.Models;

namespace ResourceInformationV2.Components.Layout {

    public partial class SidebarLayout {
        public bool IsDirty = false;
        public Breadcrumb BreadcrumbControl { get; set; } = default!;

        public Sidebar SidebarControl { get; set; } = default!;

        public string SourceCode { get; set; } = "";

        [Inject]
        protected AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;

        [Inject]
        protected CacheHolder CacheHolder { get; set; } = default!;

        [Inject]
        protected IJSRuntime JsRuntime { get; set; } = default!;

        [Inject]
        protected LogHelper LogHelper { get; set; } = default!;

        [Inject]
        protected NavigationManager NavigationManager { get; set; } = default!;

        public async Task AddMessage(string s) => _ = await JsRuntime.InvokeAsync<bool>("alertOnScreen", s);

        public async Task<string> CheckSource(bool redirectIfNoSource = true) {
            var source = CacheHolder.GetCacheSource(await AuthenticationStateProvider.GetUser());
            if (string.IsNullOrWhiteSpace(source) && redirectIfNoSource) {
                NavigationManager.NavigateTo("/?redirectIfNoSource=true");
            }
            return source ?? "";
        }

        public async Task ClearCacheId() => await SetCacheId("");

        public async Task<string> GetBaseUrl() {
            var cacheItem = CacheHolder.GetItem(await AuthenticationStateProvider.GetUser());
            return cacheItem?.BaseUrl ?? "";
        }

        public async Task<string> GetCachedId() {
            var cacheItem = CacheHolder.GetItem(await AuthenticationStateProvider.GetUser());
            return cacheItem?.ItemId ?? "";
        }

        public async Task<string> GetNetId() => await AuthenticationStateProvider.GetUser();

        public async Task Log(CategoryType categoryType, FieldType fieldType, BaseObject data, string subject = "") {
            _ = await LogHelper.Log(categoryType, fieldType, await GetNetId(), await CheckSource(), data, subject);
        }

        public async Task LogGeneric(CategoryType categoryType, string subject = "") {
            _ = await LogHelper.LogGeneric(categoryType, await GetNetId(), await CheckSource(), subject);
        }

        public void RemoveDirty() => IsDirty = false;

        public async Task RemoveMessage() => _ = await JsRuntime.InvokeAsync<bool>("removeAlertOnScreen");

        public async Task SetCacheId(string id) {
            var netid = await AuthenticationStateProvider.GetUser();
            CacheHolder.SetCacheItem(netid, id);
        }

        public void SetDirty() => IsDirty = true;

        public void SetSidebar(SidebarEnum s, string title, bool hideForNew = false) {
            SidebarControl.Rebuild(hideForNew ? SidebarEnum.None : s, title);
            BreadcrumbControl.Rebuild(s);
            IsDirty = false;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender) {
            if (firstRender) {
                _ = await JsRuntime.InvokeAsync<bool>("blazorMenu");
            }
        }

        protected override async Task OnInitializedAsync() {
            SourceCode = CacheHolder.GetCacheSource(await AuthenticationStateProvider.GetUser()) ?? "";
            await base.OnInitializedAsync();
        }

        private async Task LocationChangingHandler(LocationChangingContext arg) {
            if (IsDirty) {
                if (!(await JsRuntime.InvokeAsync<bool>("confirm", $"You have unsaved changes. Are you sure?"))) {
                    arg.PreventNavigation();
                }
            }
        }
    }
}