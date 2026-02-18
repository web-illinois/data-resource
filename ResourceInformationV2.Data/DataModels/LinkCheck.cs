using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResourceInformationV2.Data.DataModels {
    public class LinkCheck : BaseDataItem {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public override int Id { get; set; }
        public string EditLink { get; set; } = "";
        public string Title { get; set; } = "";
        public virtual Source? Source { get; set; }
        public string ItemGuid { get; set; } = "";

        public int SourceId { get; set; }

        public string Url { get; set; } = "";

        public DateTime? DateChecked { get; set; }
        public string ResponseStatusCode { get; set; } = "";
        public string ResponseMessage { get; set; } = "";

        public bool IsSuccessful { get; set; }
    }
}
