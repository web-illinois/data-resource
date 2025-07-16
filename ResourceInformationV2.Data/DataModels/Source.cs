using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResourceInformationV2.Data.DataModels {

    public class Source : BaseDataItem {
        public string BaseUrl { get; set; } = "";
        public string Code { get; set; } = "";
        public string CreatedByEmail { get; set; } = "";
        public bool DeactivateOnReview { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public override int Id { get; set; }

        public bool IsTest { get; set; } = false;
        public int NumberOfDaysForReview { get; set; }
        public string ReviewEmail { get; set; } = "";
        public string SecurityKey { get; set; } = "";
        public string SecurityKeyAlternate { get; set; } = "";
        public DateTime SecurityKeyChangeDate { get; set; }
        public string Title { get; set; } = "";
        public string UrlTemplate { get; set; } = "";
        public bool UseFaqs { get; set; }
        public bool UseNotes { get; set; }
        public bool UsePeople { get; set; }
        public bool UsePublications { get; set; }
        public bool UseResources { get; set; }
    }
}