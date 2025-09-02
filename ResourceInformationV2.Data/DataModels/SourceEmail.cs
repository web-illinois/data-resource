using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResourceInformationV2.Data.DataModels {

    public class SourceEmail : BaseDataItem {
        public string Body { get; set; } = "";

        public string BodyText { get; set; } = "";

        public string Cc { get; set; } = "";

        public EmailType EmailType { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public override int Id { get; set; }

        public string ReplyTo { get; set; } = "";
        public bool SendToReviewEmail { get; set; }
        public virtual Source? Source { get; set; }
        public int SourceId { get; set; }
        public string Subject { get; set; } = "";
        public string To { get; set; } = "";

        public void Add(string s) {
            if (!string.IsNullOrEmpty(s)) {
                To = string.IsNullOrWhiteSpace(To) ? s.Trim() : To + ";" + s.Trim();
            }
        }
    }
}