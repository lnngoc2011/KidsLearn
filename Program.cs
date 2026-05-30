using System.Text;
using KidsLearn.Configurations;
using KidsLearn.Data;
using KidsLearn.Middlewares;
using KidsLearn.Services;
using KidsLearn.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using CloudinaryDotNet;


var builder = WebApplication.CreateBuilder(args);

// =========================================================
// DATABASE
// =========================================================

builder.Services.AddDbContext<KidsLearnDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

// =========================================================
// JWT CONFIGURATION
// =========================================================

var jwtSettings = builder.Configuration
    .GetSection("JwtSettings")
    .Get<JwtSettings>();

if (jwtSettings == null)
{
    throw new InvalidOperationException(
        "JwtSettings chưa cấu hình trong appsettings.json!"
    );
}

builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JwtSettings")
);

// =========================================================
// CORS
// =========================================================

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        // Cho phép mọi port localhost (chỉ dùng cho Development)
        policy.SetIsOriginAllowed(origin =>
                new Uri(origin).Host == "localhost")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// =========================================================
// AUTHENTICATION (JWT)
// =========================================================

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme =
        JwtBearerDefaults.AuthenticationScheme;

    options.DefaultChallengeScheme =
        JwtBearerDefaults.AuthenticationScheme;
})

.AddJwtBearer(options =>
{
    options.TokenValidationParameters =
        new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,

            IssuerSigningKey =
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(
                        jwtSettings.SecretKey
                    )
                ),

            ValidateIssuer = true,

            ValidIssuer =
                jwtSettings.Issuer,

            ValidateAudience = true,

            ValidAudience =
                jwtSettings.Audience,

            ValidateLifetime = true,

            ClockSkew =
                TimeSpan.Zero
        };
});

// =========================================================
// AUTHORIZATION
// =========================================================

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(
        "AdminOnly",
        policy =>
            policy.RequireRole("Admin")
    );
});

// =========================================================
// CLOUDINARY
// =========================================================
// Bind Cloudinary settings
var cloudinarySettings = builder.Configuration
    .GetSection("Cloudinary")
    .Get<CloudinarySettings>();

// Register Cloudinary as a singleton
var account = new Account(
    cloudinarySettings.CloudName,
    cloudinarySettings.ApiKey,
    cloudinarySettings.ApiSecret
);

var cloudinary = new Cloudinary(account);
cloudinary.Api.Secure = true;

builder.Services.AddSingleton(cloudinary);

// =========================================================
// DEPENDENCY INJECTION
// =========================================================

builder.Services.AddScoped<
    IAuthService,
    AuthService>();

builder.Services.AddScoped<
    IGradeService,
    GradeService>();

builder.Services.AddScoped<
    IUnitService,
    UnitService>();

builder.Services.AddScoped<
    IVocabularyService,
    VocabularyService>();

builder.Services.AddScoped<
    IQuizService,
    QuizService>();

builder.Services.AddScoped<
    IProgressService,
    ProgressService>();

builder.Services.AddScoped<
    IUserService,
    UserService>();

builder.Services.AddScoped<
    IGameService,
    GameService>();
builder.Services.AddScoped<
    ICloudinaryService,
    CloudinaryService>();


// =========================================================
// CONTROLLERS
// =========================================================

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

// =========================================================
// SWAGGER
// =========================================================

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc(
        "v1",
        new OpenApiInfo
        {
            Title = "KidsLearn API",

            Version = "v1",

            Description =
                "API hệ thống học tiếng Anh KidsLearn"
        });

    c.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            Name = "Authorization",

            Type =
                SecuritySchemeType.Http,

            Scheme = "Bearer",

            BearerFormat = "JWT",

            In =
                ParameterLocation.Header,

            Description =
                "Nhập JWT token"
        });

    c.AddSecurityRequirement(
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference =
                        new OpenApiReference
                        {
                            Type =
                                ReferenceType.SecurityScheme,

                            Id = "Bearer"
                        }
                },

                new string[] {}
            }
        });
});

// =========================================================
// BUILD APP
// =========================================================

var app = builder.Build();

// =========================================================
// MIDDLEWARE PIPELINE
// =========================================================

// Global Exception Middleware

app.UseMiddleware<ExceptionMiddleware>();

// Swagger

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint(
            "/swagger/v1/swagger.json",
            "KidsLearn API v1"
        );

        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();

app.UseStaticFiles();

// CORS

app.UseCors("AllowReactApp");

// JWT

app.UseAuthentication();

app.UseAuthorization();

// Controllers

app.MapControllers();

app.Run();