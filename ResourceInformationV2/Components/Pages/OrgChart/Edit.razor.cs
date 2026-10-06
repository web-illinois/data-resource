using Microsoft.AspNetCore.Components;
using ResourceInformationV2.Components.Layout;
using ResourceInformationV2.Data.DataHelpers;
using ResourceInformationV2.Data.DataModels;
using ResourceInformationV2.Data.PageList;
using ResourceInformationV2.Search.Models;

namespace ResourceInformationV2.Components.Pages.OrgChart {
    public partial class Edit {
        private string _sourceCode = "";
        private bool? _useItem;

        public string ParentText => VisibleOrgchartItem?.Parent?.Title + ", " + VisibleOrgchartItem?.Parent?.Subtitle;

        public string SearchText { get; set; } = "";

        public Orgchart CurrentOrgchart { get; set; } = default!;

        public List<Orgchart> VisibleOrgchartItems { get; set; } = [];

        public Orgchart VisibleOrgchartItem { get; set; } = default!;

        [CascadingParameter]
        public SidebarLayout Layout { get; set; } = default!;

        [Inject]
        protected NavigationManager NavigationManager { get; set; } = default!;

        [Inject]
        protected OrgChartHelper OrgChartHelper { get; set; } = default!;

        [Inject]
        protected SourceHelper SourceHelper { get; set; } = default!;

        public bool CanGoUpLevel => VisibleOrgchartItem.Parent is not null;

        protected void Search() {
            if (!string.IsNullOrWhiteSpace(SearchText)) {
                var newChild = Orgchart.Search(CurrentOrgchart, null, SearchText);
                if (newChild != null) {
                    GoDownLevel(newChild);
                }
            } else {
                VisibleOrgchartItem = CurrentOrgchart;
            }
            RefreshVisibleItems();
        }

        protected void AddChild() {
            VisibleOrgchartItem.Children ??= [];
            var child = new Orgchart {
                Parent = VisibleOrgchartItem
            };
            VisibleOrgchartItem.Children.Add(child);
            GoDownLevel(child);
            RefreshVisibleItems();
        }

        protected void DeleteChild(Orgchart child) {
            if (VisibleOrgchartItem.Children is null) {
                return;
            }
            _ = VisibleOrgchartItem.Children.Remove(child);
            RefreshVisibleItems();
        }

        protected void GoDownLevel(Orgchart child) {
            VisibleOrgchartItem = child;
            RefreshVisibleItems();
        }

        protected void GoUpLevel() {
            if (VisibleOrgchartItem.Parent != null) {
                VisibleOrgchartItem = VisibleOrgchartItem.Parent;
            }
            RefreshVisibleItems();
        }

        protected async Task Save() {
            SetParents(CurrentOrgchart, null);
            var saved = await OrgChartHelper.SaveOrgChartJson(_sourceCode, CurrentOrgchart.ToString());
            await Layout.AddMessage(saved ? "Org chart saved." : "Unable to save org chart.");
        }

        protected override async Task OnInitializedAsync() {
            Layout.SetSidebar(SidebarEnum.OrgChartItem, "Organizational Chart");
            _sourceCode = await Layout.CheckSource();
            _useItem = await SourceHelper.DoesSourceUseItem(_sourceCode, CategoryType.OrgChart);

            var json = await OrgChartHelper.GetOrgChartJson(_sourceCode);
            CurrentOrgchart = Orgchart.FromJson(json);

            SetParents(CurrentOrgchart, null);
            VisibleOrgchartItem = CurrentOrgchart;
            RefreshVisibleItems();

            await base.OnInitializedAsync();
        }

        private void RefreshVisibleItems() {
            VisibleOrgchartItem.Children ??= [];
            VisibleOrgchartItems = VisibleOrgchartItem.Children;
        }

        private static void SetParents(Orgchart node, Orgchart? parent) {
            node.Parent = parent;
            if (node.Children != null) {
                node.Children.ForEach(child => SetParents(child, node));
            }
        }
    }
}
