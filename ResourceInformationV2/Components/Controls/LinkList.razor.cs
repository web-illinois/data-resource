using Microsoft.AspNetCore.Components;
using ResourceInformationV2.Components.Layout;
using ResourceInformationV2.Data.DataHelpers;
using ResourceInformationV2.Search.Models;

namespace ResourceInformationV2.Components.Controls {

    public partial class LinkList {
        public string AddLinkButtonTitle { get; set; } = "Add Link";

        [CascadingParameter]
        public SidebarLayout Layout { get; set; } = default!;

        [Parameter]
        public List<Link> Links { get; set; } = [];

        public string NewLinkTitle { get; set; } = "";

        public string NewLinkUrl { get; set; } = "";

        public string OldTitle { get; set; } = "";

        public void AddLink() {
            if (Links == null) {
                Links = [];
            }
            if (string.IsNullOrWhiteSpace(NewLinkTitle) || string.IsNullOrWhiteSpace(NewLinkUrl)) {
                if (Layout != null)
                    Layout.AddMessage("Save failed -- Please enter a title and URL for the link.");
            } else if (OldTitle != NewLinkTitle && Links.Any(l => l.Title == NewLinkTitle)) {
                if (Layout != null)
                    Layout.AddMessage("Save failed -- The title cannot be duplicated in a list of links");
            } else if (string.IsNullOrWhiteSpace(OldTitle)) {
                Links.Add(new Link { LinkHref = NewLinkUrl, Title = NewLinkTitle, Order = Links.Count + 1 });
            } else if (Links.Any(l => l.Title == OldTitle)) {
                Links.Find(l => l.Title == OldTitle).LinkHref = NewLinkUrl;
                Links.Find(l => l.Title == OldTitle).Title = NewLinkTitle;
            }
            OldTitle = "";
            NewLinkTitle = "";
            NewLinkUrl = "";
            AddLinkButtonTitle = "Add Link";
            if (Layout != null)
                Layout.SetDirty();
        }

        public void Down(Link i) {
            Links = ListExtentions.MoveItemDown(Links, i);
            StateHasChanged();
            if (Layout != null)
                Layout.SetDirty();
        }

        public void Edit(Link i) {
            NewLinkTitle = i.Title;
            NewLinkUrl = i.LinkHref;
            OldTitle = i.Title;
            AddLinkButtonTitle = "Edit Link";
            if (Layout != null)
                Layout.SetDirty();
        }

        public List<Link> GetSavedLinks() {
            var i = 1;
            foreach (var l in Links) {
                l.Order = i++;
            }
            return Links;
        }

        public void Remove(Link i) {
            Links.Remove(i);
            StateHasChanged();
            if (Layout != null)
                Layout.SetDirty();
        }

        public void Up(Link i) {
            Links = ListExtentions.MoveItemUp(Links, i);
            StateHasChanged();
            if (Layout != null)
                Layout.SetDirty();
        }
    }
}