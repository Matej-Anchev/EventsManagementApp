using System.Text;
using System.Threading.RateLimiting;
using Domain.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Quartz;
using Repository;
using Repository.Implementation;
using Repository.Interface;
using Service.Implementation;
using Service.Interface;
using Service.Jobs;
using Web.Interceptor;
using Web.Mapper;
using Web.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                       throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddScoped<AuditInterceptor>();

builder.Services.AddDbContext<ApplicationDbContext>((sp, options) =>
    {
        options.UseSqlServer(connectionString);
        options.AddInterceptors(sp.GetRequiredService<AuditInterceptor>());
    }
);

builder.Services.AddDbContext<LegacyApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration
            .GetConnectionString("LegacyVenueDb")));

builder.Services.AddControllersWithViews();

builder.Services.AddHttpContextAccessor();

builder.Services.Configure<WeatherApiSettings>(builder.Configuration.GetSection("WeatherApi"));

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 8;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddRazorPages();

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IVenueRepository, VenueRepository>();
builder.Services.AddScoped<ILegacyVenueRepository, LegacyVenueRepository>();

builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<IVenueService, VenueService>();
builder.Services.AddScoped<ReservationMapper>();
builder.Services.AddScoped<IReservationService, ReservationService>();
builder.Services.AddScoped<IFileUploadService, FileUploadService>();
builder.Services.AddScoped<VenueEtlService>();
builder.Services.AddScoped<IWeatherService, WeatherService>();
builder.Services.AddScoped<ICurrentUser, CurrentUser>();
builder.Services.AddScoped<EventMapper>();

builder.Services.AddMemoryCache();

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = 429;

    options.AddPolicy("external-api", context =>
    {
        var apiKey = context.Request.Headers["x-api-key"];

        return RateLimitPartition.GetFixedWindowLimiter(apiKey.ToString(), _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 60,
            Window = TimeSpan.FromMinutes(1),
            QueueLimit = 0,
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst
        });
    });
});

builder.Services.AddHostedService<ReservationCleanupBackgroundService>();
builder.Services.AddHostedService<LegacyDbEtlBackgroundService>();

builder.Services.AddHttpClient<IWeatherApiClient, WeatherApiClient>((sp, client) =>
{
    var settings = sp.GetRequiredService<IOptions<WeatherApiSettings>>();

    client.BaseAddress = new Uri(settings.Value.BaseAddress);
    client.Timeout = TimeSpan.FromSeconds(settings.Value.TimeoutSeconds);
});

// Quartz: AddQuartz must come before AddQuartzHostedService
builder.Services.AddQuartz(options =>
{
    var jobKey = new JobKey("reservation-cleanup", "maintenance");
    options.AddJob<QuartzReservationCleanupJob>(o => o.WithIdentity(jobKey));

    options.AddTrigger(o =>
    {
        o.ForJob(jobKey).WithIdentity("reservation-cleanup-trigger")
            .WithCronSchedule("0 0/1 * * * ?")
            .WithDescription("Expires unpaid reservations");
    });
});

builder.Services.AddQuartzHostedService();

var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]
                                 ?? throw new InvalidOperationException("Jwt:Key is missing from configuration."));

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ClockSkew = TimeSpan.Zero
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication(); // must come before UseAuthorization
app.UseAuthorization();

app.UseMiddleware<ApiKeyAuthMiddleware>();
app.UseRateLimiter();

app.MapStaticAssets();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
    .WithStaticAssets();

app.Run();