using OpenSearch.Client;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResourceInformationV2.Data.DataModels {
    public class EventImport : BaseDataItem {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public override int Id { get; set; }

        public bool IsDefaultAddAll { get; set; }

        public virtual Source Source { get; set; } = default!;

        public int SourceId { get; set; }

        public DateTime StartDate { get; set; }

        public string TitleExceptions { get; set; } = "";

        [Ignore]
        public IEnumerable<string> TitleExceptionList => TitleExceptions.Split("[-]").Select(t => t.Trim()).Where(t => !string.IsNullOrEmpty(t));

        public TagType TagType { get; set; }
        public string TagText { get; set; } = "";
    }
}
