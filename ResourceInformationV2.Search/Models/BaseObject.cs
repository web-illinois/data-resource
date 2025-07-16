using System.Text.Json;
using System.Text.Json.Serialization;
using OpenSearch.Client;

namespace ResourceInformationV2.Search.Models {

    public abstract class BaseObject {
        protected static readonly string _editLink = "https://resource.itpartners.illinois.edu/quicklink/";
        private static readonly string[] _badHtmlItems = ["<br>", "<p></p>", "<p><br></p>", "<p>&nbsp;</p>", "<p> </p>", "&nbsp;"];

        [Keyword]
        public IEnumerable<string> AudienceList { get; set; } = default!;

        public DateTime CreatedOn { get; set; }

        public abstract string EditLink { get; }

        [Keyword]
        public string Fragment { get; set; } = "";

        [Keyword]
        public string Id { get; set; } = "";

        [JsonIgnore]
        public virtual string InternalTitle => Title;

        public bool IsActive { get; set; }

        [JsonIgnore]
        public virtual bool IsIdValid => !string.IsNullOrWhiteSpace(Id) && !string.IsNullOrWhiteSpace(Source) && Id.StartsWith(Source + "-");

        public DateTime LastUpdated { get; set; }

        public IEnumerable<Link> LinkList { get; set; } = default!;

        public int Order { get; set; }

        [Keyword]
        public string Source { get; set; } = "";

        [Keyword]
        public IEnumerable<string> Tag2List { get; set; } = default!;

        public string Tag2Name { get; set; } = "";

        [Keyword]
        public IEnumerable<string> Tag3List { get; set; } = default!;

        public string Tag3Name { get; set; } = "";

        [Keyword]
        public IEnumerable<string> Tag4List { get; set; } = default!;

        public string Tag4Name { get; set; } = "";

        [Keyword]
        public IEnumerable<string> TagList { get; set; } = default!;

        public string TagName { get; set; } = "";

        public string Title { get; set; } = "";

        [Keyword]
        public string TitleSortKeyword => Title;

        [Keyword]
        public IEnumerable<string> TopicList { get; set; } = default!;

        internal virtual string CreateId => Source + "-" + Guid.NewGuid().ToString();

        public static string CleanHtml(string s) => string.IsNullOrWhiteSpace(s) || _badHtmlItems.Contains(s) ? string.Empty : s.Replace(" style=", " data-style=", StringComparison.OrdinalIgnoreCase);

        public static string ProcessTagName(string tag) => tag.Replace("\"", "");

        public virtual void CleanHtmlFields() {
        }

        public virtual GenericItem GetGenericItem() => new() { Id = Id, IsActive = IsActive, Order = Order, Title = InternalTitle };

        public virtual void Prepare() {
            LastUpdated = DateTime.Now;
            SetId();
            SetFragment();
            CleanHtmlFields();
        }

        public virtual void SetFragment() => Fragment = string.IsNullOrWhiteSpace(Fragment) ? "" : new string([.. Fragment.Where(c => char.IsLetterOrDigit(c) || c == ' ' || c == '-' || c == '/')]).Replace(" ", "-").ToLowerInvariant();

        public virtual void SetId() => Id = string.IsNullOrWhiteSpace(Id) ? CreateId : Id;

        public override string ToString() => JsonSerializer.Serialize(this);

        internal void ProcessLists() {
            AudienceList = AudienceList == null ? [] : AudienceList.Select(ProcessTagName).ToList();
            TopicList = TopicList == null ? [] : TopicList.Select(ProcessTagName).ToList();
            TagList = TagList == null ? [] : TagList.Select(ProcessTagName).ToList();
            Tag2List = Tag2List == null ? [] : Tag2List.Select(ProcessTagName).ToList();
            Tag3List = Tag3List == null ? [] : Tag3List.Select(ProcessTagName).ToList();
            Tag4List = Tag4List == null ? [] : Tag4List.Select(ProcessTagName).ToList();
        }
    }
}