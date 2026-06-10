# Application Layer

## Purpose
Catalog features, commands, queries, handlers, DTOs.

## Current implementation summary
This document was generated from the current repository snapshot. It references source paths where evidence exists and marks uncertain areas as Needs Verification.

## Important source files
- `src/Randevoo.Application`

| Area | Type | Feature | Kind | Class | Source |
| --- | --- | --- | --- | --- | --- |
| Auth | Commands | ConfirmEmail | Command | ConfirmEmailCommand | `src/Randevoo.Application/Features/Auth/Commands/ConfirmEmail/ConfirmEmailCommand.cs` |
| Auth | Commands | ConfirmEmail | Handler | ConfirmEmailHandler | `src/Randevoo.Application/Features/Auth/Commands/ConfirmEmail/ConfirmEmailHandler.cs` |
| Auth | Commands | RefreshAccessToken | Command | RefreshAccessTokenCommand | `src/Randevoo.Application/Features/Auth/Commands/RefreshAccessToken/RefreshAccessTokenCommand.cs` |
| Auth | Commands | RefreshAccessToken | Handler | RefreshAccessTokenHandler | `src/Randevoo.Application/Features/Auth/Commands/RefreshAccessToken/RefreshAccessTokenHandler.cs` |
| Auth | Commands | RequestEmailConfirmation | Command | RequestEmailConfirmationCommand | `src/Randevoo.Application/Features/Auth/Commands/RequestEmailConfirmation/RequestEmailConfirmationCommand.cs` |
| Auth | Commands | RequestEmailConfirmation | Handler | RequestEmailConfirmationHandler | `src/Randevoo.Application/Features/Auth/Commands/RequestEmailConfirmation/RequestEmailConfirmationHandler.cs` |
| Auth | Commands | RequestMobileLoginCode | Command | RequestMobileLoginCodeCommand | `src/Randevoo.Application/Features/Auth/Commands/RequestMobileLoginCode/RequestMobileLoginCodeCommand.cs` |
| Auth | Commands | RequestMobileLoginCode | Handler | RequestMobileLoginCodeHandler | `src/Randevoo.Application/Features/Auth/Commands/RequestMobileLoginCode/RequestMobileLoginCodeHandler.cs` |
| Auth | Commands | RevokeRefreshToken | Command | RevokeRefreshTokenCommand | `src/Randevoo.Application/Features/Auth/Commands/RevokeRefreshToken/RevokeRefreshTokenCommand.cs` |
| Auth | Commands | RevokeRefreshToken | Handler | RevokeRefreshTokenHandler | `src/Randevoo.Application/Features/Auth/Commands/RevokeRefreshToken/RevokeRefreshTokenHandler.cs` |
| Auth | Commands | VerifyMobileLoginCode | Command | VerifyMobileLoginCodeCommand | `src/Randevoo.Application/Features/Auth/Commands/VerifyMobileLoginCode/VerifyMobileLoginCodeCommand.cs` |
| Auth | Commands | VerifyMobileLoginCode | Handler | VerifyMobileLoginCodeHandler | `src/Randevoo.Application/Features/Auth/Commands/VerifyMobileLoginCode/VerifyMobileLoginCodeHandler.cs` |
| Auth | Common | AuthResult.cs | Other | AuthResult | `src/Randevoo.Application/Features/Auth/Common/AuthResult.cs` |
| Balances | Commands | AdjustBalance | Command | AdjustBalanceCommand | `src/Randevoo.Application/Features/Balances/Commands/AdjustBalance/AdjustBalanceCommand.cs` |
| Balances | Commands | AdjustBalance | Handler | AdjustBalanceHandler | `src/Randevoo.Application/Features/Balances/Commands/AdjustBalance/AdjustBalanceHandler.cs` |
| Balances | Common | BalanceDto.cs | DTO | BalanceTransactionDto | `src/Randevoo.Application/Features/Balances/Common/BalanceDto.cs` |
| Balances | Queries | GetBalance | Handler | GetBalanceHandler | `src/Randevoo.Application/Features/Balances/Queries/GetBalance/GetBalanceHandler.cs` |
| Balances | Queries | GetBalance | Query | GetBalanceQuery | `src/Randevoo.Application/Features/Balances/Queries/GetBalance/GetBalanceQuery.cs` |
| DatingEvents | Commands | ApproveEventParticipantSmsRequest | Command | ApproveEventParticipantSmsRequestCommand | `src/Randevoo.Application/Features/DatingEvents/Commands/ApproveEventParticipantSmsRequest/ApproveEventParticipantSmsRequestCommand.cs` |
| DatingEvents | Commands | ApproveEventParticipantSmsRequest | Handler | ApproveEventParticipantSmsRequestHandler | `src/Randevoo.Application/Features/DatingEvents/Commands/ApproveEventParticipantSmsRequest/ApproveEventParticipantSmsRequestHandler.cs` |
| DatingEvents | Commands | BuyDatingEventTicket | Command | BuyDatingEventTicketCommand | `src/Randevoo.Application/Features/DatingEvents/Commands/BuyDatingEventTicket/BuyDatingEventTicketCommand.cs` |
| DatingEvents | Commands | BuyDatingEventTicket | Handler | BuyDatingEventTicketHandler | `src/Randevoo.Application/Features/DatingEvents/Commands/BuyDatingEventTicket/BuyDatingEventTicketHandler.cs` |
| DatingEvents | Commands | CancelDatingEvent | Command | CancelDatingEventCommand | `src/Randevoo.Application/Features/DatingEvents/Commands/CancelDatingEvent/CancelDatingEventCommand.cs` |
| DatingEvents | Commands | CancelDatingEvent | Handler | CancelDatingEventHandler | `src/Randevoo.Application/Features/DatingEvents/Commands/CancelDatingEvent/CancelDatingEventHandler.cs` |
| DatingEvents | Commands | ChangeDatingEventLocation | Command | ChangeDatingEventLocationCommand | `src/Randevoo.Application/Features/DatingEvents/Commands/ChangeDatingEventLocation/ChangeDatingEventLocationCommand.cs` |
| DatingEvents | Commands | ChangeDatingEventLocation | Handler | ChangeDatingEventLocationHandler | `src/Randevoo.Application/Features/DatingEvents/Commands/ChangeDatingEventLocation/ChangeDatingEventLocationHandler.cs` |
| DatingEvents | Commands | CreateDatingEvent | Command | CreateDatingEventCommand | `src/Randevoo.Application/Features/DatingEvents/Commands/CreateDatingEvent/CreateDatingEventCommand.cs` |
| DatingEvents | Commands | CreateDatingEvent | Handler | CreateDatingEventHandler | `src/Randevoo.Application/Features/DatingEvents/Commands/CreateDatingEvent/CreateDatingEventHandler.cs` |
| DatingEvents | Commands | RejectEventParticipantSmsRequest | Command | RejectEventParticipantSmsRequestCommand | `src/Randevoo.Application/Features/DatingEvents/Commands/RejectEventParticipantSmsRequest/RejectEventParticipantSmsRequestCommand.cs` |
| DatingEvents | Commands | RejectEventParticipantSmsRequest | Handler | RejectEventParticipantSmsRequestHandler | `src/Randevoo.Application/Features/DatingEvents/Commands/RejectEventParticipantSmsRequest/RejectEventParticipantSmsRequestHandler.cs` |
| DatingEvents | Commands | RequestEventParticipantSms | Command | RequestEventParticipantSmsCommand | `src/Randevoo.Application/Features/DatingEvents/Commands/RequestEventParticipantSms/RequestEventParticipantSmsCommand.cs` |
| DatingEvents | Commands | RequestEventParticipantSms | Handler | RequestEventParticipantSmsHandler | `src/Randevoo.Application/Features/DatingEvents/Commands/RequestEventParticipantSms/RequestEventParticipantSmsHandler.cs` |
| DatingEvents | Commands | SendSmsToParticipants | Command | SendSmsToParticipantsCommand | `src/Randevoo.Application/Features/DatingEvents/Commands/SendSmsToParticipants/SendSmsToParticipantsCommand.cs` |
| DatingEvents | Commands | SendSmsToParticipants | Handler | SendSmsToParticipantsHandler | `src/Randevoo.Application/Features/DatingEvents/Commands/SendSmsToParticipants/SendSmsToParticipantsHandler.cs` |
| DatingEvents | Commands | SetDatingEventCommission | Command | SetDatingEventCommissionCommand | `src/Randevoo.Application/Features/DatingEvents/Commands/SetDatingEventCommission/SetDatingEventCommissionCommand.cs` |
| DatingEvents | Commands | SetDatingEventCommission | Handler | SetDatingEventCommissionHandler | `src/Randevoo.Application/Features/DatingEvents/Commands/SetDatingEventCommission/SetDatingEventCommissionHandler.cs` |
| DatingEvents | Commands | SetDatingEventSaleStatus | Command | SetDatingEventSaleStatusCommand | `src/Randevoo.Application/Features/DatingEvents/Commands/SetDatingEventSaleStatus/SetDatingEventSaleStatusCommand.cs` |
| DatingEvents | Commands | SetDatingEventSaleStatus | Handler | SetDatingEventSaleStatusHandler | `src/Randevoo.Application/Features/DatingEvents/Commands/SetDatingEventSaleStatus/SetDatingEventSaleStatusHandler.cs` |
| DatingEvents | Common | DatingEventDto.cs | DTO | DatingEventDto | `src/Randevoo.Application/Features/DatingEvents/Common/DatingEventDto.cs` |
| DatingEvents | Common | DatingEventInput.cs | Other | DatingEventInput | `src/Randevoo.Application/Features/DatingEvents/Common/DatingEventInput.cs` |
| DatingEvents | Queries | ListOpenDatingEvents | Handler | ListOpenDatingEventsHandler | `src/Randevoo.Application/Features/DatingEvents/Queries/ListOpenDatingEvents/ListOpenDatingEventsHandler.cs` |
| DatingEvents | Queries | ListOpenDatingEvents | Query | ListOpenDatingEventsQuery | `src/Randevoo.Application/Features/DatingEvents/Queries/ListOpenDatingEvents/ListOpenDatingEventsQuery.cs` |
| DatingProfile | Commands | CreateDatingProfile | Command | CreateDatingProfileCommand | `src/Randevoo.Application/Features/DatingProfile/Commands/CreateDatingProfile/CreateDatingProfileCommand.cs` |
| DatingProfile | Commands | CreateDatingProfile | Handler | CreateDatingProfileHandler | `src/Randevoo.Application/Features/DatingProfile/Commands/CreateDatingProfile/CreateDatingProfileHandler.cs` |
| DatingProfile | Commands | DeleteDatingProfile | Command | DeleteDatingProfileCommand | `src/Randevoo.Application/Features/DatingProfile/Commands/DeleteDatingProfile/DeleteDatingProfileCommand.cs` |
| DatingProfile | Commands | DeleteDatingProfile | Handler | DeleteDatingProfileHandler | `src/Randevoo.Application/Features/DatingProfile/Commands/DeleteDatingProfile/DeleteDatingProfileHandler.cs` |
| DatingProfile | Commands | UpdateDatingProfile | Command | UpdateDatingProfileCommand | `src/Randevoo.Application/Features/DatingProfile/Commands/UpdateDatingProfile/UpdateDatingProfileCommand.cs` |
| DatingProfile | Commands | UpdateDatingProfile | Handler | UpdateDatingProfileHandler | `src/Randevoo.Application/Features/DatingProfile/Commands/UpdateDatingProfile/UpdateDatingProfileHandler.cs` |
| DatingProfile | Common | DatingProfileDto.cs | DTO | DatingProfileDto | `src/Randevoo.Application/Features/DatingProfile/Common/DatingProfileDto.cs` |
| DatingProfile | Queries | GetDatingProfile | Handler | GetDatingProfileByIdHandler | `src/Randevoo.Application/Features/DatingProfile/Queries/GetDatingProfile/GetDatingProfileByIdHandler.cs` |
| DatingProfile | Queries | GetDatingProfile | Query | GetDatingProfileByIdQuery | `src/Randevoo.Application/Features/DatingProfile/Queries/GetDatingProfile/GetDatingProfileByIdQuery.cs` |
| DatingProfile | Queries | GetDatingProfile | Handler | GetDatingProfileByUserIdHandler | `src/Randevoo.Application/Features/DatingProfile/Queries/GetDatingProfile/GetDatingProfileByUserIdHandler.cs` |
| DatingProfile | Queries | GetDatingProfile | Query | GetDatingProfileByUserIdQuery | `src/Randevoo.Application/Features/DatingProfile/Queries/GetDatingProfile/GetDatingProfileByUserIdQuery.cs` |
| EventChats | Commands | BlockEventChatUser | Command | BlockEventChatUserCommand | `src/Randevoo.Application/Features/EventChats/Commands/BlockEventChatUser/BlockEventChatUserCommand.cs` |
| EventChats | Commands | BlockEventChatUser | Handler | BlockEventChatUserHandler | `src/Randevoo.Application/Features/EventChats/Commands/BlockEventChatUser/BlockEventChatUserHandler.cs` |
| EventChats | Commands | RejectEventLike | Command | RejectEventLikeCommand | `src/Randevoo.Application/Features/EventChats/Commands/RejectEventLike/RejectEventLikeCommand.cs` |
| EventChats | Commands | RejectEventLike | Handler | RejectEventLikeHandler | `src/Randevoo.Application/Features/EventChats/Commands/RejectEventLike/RejectEventLikeHandler.cs` |
| EventChats | Commands | SendEventChatMessage | Command | SendEventChatMessageCommand | `src/Randevoo.Application/Features/EventChats/Commands/SendEventChatMessage/SendEventChatMessageCommand.cs` |
| EventChats | Commands | SendEventChatMessage | Handler | SendEventChatMessageHandler | `src/Randevoo.Application/Features/EventChats/Commands/SendEventChatMessage/SendEventChatMessageHandler.cs` |
| EventChats | Commands | StartEventConversation | Command | StartEventConversationCommand | `src/Randevoo.Application/Features/EventChats/Commands/StartEventConversation/StartEventConversationCommand.cs` |
| EventChats | Commands | StartEventConversation | Handler | StartEventConversationHandler | `src/Randevoo.Application/Features/EventChats/Commands/StartEventConversation/StartEventConversationHandler.cs` |
| EventChats | Common | EventConversationDto.cs | DTO | EventConversationDto | `src/Randevoo.Application/Features/EventChats/Common/EventConversationDto.cs` |
| EventChats | Common | EventLikeResultDto.cs | DTO | EventLikeResultDto | `src/Randevoo.Application/Features/EventChats/Common/EventLikeResultDto.cs` |
| EventChats | Queries | ListMyEventConversations | Handler | ListMyEventConversationsHandler | `src/Randevoo.Application/Features/EventChats/Queries/ListMyEventConversations/ListMyEventConversationsHandler.cs` |
| EventChats | Queries | ListMyEventConversations | Query | ListMyEventConversationsQuery | `src/Randevoo.Application/Features/EventChats/Queries/ListMyEventConversations/ListMyEventConversationsQuery.cs` |
| EventParticipants | Commands | RemoveEventParticipant | Command | RemoveEventParticipantCommand | `src/Randevoo.Application/Features/EventParticipants/Commands/RemoveEventParticipant/RemoveEventParticipantCommand.cs` |
| EventParticipants | Commands | RemoveEventParticipant | Handler | RemoveEventParticipantHandler | `src/Randevoo.Application/Features/EventParticipants/Commands/RemoveEventParticipant/RemoveEventParticipantHandler.cs` |
| EventParticipants | Common | EventArchiveItemDto.cs | DTO | EventArchiveItemDto | `src/Randevoo.Application/Features/EventParticipants/Common/EventArchiveItemDto.cs` |
| EventParticipants | Common | EventParticipantDto.cs | DTO | EventParticipantDto | `src/Randevoo.Application/Features/EventParticipants/Common/EventParticipantDto.cs` |
| EventParticipants | Queries | ListEventParticipants | Handler | ListEventParticipantsHandler | `src/Randevoo.Application/Features/EventParticipants/Queries/ListEventParticipants/ListEventParticipantsHandler.cs` |
| EventParticipants | Queries | ListEventParticipants | Query | ListEventParticipantsQuery | `src/Randevoo.Application/Features/EventParticipants/Queries/ListEventParticipants/ListEventParticipantsQuery.cs` |
| EventParticipants | Queries | ListMyEventArchive | Handler | ListMyEventArchiveHandler | `src/Randevoo.Application/Features/EventParticipants/Queries/ListMyEventArchive/ListMyEventArchiveHandler.cs` |
| EventParticipants | Queries | ListMyEventArchive | Query | ListMyEventArchiveQuery | `src/Randevoo.Application/Features/EventParticipants/Queries/ListMyEventArchive/ListMyEventArchiveQuery.cs` |
| EventParticipants | Queries | ListVisibleParticipantProfiles | Handler | ListVisibleParticipantProfilesHandler | `src/Randevoo.Application/Features/EventParticipants/Queries/ListVisibleParticipantProfiles/ListVisibleParticipantProfilesHandler.cs` |
| EventParticipants | Queries | ListVisibleParticipantProfiles | Query | ListVisibleParticipantProfilesQuery | `src/Randevoo.Application/Features/EventParticipants/Queries/ListVisibleParticipantProfiles/ListVisibleParticipantProfilesQuery.cs` |
| EventPlannerProfiles | Commands | UpsertEventPlannerProfile | Command | UpsertEventPlannerProfileCommand | `src/Randevoo.Application/Features/EventPlannerProfiles/Commands/UpsertEventPlannerProfile/UpsertEventPlannerProfileCommand.cs` |
| EventPlannerProfiles | Commands | UpsertEventPlannerProfile | Handler | UpsertEventPlannerProfileHandler | `src/Randevoo.Application/Features/EventPlannerProfiles/Commands/UpsertEventPlannerProfile/UpsertEventPlannerProfileHandler.cs` |
| EventPlannerProfiles | Common | EventPlannerProfileDto.cs | DTO | EventPlannerProfileDto | `src/Randevoo.Application/Features/EventPlannerProfiles/Common/EventPlannerProfileDto.cs` |
| EventSurveys | Commands | SubmitEventSurvey | Command | SubmitEventSurveyCommand | `src/Randevoo.Application/Features/EventSurveys/Commands/SubmitEventSurvey/SubmitEventSurveyCommand.cs` |
| EventSurveys | Commands | SubmitEventSurvey | Handler | SubmitEventSurveyHandler | `src/Randevoo.Application/Features/EventSurveys/Commands/SubmitEventSurvey/SubmitEventSurveyHandler.cs` |
| EventSurveys | Common | EventSurveyDto.cs | DTO | EventSurveyDto | `src/Randevoo.Application/Features/EventSurveys/Common/EventSurveyDto.cs` |
| EventSurveys | Queries | GetMyEventSurvey | Handler | GetMyEventSurveyHandler | `src/Randevoo.Application/Features/EventSurveys/Queries/GetMyEventSurvey/GetMyEventSurveyHandler.cs` |
| EventSurveys | Queries | GetMyEventSurvey | Query | GetMyEventSurveyQuery | `src/Randevoo.Application/Features/EventSurveys/Queries/GetMyEventSurvey/GetMyEventSurveyQuery.cs` |
| EventTypes | Commands | UpsertEventType | Command | UpsertEventTypeCommand | `src/Randevoo.Application/Features/EventTypes/Commands/UpsertEventType/UpsertEventTypeCommand.cs` |
| EventTypes | Commands | UpsertEventType | Handler | UpsertEventTypeHandler | `src/Randevoo.Application/Features/EventTypes/Commands/UpsertEventType/UpsertEventTypeHandler.cs` |
| EventTypes | Common | EventTypeDto.cs | DTO | EventTypeDto | `src/Randevoo.Application/Features/EventTypes/Common/EventTypeDto.cs` |
| EventTypes | Queries | ListEventTypes | Handler | ListEventTypesHandler | `src/Randevoo.Application/Features/EventTypes/Queries/ListEventTypes/ListEventTypesHandler.cs` |
| EventTypes | Queries | ListEventTypes | Query | ListEventTypesQuery | `src/Randevoo.Application/Features/EventTypes/Queries/ListEventTypes/ListEventTypesQuery.cs` |
| Moderation | Commands | CreateModerationReport | Command | CreateModerationReportCommand | `src/Randevoo.Application/Features/Moderation/Commands/CreateModerationReport/CreateModerationReportCommand.cs` |
| Moderation | Commands | CreateModerationReport | Handler | CreateModerationReportHandler | `src/Randevoo.Application/Features/Moderation/Commands/CreateModerationReport/CreateModerationReportHandler.cs` |
| Moderation | Commands | ReviewModerationReport | Command | ReviewModerationReportCommand | `src/Randevoo.Application/Features/Moderation/Commands/ReviewModerationReport/ReviewModerationReportCommand.cs` |
| Moderation | Commands | ReviewModerationReport | Handler | ReviewModerationReportHandler | `src/Randevoo.Application/Features/Moderation/Commands/ReviewModerationReport/ReviewModerationReportHandler.cs` |
| Moderation | Common | ModerationReportDto.cs | DTO | ModerationReportDto | `src/Randevoo.Application/Features/Moderation/Common/ModerationReportDto.cs` |
| Moderation | Queries | ListModerationReports | Handler | ListModerationReportsHandler | `src/Randevoo.Application/Features/Moderation/Queries/ListModerationReports/ListModerationReportsHandler.cs` |
| Moderation | Queries | ListModerationReports | Query | ListModerationReportsQuery | `src/Randevoo.Application/Features/Moderation/Queries/ListModerationReports/ListModerationReportsQuery.cs` |
| Privacy | Commands | DeleteMyAccount | Command | DeleteMyAccountCommand | `src/Randevoo.Application/Features/Privacy/Commands/DeleteMyAccount/DeleteMyAccountCommand.cs` |
| Privacy | Commands | DeleteMyAccount | Handler | DeleteMyAccountHandler | `src/Randevoo.Application/Features/Privacy/Commands/DeleteMyAccount/DeleteMyAccountHandler.cs` |
| Privacy | Common | PrivacyExportDto.cs | DTO | PrivacyExportDto | `src/Randevoo.Application/Features/Privacy/Common/PrivacyExportDto.cs` |
| Privacy | Queries | ExportMyData | Handler | ExportMyDataHandler | `src/Randevoo.Application/Features/Privacy/Queries/ExportMyData/ExportMyDataHandler.cs` |
| Privacy | Queries | ExportMyData | Query | ExportMyDataQuery | `src/Randevoo.Application/Features/Privacy/Queries/ExportMyData/ExportMyDataQuery.cs` |
| SupportTickets | Commands | ChangeSupportTicketStatus | Command | ChangeSupportTicketStatusCommand | `src/Randevoo.Application/Features/SupportTickets/Commands/ChangeSupportTicketStatus/ChangeSupportTicketStatusCommand.cs` |
| SupportTickets | Commands | ChangeSupportTicketStatus | Handler | ChangeSupportTicketStatusHandler | `src/Randevoo.Application/Features/SupportTickets/Commands/ChangeSupportTicketStatus/ChangeSupportTicketStatusHandler.cs` |
| SupportTickets | Commands | CreateSupportTicket | Command | CreateSupportTicketCommand | `src/Randevoo.Application/Features/SupportTickets/Commands/CreateSupportTicket/CreateSupportTicketCommand.cs` |
| SupportTickets | Commands | CreateSupportTicket | Handler | CreateSupportTicketHandler | `src/Randevoo.Application/Features/SupportTickets/Commands/CreateSupportTicket/CreateSupportTicketHandler.cs` |
| SupportTickets | Commands | ReassignSupportTicket | Command | ReassignSupportTicketCommand | `src/Randevoo.Application/Features/SupportTickets/Commands/ReassignSupportTicket/ReassignSupportTicketCommand.cs` |
| SupportTickets | Commands | ReassignSupportTicket | Handler | ReassignSupportTicketHandler | `src/Randevoo.Application/Features/SupportTickets/Commands/ReassignSupportTicket/ReassignSupportTicketHandler.cs` |
| SupportTickets | Commands | ReplyToSupportTicket | Command | ReplyToSupportTicketCommand | `src/Randevoo.Application/Features/SupportTickets/Commands/ReplyToSupportTicket/ReplyToSupportTicketCommand.cs` |
| SupportTickets | Commands | ReplyToSupportTicket | Handler | ReplyToSupportTicketHandler | `src/Randevoo.Application/Features/SupportTickets/Commands/ReplyToSupportTicket/ReplyToSupportTicketHandler.cs` |
| SupportTickets | Common | SupportTicketAttachmentInput.cs | Other | SupportTicketAttachmentInput | `src/Randevoo.Application/Features/SupportTickets/Common/SupportTicketAttachmentInput.cs` |
| SupportTickets | Common | SupportTicketDtos.cs | DTO | SupportTicketListItemDto | `src/Randevoo.Application/Features/SupportTickets/Common/SupportTicketDtos.cs` |
| SupportTickets | Queries | GetSupportTicket | Handler | GetSupportTicketHandler | `src/Randevoo.Application/Features/SupportTickets/Queries/GetSupportTicket/GetSupportTicketHandler.cs` |
| SupportTickets | Queries | GetSupportTicket | Query | GetSupportTicketQuery | `src/Randevoo.Application/Features/SupportTickets/Queries/GetSupportTicket/GetSupportTicketQuery.cs` |
| SupportTickets | Queries | ListSupportTickets | Handler | ListSupportTicketsHandler | `src/Randevoo.Application/Features/SupportTickets/Queries/ListSupportTickets/ListSupportTicketsHandler.cs` |
| SupportTickets | Queries | ListSupportTickets | Query | ListSupportTicketsQuery | `src/Randevoo.Application/Features/SupportTickets/Queries/ListSupportTickets/ListSupportTicketsQuery.cs` |
| Users | Commands | ChangeUserRole | Command | ChangeUserRoleCommand | `src/Randevoo.Application/Features/Users/Commands/ChangeUserRole/ChangeUserRoleCommand.cs` |
| Users | Commands | ChangeUserRole | Handler | ChangeUserRoleHandler | `src/Randevoo.Application/Features/Users/Commands/ChangeUserRole/ChangeUserRoleHandler.cs` |

## Practical notes for developers
Use the linked source files as the authority. Validate behavior with tests before changing production code.

## Practical notes for future AI coding agents
Do not invent missing behavior. If a feature is represented only by docs or UI labels, mark it as partial until a handler, endpoint, entity, or test confirms it.
