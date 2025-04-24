using Chordy.BusinessLogic.Authorization;
using Chordy.BusinessLogic.Extensions;
using Chordy.BusinessLogic.Jwt;
using Chordy.BusinessLogic.Middleware;
using Chordy.DataAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy =>
        {
            policy.WithOrigins("http://localhost:3000") // адрес фронта
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDataAcess(builder.Configuration);
builder.Services.AddBusinessLogic();
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(nameof(JwtOptions)));
builder.Services.AddApiAuthentication(builder.Configuration);
builder.Services.AddHostedService<RefreshTokenCleanupService>();
builder.Services.AddScoped<IAuthorizationHandler, SongOwnerOrAdminHandler>();
builder.Services.AddScoped<IAuthorizationHandler, ChordVariationOwnerOrAdminHandler>();

builder.Services.AddAuthorizationBuilder().AddPolicy("ChordVariationOwnerOrAdmin", policy =>
    policy.Requirements.Add(new ChordVariationOwnerOrAdminRequirements()));

builder.Services.AddAuthorizationBuilder().AddPolicy("SongOwnerOrAdmin", policy =>
        policy.Requirements.Add(new SongOwnerOrAdminRequirements()));

var app = builder.Build();
app.UseMiddleware<GlobalExceptionMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCookiePolicy(new CookiePolicyOptions
{
    MinimumSameSitePolicy = SameSiteMode.Strict,
    HttpOnly = Microsoft.AspNetCore.CookiePolicy.HttpOnlyPolicy.Always,
    Secure = CookieSecurePolicy.Always,
});

app.UseCors("AllowReactApp");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.UseStaticFiles();

app.Run();
