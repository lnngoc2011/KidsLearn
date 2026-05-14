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

var builder = WebApplication.CreateBuilder(args);

// =========================================================
// DATABASE
// =========================================================
builder.Services.AddDbContext<KidsLearnDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// =========================================================
// JWT CONFIGURATION
// =========================================================
var jwtSettings = builder.Configuration
    .GetSection("JwtSettings")
    .Get<JwtSettings>();

if (jwtSettings == null)
    throw new InvalidOperationException("JwtSettings chưa cấu hình trong appsettings.json!");

builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JwtSettings"));

// =========================================================
// AUTHENTICATION (JWT Bearer)
// =========================================================
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),

        ValidateIssuer = true,
        ValidIssuer = jwtSettings.Issuer,

        ValidateAudience = true,
        ValidAudience = jwtSettings.Audience,

        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero    // Không cho phép sai lệch thời gian
    };
});

// =========================================================
// ✨ FIX: KHAI BÁO POLICY "AdminOnly"
// Trước đây Controllers dùng [Authorize(Policy = "AdminOnly")]
// nhưng chưa khai báo → API Admin sẽ luôn 403 Forbidden
// =========================================================
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("Admin"));
});

// =========================================================
// CORS - cho phép React App gọi API
// =========================================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// =========================================================
// ✨ FIX: DEPENDENCY INJECTION
// Trước đây IUnitService đăng ký 2 lần - đã sửa
// =========================================================
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IGradeService, GradeService>();    // ✨ MỚI
builder.Services.AddScoped<IUnitService, UnitService>();
builder.Services.AddScoped<IVocabularyService, VocabularyService>();
builder.Services.AddScoped<IQuizService, QuizService>();
builder.Services.AddScoped<IProgressService, ProgressService>();
builder.Services.AddScoped<IUserService, UserService>();

// =========================================================
// CONTROLLERS + SWAGGER
// =========================================================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "KidsLearn API",
        Version = "v1",
        Description = "API hệ thống học từ vựng tiếng Anh cho trẻ em"
    });

    // Cho phép Swagger gửi JWT Bearer Token để test API Auth
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Nhập token (KHÔNG cần ghi 'Bearer ' phía trước)"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();

// =========================================================
// MIDDLEWARE PIPELINE - thứ tự rất quan trọng!
// =========================================================

// 1. ExceptionMiddleware đầu tiên - bắt mọi lỗi từ các middleware sau
app.UseMiddleware<ExceptionMiddleware>();

// 2. Swagger chỉ bật khi Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "KidsLearn API v1");
        c.RoutePrefix = "swagger";   // Truy cập tại: https://localhost:port/swagger
    });
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// 3. CORS phải trước Authentication
app.UseCors("AllowReactApp");

// 4. Authentication trước Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();