using studeehub.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Load .env manually in Development
if (builder.Environment.IsDevelopment())
{
    string solutionRootPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
    string envFile = Path.Combine(solutionRootPath, ".env");

    if (File.Exists(envFile))
    {
        var lines = File.ReadAllLines(envFile);

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line) || line.TrimStart().StartsWith("#"))
                continue;

            var split = line.Split('=', 2);
            if (split.Length != 2)
                continue;

            var key = split[0].Trim();
            var value = split[1].Trim();

            if (value.StartsWith("\"") && value.EndsWith("\""))
                value = value[1..^1];

            Environment.SetEnvironmentVariable(key, value);
        }
    }
}

// Add config sources
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

// Add services to the container.
builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddControllers();
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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
