using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResourceInformationV2.Data.DataModels {

    public class Log : BaseDataItem {
        public CategoryType CategoryType { get; set; }
        public string ChangedByNetId { get; set; } = "";
        public string ChangeType { get; set; } = "";
        public string Data { get; set; } = "";
        public string DateCreated => LastUpdated.ToString("g");
        public bool EmailSent { get; set; }
        public FieldType FieldType { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public override int Id { get; set; }

        public virtual Source Source { get; set; } = default!;

        public int SourceId { get; set; }

        public string SubjectId { get; set; } = "";

        public string Title { get; set; } = "";
    }
}