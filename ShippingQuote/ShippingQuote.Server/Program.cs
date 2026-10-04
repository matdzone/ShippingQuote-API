using Microsoft.EntityFrameworkCore;
using ShippingQuote.Server.Data;
using ShippingQuote.Server.Services;
using System.Text.Json.Serialization;
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
builder.Services.AddDbContext<ShippingQuoteDbContext>(options =>
	options.UseSqlServer(
		builder.Configuration.GetConnectionString("DefaultConnection")));

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();



var app = builder.Build();

app.UseDefaultFiles();
app.MapStaticAssets();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");
using (var scope = app.Services.CreateScope())
{
	var dbContext =
		scope.ServiceProvider.GetRequiredService<ShippingQuoteDbContext>();

	await DbSeeder.SeedAsync(dbContext);
}
app.Run();
