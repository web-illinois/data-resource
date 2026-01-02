using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResourceInformationV2.Data.DataModels {

    public class Tag : BaseDataItem {

        private static readonly Dictionary<TagType, string> _translator = new() {
            { TagType.None, "" },
            { TagType.Audience, "audienceList" },
            { TagType.Department, "departmentList" },
            { TagType.Topic, "topicList" },
            { TagType.Tag1, "tag1List" },
            { TagType.Tag2, "tag2List" },
            { TagType.Tag3, "tag3List" },
            { TagType.Tag4, "tag4List" }
        };

        [NotMapped]
        public bool EnabledBySource { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public override int Id { get; set; }

        [NotMapped]
        public string OldTitle { get; set; } = "";

        public int Order { get; set; }

        public virtual Source? Source { get; set; }

        public int SourceId { get; set; }

        public TagType TagType { get; set; }

        [NotMapped]
        public string TagTypeSourceName => _translator[TagType];

        public string Title { get; set; } = "";
    }
}