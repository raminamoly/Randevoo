# UC-024 Event Chat

Includes UC-023 through UC-026.

## Goal

Allow event participants to start limited conversations, message, and block.

## Actor

EndUser or Admin through `EndUserOnly` policy.

## Preconditions

- Event has started.
- Both users have valid non-refunded, non-removed tickets.
- Starter has not exceeded event chat limit.

## Main Flow

1. User starts conversation with another participant.
2. User sends messages.
3. User lists conversations.
4. User can block the other conversation participant.

## Alternative Flows

- Starting chat before event start returns business-rule error.
- Exceeding chat limit returns business-rule error.
- Blocked or disabled conversation rejects messages.

## Business Rules

- User cannot start chat with self.
- Sender must be conversation participant.
- Blocking self is forbidden.

## APIs

API-024 through API-027.

## Entities

`EventConversation`, `EventChatMessage`, `EventChatBlock`, `EventTicket`, `DatingEvent`.
