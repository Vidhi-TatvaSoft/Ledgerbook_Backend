using System.Security.Claims;
using System.Text;
using BusinessAcessLayer.Constant;
using BusinessAcessLayer.Interface;
using BusinessAcessLayer.Services;
using DataAccessLayer.Models;
using LedgerBookWebApi.Authorization;
using LedgerBook.ExceptionMiddleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Rotativa.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddDbContext<LedgerBookDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("LedgerbookDbConnection")));


builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
                    .AddEntityFrameworkStores<LedgerBookDbContext>();

// builder.Services.AddCors(options =>
// {
//     options.AddPolicy("AllowLocalhost",
//         policy => policy.WithOrigins("http://localhost:5189")
//             .AllowAnyMethod()
//             .AllowAnyHeader()
//     );
// });

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
// var app = builder.Build();

// app.UseCors();

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
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<IGenericRepo, GenericRepo>();
builder.Services.AddScoped<ITransactionReportSevice, TransactionReportService>();
builder.Services.AddScoped<IExceptionService, ExceptionService>();
builder.Services.AddScoped<IActivityLogService, ActivityLogService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer("Authorization", options =>
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
}
).AddJwtBearer("BusinessToken", options =>
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
});


builder.Services.AddScoped<IAuthorizationHandler, PermissionHandler>();
builder.Services.AddAuthorization(options =>
{
    var permissions = new[]
    {
        "User","Owner/Admin", "PurchaseManager", "SalesManager","AnyRole","User"
    };

    foreach (var permission in permissions)
    {
        options.AddPolicy(permission, policy => policy.Requirements.Add(new PermissionRequirement(permission)));
    }
});

builder.Services.AddSwaggerGen(c =>
    {
        c.AddSecurityDefinition("Authorization", new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header using the Bearer scheme (Token 1)",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = "bearer"
        });
        c.AddSecurityDefinition("BusinessToken", new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header using the Bearer scheme (Token 2)",
            Name = "BusinessToken", // Example for a second token in a different header
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey
        });
        c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Authorization" }
            },
            new string[] { }
        },
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "BusinessToken" }
            },
            new string[] { }
        }
    });
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
// app.UseCors("AllowLocalhost");
app.UseCors();
app.UseAuthorization();
app.UseAuthentication();
app.MapControllers();

app.Run();
