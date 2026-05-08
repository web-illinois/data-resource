using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using ResourceInformationV2.Components.Layout;
using ResourceInformationV2.Data.Cache;
using ResourceInformationV2.Data.DataHelpers;
using ResourceInformationV2.Data.DataModels;
using ResourceInformationV2.Data.PageList;
using ResourceInformationV2.Helpers;

namespace ResourceInformationV2.Components.Pages.Configuration {

    public partial class Security {
        public bool ApiDraft { get; set; } = false;

        public string ApiGuid { get; set; } = "";

        public int CurrentOwnerId { get; set; }
        public List<string> Departments { get; set; } = [];
        public DateTime LastDateApiChanged { get; set; }

        [CascadingParameter]
        public SidebarLayout Layout { get; set; } = default!;

        public string NewDepartment { get; set; } = "";
        public List<SecurityEntry> NetIds { get; set; } = [];
        public string NewNetId { get; set; } = "";
        public bool UseApi { get; set; } = false;

        [Inject]
        protected ApiHelper ApiHelper { get; set; } = default!;

        [Inject]
        protected AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;

        [Inject]
        protected CacheHolder CacheHolder { get; set; } = default!;

        [Inject]
        protected FilterHelper FilterHelper { get; set; } = default!;

        [Inject]
        protected NavigationManager NavigationManager { get; set; } = default!;

        [Inject]
        protected SecurityHelper SecurityHelper { get; set; } = default!;

        public async Task Add() {
            if (!string.IsNullOrWhiteSpace(NewNetId)) {
                var source = await Layout.CheckSource();
                var newId = await SecurityHelper.AddName(source, NewNetId, NewDepartment);
                if (!string.IsNullOrWhiteSpace(newId)) {
                    NetIds.Add(new SecurityEntry {
                        Email = newId,
                        DepartmentTag = NewDepartment
                    });
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

        public async Task ChangeOwner() {
            var source = await Layout.CheckSource();
            if (await SecurityHelper.ChangeOwner(source, CurrentOwnerId)) {
                if (NetIds.Any(a => a.IsOwner) && NetIds.Any(a => a.Id == CurrentOwnerId)) {
                    NetIds.First(a => a.IsOwner).IsOwner = false;
                    NetIds.First(a => a.Id == CurrentOwnerId).IsOwner = true;
                }
                await Layout.AddMessage("The Owner has been changed");
            }
        }

        public async Task Remove(string netId) {
            var source = await Layout.CheckSource();
            if (await SecurityHelper.RemoveName(source, netId)) {
                NetIds.Remove(NetIds.FirstOrDefault(n => n.Email == netId) ?? new SecurityEntry());
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
            _ = await Layout.ConfirmDepartmentName(true);
            NetIds = await SecurityHelper.GetNames(source);
            (UseApi, LastDateApiChanged, ApiDraft) = await ApiHelper.GetApi(source);
            Layout.SetSidebar(SidebarEnum.Configuration, "Configuration");
            CurrentOwnerId = NetIds.FirstOrDefault(n => n.IsOwner)?.Id ?? 0;
            Departments = [.. (await FilterHelper.GetFilters(source, TagType.Department)).TagSources.Select(ts => ts.Title)];
        }
    }
}