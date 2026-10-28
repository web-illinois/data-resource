# Adding Instructions for Editors

Source administrators can add custom instructions that appear at the top of the edit pages for each item type. Editors see them as a bordered red message beginning with "Instructions:". Instructions are stored per source, so each source can have its own wording.

## Where to find the Instruction pages
Open **Configuration** and select **Options and Instructions** in the sidebar. There is one page per item type:

| Item type | Page |
|-----------|------|
| Resources | `/configuration/instructions/resources` |
| Publications | `/configuration/instructions/publications` |
| Events | `/configuration/instructions/events` |
| People | `/configuration/instructions/people` |
| FAQs | `/configuration/instructions/faqs` |
| Notes | `/configuration/instructions/notes` |
| Organizational Chart | `/configuration/instructions/orgchart` |

Each page begins with a short description of the item type.

## Turn an item type on or off
Under **Would you like to add this item to your source?**, choose **Yes** or **No**. Only item types set to Yes appear on the home page and in **Add Items**. Select **Save** to apply the change.

## Write the instructions
For an enabled item type (other than the Org Chart), enter optional text in any of these boxes. Each box matches an area of the edit pages:

| Box | Shown on |
|-----|----------|
| General Area | General Information page |
| Image/Video Area | Image and Video page |
| Item-Specific Area | The page with fields specific to that type, such as publication or event fields (not available for Resources) |
| Filters Area | Filters page |
| Related Links Area | Related Links page |
| Technical Information | Technical page |

Leave a box empty to show no instructions on that page.

Steps:
1. Open the instruction page for the item type.
2. Make sure it is set to **Yes**.
3. Type the text in the boxes you want.
4. Select **Save**. The message "Information saved" confirms the change.

The text appears right away the next time an editor opens that page of an item.

## URL Fragment option
Below the boxes, **Would you like to implement a URL Fragment?** turns the *URL Fragment* field on or off for this type. Choose **Yes** if you want SEO-friendly URLs for specific items. When on, editors see the field on the General Information page. See [Adding and Editing Items](02-adding-and-editing-items.md).

## Tips for good instructions
- Keep them short and specific, since they are displayed in a prominent box.
- Use plain text. Do not rely on formatting or links.
- State your rules, such as naming conventions, who should be the review contact, or required fields.
- Review them whenever your process changes.
- Instructions only affect the source that is currently active. Check which source you are working in on the home page.
