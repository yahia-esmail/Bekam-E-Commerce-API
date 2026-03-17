using Bekam.Application.Abstraction.Results;

namespace Bekam.Application.Abstraction.Errors;
public static class UserErrors
{
    public static readonly Error InvalidCredentials =
        new("User.InvalidCredentials", "Invalid email/password", ErrorType.Unauthorized);

    public static readonly Error InactiveUser =
        new("User.Inactive",
        "User account is inactive. Please contact your administrator.", ErrorType.Unauthorized);

    public static readonly Error LockedUser =
        new("User.LockedOut",
           "Your account has been temporarily locked due to multiple failed login attempts. Please try again later.", ErrorType.Unauthorized);

    public static readonly Error DuplicatedEmail =
        new("User.DuplicatedEmail", "An account with this email already exists.", ErrorType.Conflict);

    public static readonly Error EmailNotConfirmed =
        new("User.EmailNotConfirmed", "Email address has not been confirmed.", ErrorType.Unauthorized);

    public static readonly Error UserNotFound =
        new("User.UserNotFound", "User not found.", ErrorType.NotFound);

    public static readonly Error InvalidRoles =
        new("Role.InvalidRoles", "One or more roles are invalid.", ErrorType.Validation);

    public static readonly Error InvalidCode =
        new("User.InvalidCode", "Invalid code.", ErrorType.Validation);

    public static readonly Error EmailAlreadyConfirmed =
        new("User.EmailAlreadyConfirmed", "Email already confirmed.", ErrorType.Conflict);

    public static readonly Error InvalidAccessToken =
        new("User.InvalidAccessToken", "Invalid Access token", ErrorType.Unauthorized);

    public static readonly Error InvalidRefreshToken =
        new("User.InvalidRefreshToken", "Invalid refresh token", ErrorType.Unauthorized);

    public static readonly Error FailedInUpdateUserStatus =
        new("User.FailedInUpdateUserStatus", "failed in update user status", ErrorType.Failure);

    public static readonly Error LockFailed =
        new ("User.LockFailed", "failed in lock user", ErrorType.Failure);

    public static readonly Error ActionNotAllow =
        new ("User.NotAllowed", "This Action is Not allowed for this user", ErrorType.Forbidden);
}