WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

string connectionString = Guard.Against.NullOrEmpty(
    builder.Configuration
        .GetConnectionString("DefaultConnection"));

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services
    .AddIdentity<User, Role>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddClaimsPrincipalFactory<ApplicationUserClaimsPrincipalFactory>();

builder.Services.AddControllers();
builder.Services.AddOpenApi(options => { options.AddDocumentTransformer<BearerSecuritySchemeTransformer>(); });

// Add JWT authentication
builder.Services.AddAuthorization()
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false; // Set to true in production
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"]!)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

builder.Services.AddScoped<DbInitializer>();

builder.Services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();
builder.Services.AddSingleton<IAuthorizationPolicyProvider, FlexibleAuthorizationPolicyProvider>();


WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("Authorization Api Reference")
            .WithTheme(ScalarTheme.BluePlanet)
            .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient)
            .EnableDarkMode();
        options.Authentication = new ScalarAuthenticationOptions
        {
            PreferredSecuritySchemes = [BearerSecuritySchemeTransformer.BearerSecuritySchemeName],
        };
    });
}

#region db initializer

using IServiceScope scope = app.Services.CreateScope();
IServiceProvider services = scope.ServiceProvider;
ApplicationDbContext dbContext = services.GetRequiredService<ApplicationDbContext>();
DbInitializer initializer = services.GetRequiredService<DbInitializer>();
ILoggerFactory loggerFactory = services.GetRequiredService<ILoggerFactory>();
ILogger<Program> logger = loggerFactory.CreateLogger<Program>();

try
{
    await dbContext.Database.MigrateAsync();
    await initializer.RunAsync();
}
catch (Exception ex)
{
    logger.LogError(ex, "An error occurred while migrating the database.");
}

#endregion

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();