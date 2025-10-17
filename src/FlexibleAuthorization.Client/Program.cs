WebAssemblyHostBuilder builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddBlazoredLocalStorage();

builder.Services.AddAuthorizationCore();

// injection of custom authorization policy provider and handler
builder.Services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();
builder.Services.AddSingleton<IAuthorizationPolicyProvider, FlexibleAuthorizationPolicyProvider>();

builder.Services.AddScoped<AuthenticationStateProvider, JwtAuthenticationStateProvider>();

builder.Services.AddScoped(sp =>
    (IJwtAuthenticationStateProvider)sp.GetRequiredService<AuthenticationStateProvider>());

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddRefitClient<IAuthApi>()
    .ConfigureHttpClient(client => client.BaseAddress = new Uri("https://localhost:7288"));

builder.Services
    .AddRefitClient<IAccessControlApi>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri("https://localhost:7288"));

builder.Services
    .AddRefitClient<IRolesApi>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri("https://localhost:7288"));

builder.Services
    .AddRefitClient<IUsersApi>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri("https://localhost:7288"));

await builder.Build()
    .RunAsync();