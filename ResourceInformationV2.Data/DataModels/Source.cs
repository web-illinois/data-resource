using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResourceInformationV2.Data.DataModels {

    public class Source : BaseDataItem {
        public string ApiSecretCurrent { get; set; } = "";
        public DateTime? ApiSecretLastChanged { get; set; }
        public string ApiSecretPrevious { get; set; } = "";
        public string BaseUrl { get; set; } = "";
        public string Code { get; set; } = "";
        public string CreatedByEmail { get; set; } = "";
        public bool DeactivateOnReview { get; set; }
        public string DeletedByEmail { get; set; } = "";
        public string FilterAudienceTitle { get; set; } = "";
        public string FilterDepartmentTitle { get; set; } = "";
        public string FilterOrder { get; set; } = "";
        public string FilterTag1Title { get; set; } = "";
        public string FilterTag2Title { get; set; } = "";
        public string FilterTag3Title { get; set; } = "";
        public string FilterTag4Title { get; set; } = "";
        public string FilterTopicTitle { get; set; } = "";
        public bool ForceApiToDraft { get; set; } = true;

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public override int Id { get; set; }

        public bool IsTest { get; set; } = false;
        public int NumberOfDaysForReview { get; set; }
        public string ReviewEmail { get; set; } = "";
        public string Title { get; set; } = "";
        public bool UseEvents { get; set; }
        public bool UseFaqs { get; set; }
        public bool UseNotes { get; set; }
        public bool UsePeople { get; set; }
        public bool UsePublications { get; set; }
        public bool UseResources { get; set; }
    }
}