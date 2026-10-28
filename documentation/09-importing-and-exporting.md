# Importing and Exporting Data

The **Transfer Items** section lets you export your source's data to a file and import data from a file. There are two formats:

| Format | Best for | Pages |
|--------|----------|-------|
| Tab-delimited text file | Bulk editing in Excel, manual updates, initial loads | Save Text File, Load File |
| JSON file | Full backup and restore to a blank area | Save JSON File, Load JSON File |

Pages work on the **active source** (see [Getting Started](01-getting-started.md)). The *Select Type to Load* list shows only the item types your source uses (Resources, Publications, Events, People, FAQs, Notes, Org Chart).

> Large transfers take a while. **Do not leave the page** until the confirmation message appears.

---

## Export to a text file (`/transfer/save`)
1. Open **Transfer Items > Save Text File**.
2. Choose a type under **Select Type to Load**.
3. Select **Download File**.
4. Open the downloaded tab-delimited file in Excel.

Format notes:
- **Related Links** are separated by semicolons, in Markdown format: `[link text](https://example.com)`.
- **Tags, Audience, and Topics** are separated by semicolons.
- A *link text* column follows the link column in all exports and imports (added 6/2/2026). If an older file fails to import, add a blank column after the link column.

## Import from a text file (`/transfer/load`)
1. **Get the template.** Download the file from *Save Text File*. Even with no items, it provides the correct column headings.
2. Fill in or edit the data.
3. **If using Excel, save as a tab-delimited text file** before uploading.
4. Open **Transfer Items > Load File** and choose the type.
5. Choose the file, set the options below, and select **Upload File**.
6. Wait for the confirmation message.

Rules:
- IDs not in the file are generated automatically. Any ID you provide must start with the source code followed by a dash (for example, `mysource-123`).
- Tags, Audience, and Topics are separated by semicolons and are **case-sensitive**.
- Related Links use the Markdown format shown above.
- Items load **inactive** by default. Enter `true` under **Active** to load them as active.
- The maximum file size is about 2 MB. Split larger files.

Options (non-Org Chart types):
- **Should you delete all items first?** Deletes all existing items of that type before loading. Use with care; consider downloading a backup first.
- **Ignore tags that do not exist?** Selected by default. Tags in the file that are not defined in the source are skipped. If cleared, the tag list is overwritten with the tags in the file, which can produce unpredictable results for existing tagged items.

### Org Chart
Loading an Org Chart replaces the entire chart. The delete and tag options do not apply.

---

## Export to JSON (`/transfer/savejson`)
1. Open **Transfer Items > Save JSON File**.
2. Choose a type and select **Download File**.

The file is named `source_yyyy_MM_dd_type.json`.

## Import from JSON (`/transfer/loadjson`)
1. Open **Transfer Items > Load JSON File**.
2. Choose the type and the `.json` file, then select **Upload File**.

JSON import is intended as a **full backup and restore to a blank area**. To manually update existing information, use the text file option.

---

## Which should I use?
- Update many items by hand: **text file** (export, edit, import).
- Back up or move a source's data: **JSON**.
- Start a new source with existing data: export the template, fill it in, and import.

## Troubleshooting
- **Import fails or columns look shifted:** add the *link text* column after the link column.
- **Tags missing after import:** tag names must match exactly (case-sensitive) and exist in the source, or turn off *Ignore tags that do not exist*.
- **Special characters look wrong:** save the file as UTF-8.
- **Nothing happens on upload:** choose the file first and wait for the "ready to be uploaded" message.
