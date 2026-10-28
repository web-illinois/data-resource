# Viewing Drafts and Tag Use

Two pages under **Review** help you see the state of your source: **View Drafts** and **Review Tag Use**.

## View Drafts (`/review/viewdrafts`)
This page lists all drafts for all item types in the active source.

1. Open **Review > View Drafts**.
2. Browse the list, which is sorted alphabetically by title.
3. Select a title to open that item's **technical** page, where you can review and change its status.

Notes:
- If there are no drafts, the page shows "No drafts are created."
- An entry marked **(newer draft)** has a draft version that is newer than the published version.
- Items can be in draft because they were added through the API (which defaults to Draft), were moved back to draft, or failed a [link check](11-link-checks.md).
- The list is generated each time you open the page, so it always shows the current drafts.

## Review Tag Use (`/review/taguse`)
This page lists the tags that are used in your source, grouped by filter group (for example, Audience, Topic, or a generic tag group). Each group shows its title and the tags in it.

Use it to:
- confirm which tags are in use before you rename or remove a tag;
- spot misspelled, duplicated, or unneeded tags;
- check tag names before an [import](09-importing-and-exporting.md), since tag names are case-sensitive.

To change tags, go to **Configuration > Manage Filters**. See [Filters and Categories](04-filters-and-categories.md).
