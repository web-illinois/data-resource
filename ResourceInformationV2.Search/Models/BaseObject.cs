using OpenSearch.Client;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ResourceInformationV2.Search.Models {

    public abstract class BaseObject {
        protected static readonly string _editLink = "https://resource.wigg.illinois.edu/quicklink/";
        private static readonly string[] _badHtmlItems = ["<br>", "<p></p>", "<p><br></p>", "<p>&nbsp;</p>", "<p> </p>", "&nbsp;"];
        private readonly JsonSerializerOptions _serializer = new() { PropertyNamingPolicy = new JsonNamingPolicyLowerCase() };

        [Keyword]
        public IEnumerable<string> AudienceList { get; set; } = default!;

        public DateTime CreatedOn { get; set; }

        [Keyword]
        public IEnumerable<string> DepartmentList { get; set; } = default!;
        public virtual string Description { get; set; } = "";

        [Keyword]
        public abstract string EditLink { get; }

        [Keyword]
        public string Fragment { get; set; } = "";

        [Keyword]
        public string Id { get; set; } = "";

        public string Image { get; set; } = "";

        public string ImageAltText { get; set; } = "";

        public string ImageSource { get; set; } = "";

        public bool IsActive { get; set; }

        public bool IsDraftAvailable { get; set; } = false;

        [JsonIgnore]
        public virtual bool IsIdValid => !string.IsNullOrWhiteSpace(Id) && !string.IsNullOrWhiteSpace(Source) && (Id.StartsWith(Source + "-") || Id.StartsWith(Source + "!-"));

        public bool IsNewerDraft => Id.StartsWith(Source + "!-");
        public DateTime LastUpdated { get; set; }

        public IEnumerable<Link> LinkList { get; set; } = default!;

        [JsonIgnore]
        public abstract string NameType { get; }

        public string Notes { get; set; } = "";
        public int Order { get; set; }
        public string PrivateNotes { get; set; } = "";
        public DateTime? ReviewDate { get; set; }

        [Keyword]
        public string ReviewEmail { get; set; } = "";

        [Keyword]
        public string Source { get; set; } = "";

        [Keyword]
        public IEnumerable<string> Tag2List { get; set; } = default!;

        [Keyword]
        public IEnumerable<string> Tag3List { get; set; } = default!;

        [Keyword]
        public IEnumerable<string> Tag4List { get; set; } = default!;

        [Keyword]
        public IEnumerable<string> TagList { get; set; } = default!;

        public virtual string Title { get; set; } = "";

        [Keyword]
        public string TitleSortKeyword => Title;

        [Keyword]
        public IEnumerable<string> TopicList { get; set; } = default!;

        [Keyword]
        public string Url { get; set; } = "";

        [Keyword]
        public string VideoUrl { get; set; } = "";

        internal abstract string[] Headings { get; }

        public static string CleanHtml(string s) => string.IsNullOrWhiteSpace(s) || _badHtmlItems.Contains(s) ? string.Empty : s.Replace(" style=", " data-style=", StringComparison.OrdinalIgnoreCase);

        public static string ConvertVideoToEmbed(string href) {
            if (string.IsNullOrWhiteSpace(href)) {
                return string.Empty;
            } else if (href.Contains("youtube", StringComparison.InvariantCultureIgnoreCase) || href.Contains("youtu.be", StringComparison.InvariantCultureIgnoreCase)) {
                href = href.Trim('/').Replace("https://www.youtube.com/watch?v=", string.Empty).Replace("http://www.youtube.com/watch?v=", string.Empty).Replace("https://youtu.be/", string.Empty).Replace("https://www.youtube.com/embed/", string.Empty).Replace("http://www.youtube.com/embed/", string.Empty);
                return $"https://www.youtube.com/embed/{href}";
            } else if (href.Contains("mediaspace", StringComparison.InvariantCultureIgnoreCase) && !href.Contains("embed", StringComparison.InvariantCultureIgnoreCase)) {
                href = href.Trim('/').Split('/').Last();
                return $"https://mediaspace.illinois.edu/embed/secure/iframe/entryId/{href}/uiConfId/26883701";
            }
            return href;
        }

        public static string ProcessTagName(string tag) => tag.Replace("\"", "").Trim();

        public virtual void CleanHtmlFields() {
        }

        public virtual GenericItem GetGenericItem() => new() { Id = Id, IsActive = IsActive, IsNewerDraft = IsNewerDraft, Order = Order, Title = Title, EditLink = EditLink };

        public (bool successful, bool headerIssue, string message) LoadFromString(string source, string line) {
            var array = line.Split('\t');
            if (array.Length == 0 || (array.Length == 1 && string.IsNullOrWhiteSpace(array[0]))) {
                return (false, true, string.Empty);
            }
            if (array.Length != Headings.Length) {
                return (false, true, $"Expecting {Headings.Length} and received {array.Length} items");
            }
            if (array[0].Equals("id", StringComparison.OrdinalIgnoreCase) || array[1].Equals("title", StringComparison.OrdinalIgnoreCase)) {
                return (false, true, string.Empty);
            }
            if (string.IsNullOrWhiteSpace(array[1])) {
                return (false, false, string.Empty);
            }
            if (!string.IsNullOrWhiteSpace(array[0]) && !array[0].StartsWith(source + "-", StringComparison.OrdinalIgnoreCase)) {
                array[0] = source + "-" + array[0];
            }
            Source = source.Trim();
            LastUpdated = DateTime.Now;
            return (LoadFromStringPrivate(array), false, string.Empty);
        }

        public virtual void Prepare() {
            if (CreatedOn == default) {
                CreatedOn = DateTime.Now;
            }
            LastUpdated = DateTime.Now;
            Id = string.IsNullOrWhiteSpace(Id) ? Source + "-" + Guid.NewGuid().ToString() : Id;
            if (!Id.StartsWith(Source + "-") && !Id.StartsWith(Source + "!-")) {
                throw new ArgumentException("ID needs to start with source");
            }
            Fragment = string.IsNullOrWhiteSpace(Fragment) ? "" : new string([.. Fragment.Where(c => char.IsLetterOrDigit(c) || c == ' ' || c == '-' || c == '/')]).Replace(" ", "-").ToLowerInvariant();
            AudienceList = AudienceList == null ? [] : AudienceList.Select(ProcessTagName).ToList();
            DepartmentList = DepartmentList == null ? [] : DepartmentList.Select(ProcessTagName).ToList();
            TopicList = TopicList == null ? [] : TopicList.Select(ProcessTagName).ToList();
            TagList = TagList == null ? [] : TagList.Select(ProcessTagName).ToList();
            Tag2List = Tag2List == null ? [] : Tag2List.Select(ProcessTagName).ToList();
            Tag3List = Tag3List == null ? [] : Tag3List.Select(ProcessTagName).ToList();
            Tag4List = Tag4List == null ? [] : Tag4List.Select(ProcessTagName).ToList();
            LinkList = LinkList ?? [];
            CleanHtmlFields();
        }

        public virtual void PrepareForExport() => PrivateNotes = "";

        public override string ToString() => JsonSerializer.Serialize(this, _serializer);

        internal static List<Link> GetLinksFromString(string s) => [.. s.Split(';').Where(t => !string.IsNullOrWhiteSpace(t)).Select(Link.Translate)];

        internal static List<string> GetTagsFromString(string s) => [.. s.Split(';').Select(ProcessTagName).Where(t => !string.IsNullOrWhiteSpace(t)).Distinct()];

        internal abstract bool LoadFromStringPrivate(string[] lineArray);

        internal abstract string[] SaveToStringPrivate();
    }
}