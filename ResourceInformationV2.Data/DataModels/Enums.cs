namespace ResourceInformationV2.Data.DataModels
{

    public enum CategoryType
    {
        None,
        Person,
        Publication,
        Resource,
        Faq,
        Note,
        Event
    }

    public enum EmailType
    {
        None,
        OnSubmission,
        OnPublication,
        OnDraft
    }

    public enum FieldType
    {
        None,
        General,
        ImageAndVideo,
        Specific,
        Filters,
        Links,
        Technical
    }

    public enum TagType
    {
        None,
        Tag1,
        Tag2,
        Tag3,
        Tag4,
        Topic,
        Audience,
        Department
    }
}