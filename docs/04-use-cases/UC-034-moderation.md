# UC-034 Moderation Reports

Includes UC-034 through UC-037.

## Goal

Allow users to report issues and Admins to review reports.

## Actor

Authenticated user, Admin.

## Main Flow

1. User creates report against another user.
2. User can list own reports.
3. Admin lists reports, optionally by status.
4. Admin reviews report and sets status/note.

## Alternative Flows

- User cannot report self.
- Event-context report requires both users in event.
- Conversation-context report requires reporter to be in conversation.
- Admin cannot review report into `Pending` status.

## APIs

API-036 through API-039.

## Entities

`ModerationReport`, `User`, `DatingEvent`, `EventConversation`, `EventTicket`.
