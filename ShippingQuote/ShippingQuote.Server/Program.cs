using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ShippingQuote.Server.Data;
using ShippingQuote.Server.Models;
using ShippingQuote.Server.Services;
using System.Text.Json.Serialization;
using ShippingQuote.Server.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
	.AddJsonOptions(options =>
	{
		options.JsonSerializerOptions.Converters
			.Add(new JsonStringEnumConverter());
	});

builder.Services.AddScoped<CarrierService>();
builder.Services.AddScoped<DeliveryServiceService>();
builder.Services.AddScoped<PricingRuleService>();
builder.Services.AddScoped<DashboardService>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<ResourceAccessService>();

builder.Services.Configure<JwtOptions>(
	builder.Configuration.GetSection("Jwt"));

JwtOptions jwtOptions =
	builder.Configuration.GetSection("Jwt").Get<JwtOptions>()
	?? throw new InvalidOperationException("JWT configuration is missing.");

if (string.IsNullOrWhiteSpace(jwtOptions.Key))
{
	throw new InvalidOperationException("JWT signing key is missing.");
}

builder.Services.AddDbContext<ShippingQuoteDbContext>(options =>
	options.UseSqlServer(
		builder.Configuration.GetConnectionString("DefaultConnection")));

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();


builder.Services
	.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer(options =>
	{
		options.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuer = true,
			ValidIssuer = jwtOptions.Issuer,

			ValidateAudience = true,
			ValidAudience = jwtOptions.Audience,

			ValidateLifetime = true,

			ValidateIssuerSigningKey = true,
			IssuerSigningKey = new SymmetricSecurityKey(
				Encoding.UTF8.GetBytes(jwtOptions.Key)),

			ClockSkew = TimeSpan.Zero
		};
	});

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseDefaultFiles();
app.MapStaticAssets();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");
using (var scope = app.Services.CreateScope())
{
	ShippingQuoteDbContext dbContext =
		scope.ServiceProvider
			.GetRequiredService<ShippingQuoteDbContext>();

	IPasswordHasher<User> passwordHasher =
		scope.ServiceProvider
			.GetRequiredService<IPasswordHasher<User>>();

	await DbSeeder.SeedAsync(
		dbContext,
		passwordHasher);
}
app.Run();
