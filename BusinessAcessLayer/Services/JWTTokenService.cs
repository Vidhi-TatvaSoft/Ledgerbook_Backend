using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BusinessAcessLayer.Interface;
using DataAccessLayer.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace BusinessAcessLayer.Services;

public class JWTTokenService : IJWTTokenService
{
    private readonly string _secretKey;
    private readonly int _tokenDuration;
    private readonly string _issuer;
    private readonly string _audiance;

    private readonly IGenericRepo _genericRepository;

    public JWTTokenService(IConfiguration configuration,
     IGenericRepo genericRepository)
    {
        _secretKey = configuration.GetValue<string>("JwtConfig:Key");
        _tokenDuration = configuration.GetValue<int>("JwtConfig:Duration");
        _issuer = configuration.GetValue<string>("JwtConfig:Issuer");
        _audiance = configuration.GetValue<string>("JwtConfig:Audience");
        _genericRepository = genericRepository;
    }

    public string GenerateToken(string email)
    {
        SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        ApplicationUser user = _genericRepository.Get<ApplicationUser>(u => u.Email == email && !u.DeletedAt.HasValue)!;
        string UserId = user.Id.ToString();
        string FirstName = user.FirstName;
        string LastName = user.LastName;

        Claim[] claims = new[]
        {
                new Claim("email", email),
                new Claim("id", UserId),
                new Claim("firstname",FirstName),
                new Claim("lastname",LastName),
                new Claim("role","User")
        };

        JwtSecurityToken token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audiance,
            claims: claims,
            expires: DateTime.Now.AddHours(_tokenDuration),
            signingCredentials: credentials
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateCombineToken(string userToken, string? businessToken= null)
    {
        SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        Claim[] claims = new[]
       {
                new Claim("UserToken", userToken),
                new Claim("BusinessToken", businessToken)
        };
        JwtSecurityToken token = new JwtSecurityToken(
                    issuer: _issuer,
                    audience: _audiance,
                    claims: claims,
                    expires: DateTime.Now.AddHours(_tokenDuration),
                    signingCredentials: credentials
                );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateTokenEmailVerificationToken(string email, string verificationToken)
    {
        SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        Claim[] claims = new[]
        {
                new Claim("email", email),
                new Claim("token", verificationToken)
            };

        JwtSecurityToken token = new JwtSecurityToken(
            issuer: "localhost",
            audience: "localhost",
            claims: claims,
            expires: DateTime.Now.AddHours(_tokenDuration),
            signingCredentials: credentials
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateTokenPartyEmailVerification(string email, string verificationToken, int partyId, string businessName, string partyType)
    {
        SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        Claim[] claims = new[]
        {
                new Claim("email", email),
                new Claim("token", verificationToken),
                new Claim("partyId", partyId.ToString()),
                new Claim("businessName", businessName),
                new Claim("partyType", partyType),

            };

        JwtSecurityToken token = new JwtSecurityToken(
            issuer: "localhost",
            audience: "localhost",
            claims: claims,
            expires: DateTime.Now.AddHours(_tokenDuration),
            signingCredentials: credentials
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateTokenEmailPassword(string email, string password)
    {
        SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        Claim[] claims = new[]
        {
                new Claim("email", email),
                new Claim("password", password)
            };

        JwtSecurityToken token = new JwtSecurityToken(
            issuer: "localhost",
            audience: "localhost",
            claims: claims,
            expires: DateTime.Now.AddHours(_tokenDuration),
            signingCredentials: credentials
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }


    public string GenerateBusinessToken(int businessId)
    {
        SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        Businesses business = _genericRepository.Get<Businesses>(b => b.Id == businessId && b.DeletedAt == null)!;
        string businessIdTemp = business.Id.ToString();
        string businessName = business.BusinessName;

        Claim[] claims = new[]
        {
                new Claim("id", businessIdTemp),
                new Claim("name", businessName)
        };

        JwtSecurityToken token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audiance,
            claims: claims,
            expires: DateTime.Now.AddHours(_tokenDuration),
            signingCredentials: credentials
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public ClaimsPrincipal? GetClaimsFromToken(string token)
    {
        JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
        JwtSecurityToken jwtToken = handler.ReadJwtToken(token);
        ClaimsIdentity claims = new ClaimsIdentity(jwtToken.Claims);
        return new ClaimsPrincipal(claims);
    }

    // Retrieves a specific claim value from a JWT token.
    public string? GetClaimValue(string token, string claimType)
    {
        try
        {
            ClaimsPrincipal claimsPrincipal = GetClaimsFromToken(token);
            string value = claimsPrincipal?.FindFirst(claimType)?.Value;
            return value;
        }
        catch (Exception e)
        {
            throw new("Invalid Token Exception");
        }

    }
}