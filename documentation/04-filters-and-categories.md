# Filters and Categories

Filters tag an item so users can search and narrow results by category.

## Apply filters
1. Open an item and select **Filters** in the sidebar.
2. Toggle the tags that apply. Available groups include Audience, Department, Topic, and up to four additional tag groups (Tag 1–4), depending on how your source is configured.
3. Select **Save**.

## Notes
- Department filters may be disabled for item types that do not use them.
- Changing a filter name in configuration affects every item that uses it.

## Managing filters (Configuration > Manage Filters)
Audience, topics, and tags are keywords configured per source and used as filters on items.

### Preset filter groups
Three groups are "preset" to standardize keywords across sources. If one of your keywords resembles a preset, use it instead of a generic tag.
- **Audience**: who the item is appropriate for, such as student / staff / faculty or first grade / second grade.
- **Topic**: the general subject of the item. This depends on your needs.
- **Department**: the department or group the item belongs to. It is mainly used for internal documentation and for permissions (see [Security](10-security.md)).

There are also four generic groups: **Generic Tag 1** through **Generic Tag 4**. Use them for your own filters, such as Grade Level, Skill, or Format.

### Edit a filter group
1. Open **Configuration > Manage Filters**.
2. Choose a group under **Type**.
3. Enter the **Filter Title**, the name users see (for example, "Grade Level"). The title is required for the group to be used, even for preset groups.
4. Under **Filters**, edit existing entries. Each entry can be moved up, moved down, or removed.
5. To add an entry, type its name under **Add New Filter** and select **Add**.
6. Select **Save**.

Removing an entry takes effect only after you save. Review the effect on items that already use the tag.

### Order of filter groups
Under **Filter Names and Order**, move groups up or down to set the order in which they are returned when you pull filters through the API. Select **Save Group Order** to keep the change. This is saved separately from the filter lists above.

### Tips
- Tag names are case-sensitive when importing files. See [Importing and Exporting Data](09-importing-and-exporting.md).
- Save each filter group before choosing another type, or your edits may be lost.
