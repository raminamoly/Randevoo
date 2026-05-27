public sealed record CreateUserCommand(
    string Email,
    string Name
) : IRequest<Result<Guid>>;
