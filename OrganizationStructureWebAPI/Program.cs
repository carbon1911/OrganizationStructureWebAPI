using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using OrganizationStructureWebAPI.Models;
using OrganizationStructureWebAPI.Models.Misc;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers().AddJsonOptions(
	options =>
	{
		options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
		options.JsonSerializerOptions.AllowTrailingCommas = true;
	}
);
builder.Services.AddDbContext<OrganizationStructureDbContext>(
	opt => opt.UseSqlServer(
		builder.Configuration.GetConnectionString("OrganizationStructureDatabase")
	)
);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(setupAction =>
{
	setupAction.SwaggerDoc("v1", new OpenApiInfo
	{
		Title = "Organization Structure Hierarchy",
		Version = "v1",
		Description = "API for organization hierarchy management"
	});

	setupAction.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
	setupAction.CustomSchemaIds(schemaIdSelector => schemaIdSelector.FullName);
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
	var services = scope.ServiceProvider;
	try
	{
		var appDbContext = services.GetRequiredService<OrganizationStructureDbContext>();
		DataGenerator.Initialize(appDbContext);
	}
	catch (Exception ex)
	{
		var logger = services.GetRequiredService<ILogger<Program>>();
		logger.LogError(ex, "An error occurred creating the DB.");
	}
	
}

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(ui => ui.DefaultModelsExpandDepth(-1));	

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
