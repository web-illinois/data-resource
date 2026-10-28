# Review, Publishing, and Email Notifications

## Email notifications
Source administrators can configure emails under **Configuration > Email Setup**.

### Triggers
- When an item is submitted for review
- When an item is published
- When an item is moved back to draft

Each trigger is configured separately. To send emails for several events, set up each one.

### Set up an email
1. Choose a **Trigger Option**.
2. Fill in:
   - **To Email** and **CC Email**: semicolon-separated lists are allowed.
   - **Reply-to Email**
   - **Add review email listed on resource**: also sends to the item's *Email for Review*.
   - **Subject Line**
   - **Body**: plain text, for clients that cannot show HTML.
   - **HTML Body**: for clients that can.
   - **Is this active?**: turns the email on or off.
3. Select **Save**.

To stop an email permanently, select **Remove Email**.

### Emails when an item is submitted
The **When an item is submitted for review** trigger sends an email when a new item is added to the source, whether it is added in the tool or loaded through the API. A source may have its own message for this trigger.

Typical setup:
1. Open **Configuration > Email Setup**.
2. Select **When an item is submitted for review** under **Select Trigger Option**.
3. Choose who receives the message:
   - Enter one or more fixed addresses in **To Email** (separated by semicolons) and optionally **CC Email**, or
   - Select **Add review email listed on resource to "To:" email list** to also send to the *Email for Review* address entered on the item. This is useful when different items have different reviewers.
4. Enter a **Reply-to Email** so replies reach a monitored mailbox.
5. Write the **Subject Line**, plain-text **Body**, and **HTML Body**. Provide both bodies so the message reads well in any email client.
6. Select **Is this active?** and then **Save**.

Notes:
- The email is sent only when **Is this active?** is selected. You can keep a saved message and turn it off temporarily by clearing the checkbox.
- Each trigger has its own message. Setting up the submission email does not set up the publication or draft emails.
- When an email is sent, the result is recorded alongside the change in the item's log entry.
- If nobody receives the message, check that the address is correct, that the message is active, and that either **To Email** has an address or the item has an *Email for Review*.
- **Remove Email** deletes the configuration for the selected trigger. To pause it instead, clear **Is this active?**.

See also [Link Checks](11-link-checks.md).
