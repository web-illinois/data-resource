# Security

Open **Configuration > Security** to control who can edit a source and to manage API access.

## Managing editors
The page lists everyone with access to the source. The owner is marked and cannot be removed.

### Add a person
1. Enter their Net ID under **New Net ID**. Adding `@illinois.edu` is optional.
2. If the list of departments is shown, you can set **Restrict to a department**. Choose a department to limit that person to items in that department, or leave it as *None (access to all departments)*.
3. Select **Add Net ID**.

Departments come from the Department filter group. See [Filters and Categories](04-filters-and-categories.md).

### Remove a person
Select **Remove** beside their name. The button is disabled for the owner.

## Changing the owner
Every source must have an owner.
1. Under **Change Owner**, choose a person from the list. Only people already added to the source appear.
2. Select **Change Owner**.

## API access
An API key lets a program add, update, and delete items in your source.

### Create a key
1. Select **Create a New API key**.
2. Copy the key shown on the page. **It is displayed only once and cannot be retrieved after you leave the page.**

By default, items created through the API are forced to **Draft** so an administrator can review them. Keep this unless the API is used in a controlled process. When a key is active, the checkbox **Will an API call be forced to Draft mode?** controls this setting.

### Replace or revoke a key
- Creating a new key does **not** disable the old one. Both keys work.
- If a key may be compromised, select **Invalidate the API key** first, and then create a new key.
- The page shows the date the key was last changed.

### Good practices
- Store keys securely, as you would a password.
- Add only the people who need access, and use department restrictions where possible.
- Review the list regularly and remove people who no longer need access.
