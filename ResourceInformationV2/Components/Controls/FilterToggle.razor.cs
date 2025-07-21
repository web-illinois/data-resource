using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components;
using ResourceInformationV2.Components.Layout;
using ResourceInformationV2.Data.DataModels;

namespace ResourceInformationV2.Components.Controls {

    public partial class FilterToggle {

        [Parameter]
        public Tag Filter { get; set; } = default!;

        public string FilterId => Regex.Replace(Filter.Title, "[^A-Za-z0-9 -]", "").ToLowerInvariant();

        [CascadingParameter]
        public required SidebarLayout Layout { get; set; }
    }
}