using System.Text;
using _2FaSample.Features.Auth;
using _2FaSample.Features.Profile;
using _2FaSample.Features.PushNotifications;
using _2FaSample.Infrastructure.Services;
using _2FaSample.Models;
using _2FaSample.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using WebPush;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile("secrets.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddCookie(IdentityConstants.ApplicationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });
builder.Services.AddIdentityCore<AppUser>(options =>
    {
        options.Tokens.AuthenticatorTokenProvider = TokenOptions.DefaultAuthenticatorProvider;
        options.Tokens.ChangePhoneNumberTokenProvider = "Phone";
    })
    .AddRoles<IdentityRole>() 
    .AddEntityFrameworkStores<AppDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "App API", Version = "v1" });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials(); 
    });
});
builder.Services.AddAuthorization();
var vapidSection = builder.Configuration.GetSection("VapidDetails");
var vapidDetails = new VapidDetails(
    vapidSection["Subject"],
    vapidSection["PublicKey"],
    vapidSection["PrivateKey"]
);

builder.Services.AddSingleton(vapidDetails);
builder.Services.AddSingleton<WebPushClient>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var client = new WebPushClient();
    
    client.SetVapidDetails(
        subject: configuration["Vapid:Subject"],
        publicKey: configuration["Vapid:PublicKey"],
        privateKey: configuration["Vapid:PrivateKey"]
    );
    return client;
});
builder.Services.AddTransient<IEmailSender<AppUser>, EmailSender>();
builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<SmsService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("FrontendPolicy");
app.UseAuthentication();
app.UseAuthorization();

app.MapAuthEndpoints();
app.MapProfileEndpoint();
app.MapPushNotificationsEndpoint();

app.Run();