using System.Security.Claims;

namespace BusinessAcessLayer.Interface;

public interface IJWTTokenService
{
    string GenerateToken(string email);
    string GenerateTokenEmailVerificationToken(string email, string password);
    string GenerateTokenEmailPassword(string email, string password);
    string GenerateBusinessToken(int businessId);
    string GenerateTokenPartyEmailVerification(string email, string verificationToken, int partyId, string businessName, string partyType);
    ClaimsPrincipal? GetClaimsFromToken(string token);
    string? GetClaimValue(string token, string claimType);
}