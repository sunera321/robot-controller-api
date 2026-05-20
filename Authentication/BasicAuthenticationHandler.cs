using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using robot_controller_api.Models;
using robot_controller_api.Persistence;

// This folder keeps the custom login code.
namespace robot_controller_api.Authentication;

// This class checks the Basic Auth header and turns it into a signed-in user.
public class BasicAuthenticationHandler
    : AuthenticationHandler<AuthenticationSchemeOptions>
{
    // This object reads user data from the database.
    private readonly UserDataAccess _userDataAccess;

    // Save the user data service so this class can find users.
    public BasicAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        UserDataAccess userDataAccess)
        : base(options, logger, encoder)
    {
        _userDataAccess = userDataAccess;
    }

    // This method checks the login request.
    protected override Task<AuthenticateResult>
        HandleAuthenticateAsync()
    {
        // Tell the client that Basic Auth is needed.
        Response.Headers.Add(
            "WWW-Authenticate",
            @"Basic realm=""Access to robot controller""");
        
        // Read the Authorization header from the request.
        var authHeader =
            Request.Headers["Authorization"].ToString();
        Console.WriteLine($"Received Authorization Header: {authHeader}"); // Debug line to print the header value    
        // This debug line prints the header value.
        //Console.WriteLine($"Authorization Header: {authHeader}");

        // If the header is empty, login failed.
        if (string.IsNullOrEmpty(authHeader))
        {
            return Task.FromResult(
                AuthenticateResult.Fail(
                    "Missing Authorization Header"));
        }

        // If the header is not Basic Auth, fail.
        if (!authHeader.StartsWith("Basic "))
        {
            return Task.FromResult(
                AuthenticateResult.Fail(
                    "Invalid Authorization Header"));
        }

        // Try to read and check the login details.
        try
        {
            // Remove the Basic word from the header.
            var encodedCredentials =
                authHeader.Substring(6);

            // Convert the Base64 text into bytes.
            var decodedBytes =
                Convert.FromBase64String(encodedCredentials);

            // This debug line prints the decoded bytes.
            Console.WriteLine($"Decoded Bytes: {BitConverter.ToString(decodedBytes)}");    

            // Turn the bytes into normal text.
            var credentials =
                Encoding.UTF8.GetString(decodedBytes);

            // This debug line prints the username and password text.
            Console.WriteLine($"Credentials: {credentials}");

            // Split the text into email and password.
            var parts = credentials.Split(':');

            // This debug line shows both parts.
            Console.WriteLine($"Parts: {string.Join(", ", parts)}");

            // The text must have exactly two parts.
            if (parts.Length != 2)
            {
                return Task.FromResult(
                    AuthenticateResult.Fail(
                        "Invalid Credentials Format"));
            }

            // Save the email and password values.
            var email = parts[0];
            var password = parts[1];

            // Look up the user by email.
            var user =
                _userDataAccess.GetUserByEmail(email);

            // If no user is found, stop here.
            if (user == null)
            {
                return Task.FromResult(
                    AuthenticateResult.Fail(
                        "User not found"));
            }

            // Check the password against the saved hash.
            var hasher =
                new PasswordHasher<UserModel>();

            // Compare the entered password with the stored password hash.
            var result =
                hasher.VerifyHashedPassword(
                    user,
                    user.PasswordHash,
                    password);

            // If the password does not match, fail login.
            if (result == PasswordVerificationResult.Failed)
            {
                return Task.FromResult(
                    AuthenticateResult.Fail(
                        "Invalid Password"));
            }

            // Create the values that describe this signed-in user.
            var claims = new[]
            {
                new Claim(
                    ClaimTypes.Name,
                    $"{user.FirstName} {user.LastName}"),

                new Claim(
                    ClaimTypes.Role,
                    user.Role ?? "User"), //if null then default to "User"

                new Claim(
                    ClaimTypes.Email,
                    user.Email)
            };

            // Create the user identity from the claims.
            var identity =
                new ClaimsIdentity(claims, Scheme.Name);

            // Wrap the identity in a principal object.
            var principal =
                new ClaimsPrincipal(identity);

            // Build the login ticket for this request.
            var ticket =
                new AuthenticationTicket(
                    principal,
                    Scheme.Name);
            Console.WriteLine($"User {user.Email} authenticated successfully with role {user.Role}.");
            //ptint console ticket
            Console.WriteLine($"Authentication Ticket: {ticket}");
            // Return success so the user is treated as logged in.
            return Task.FromResult(
                AuthenticateResult.Success(ticket));
            
        }   
        catch
        {
            // Any error means authentication did not work.
            return Task.FromResult(
                AuthenticateResult.Fail(
                    "Authentication Failed"));
        }
    }
}