using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResourceInformationV2.Data.DataModels {

    public class Instruction : BaseDataItem {
        public CategoryType CategoryType { get; set; }
        public FieldType FieldType { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public override int Id { get; set; }

        public virtual Source Source { get; set; } = default!;

        public int SourceId { get; set; }
        public string Text { get; set; } = "";
    }
}