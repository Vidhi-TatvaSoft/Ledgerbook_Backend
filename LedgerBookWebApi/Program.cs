using System.Security.Claims;
using System.Text;
using BusinessAcessLayer.Constant;
using BusinessAcessLayer.Interface;
using BusinessAcessLayer.Services;
using DataAccessLayer.Models;
using LedgerBook.Authorization;
using LedgerBook.ExceptionMiddleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Rotativa.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddDbContext<LedgerBookDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("LedgerbookDbConnection")));


builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
                    .AddEntityFrameworkStores<LedgerBookDbContext>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost",
        policy => policy.WithOrigins("http://localhost:5189")
            .AllowAnyMethod()
            .AllowAnyHeader()
    );
});

// builder.Services.AddScoped<IViewRenderService, ViewRenderService>();
builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<IJWTTokenService, JWTTokenService>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<IBusinessService, BusinessService>();
builder.Services.AddScoped<IReferenceDataEntityService, ReferenceDataEntityService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IAddressService, AddressService>();
builder.Services.AddScoped<IAttachmentService, AttachmentService>();
builder.Services.AddScoped<IUserBusinessMappingService, UserBusinessMappingService>();
builder.Services.AddScoped<IPartyService, PartyService>();
builder.Services.AddScoped<ICookieService, CookieService>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<IGenericRepo, GenericRepo>();
builder.Services.AddScoped<ITransactionReportSevice, TransactionReportService>();
builder.Services.AddScoped<IExceptionService, ExceptionService>();
builder.Services.AddScoped<IActivityLogService, ActivityLogService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JwtConfig:Issuer"],  // The issuer of the token (e.g., your app's URL)
        ValidAudience = builder.Configuration["JwtConfig:Audience"], // The audience for the token (e.g., your API)
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtConfig:Key"] ?? "")), // The key to validate the JWT's signature
        RoleClaimType = ClaimTypes.Role,
        NameClaimType = ClaimTypes.Name
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            // Check for the token in cookies
            var token = context.Request.Cookies[TokenKey.UserToken];
            if (!string.IsNullOrEmpty(token))
            {
                context.Request.Headers["Authorization"] = "Bearer " + token;
            }
            if (!string.IsNullOrEmpty(token))
            {
                context.Token = token;
            }
            return Task.CompletedTask;
        },
        OnChallenge = context =>
        {
            // Redirect to login page when unauthorized 
            context.HandleResponse();
            context.Response.Redirect("/Login/Login");
            return Task.CompletedTask;
        },
        OnForbidden = context =>
        {
            if (!context.Response.HasStarted)
            {
                // Redirect to unauthorize when access is forbidden (403)
                context.Response.Redirect("/Login/Login");
            }
            return Task.CompletedTask;
        }
    };
}
);


builder.Services.AddScoped<IAuthorizationHandler, PermissionHandler>();
builder.Services.AddAuthorization(options =>
{
    var permissions = new[]
    {
        "User","Owner/Admin", "PurchaseManager", "SalesManager","AnyRole"
    };

    foreach (var permission in permissions)
    {
        options.AddPolicy(permission, policy => policy.Requirements.Add(new PermissionRequirement(permission)));
    }
});


builder.Services.AddSession(
    options =>
    {
        options.IdleTimeout = TimeSpan.FromSeconds(10);
    }
);

builder.Services.AddDistributedMemoryCache();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Use(async (context, next) =>
{
    context.Response.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate");
    context.Response.Headers.Add("Pragma", "no-cache");
    context.Response.Headers.Add("Expires", "0");

    await next();
});


app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseMiddleware<ExceptionMiddleware>();

app.UseRouting();
app.UseCors("AllowLocalhost");
app.UseAuthorization();
app.UseAuthentication();
app.MapControllers();

app.Run();
