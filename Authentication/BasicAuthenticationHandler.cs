using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using robot_controller_api.Models;
using robot_controller_api.Persistence;

namespace robot_controller_api.Authentication;

public class BasicAuthenticationHandler
    : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly UserDataAccess _userDataAccess;

public BasicAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    UserDataAccess userDataAccess)
    : base(options, logger, encoder)
{
    _userDataAccess = userDataAccess;
}

    protected override Task<AuthenticateResult>
        HandleAuthenticateAsync()
    {
        Response.Headers.Add(
            "WWW-Authenticate",
            @"Basic realm=""Access to robot controller""");

        var authHeader =
            Request.Headers["Authorization"].ToString();

        if (string.IsNullOrEmpty(authHeader))
        {
            return Task.FromResult(
                AuthenticateResult.Fail(
                    "Missing Authorization Header"));
        }

        if (!authHeader.StartsWith("Basic "))
        {
            return Task.FromResult(
                AuthenticateResult.Fail(
                    "Invalid Authorization Header"));
        }

        try
        {
            // Remove "Basic "
            var encodedCredentials =
                authHeader.Substring(6);

            // Decode Base64
            var decodedBytes =
                Convert.FromBase64String(encodedCredentials);

            var credentials =
                Encoding.UTF8.GetString(decodedBytes);

            // Split email and password
            var parts = credentials.Split(':');

            if (parts.Length != 2)
            {
                return Task.FromResult(
                    AuthenticateResult.Fail(
                        "Invalid Credentials Format"));
            }

            var email = parts[0];
            var password = parts[1];

            // Find user
            var user =
                _userDataAccess.GetUserByEmail(email);

            if (user == null)
            {
                return Task.FromResult(
                    AuthenticateResult.Fail(
                        "User not found"));
            }

            // Verify password
            var hasher =
                new PasswordHasher<UserModel>();

            var result =
                hasher.VerifyHashedPassword(
                    user,
                    user.PasswordHash,
                    password);

            if (result ==
                PasswordVerificationResult.Failed)
            {
                return Task.FromResult(
                    AuthenticateResult.Fail(
                        "Invalid Password"));
            }

            // Create claims
            var claims = new[]
            {
                new Claim(
                    ClaimTypes.Name,
                    $"{user.FirstName} {user.LastName}"),

                new Claim(
                    ClaimTypes.Role,
                    user.Role ?? "User"),

                new Claim(
                    ClaimTypes.Email,
                    user.Email)
            };

            // Create identity
            var identity =
                new ClaimsIdentity(claims, Scheme.Name);

            var principal =
                new ClaimsPrincipal(identity);

            var ticket =
                new AuthenticationTicket(
                    principal,
                    Scheme.Name);

            return Task.FromResult(
                AuthenticateResult.Success(ticket));
        }
        catch
        {
            return Task.FromResult(
                AuthenticateResult.Fail(
                    "Authentication Failed"));
        }
    }
}