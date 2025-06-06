using codegeneratorlib.Helpers;
using codegen.Middleware;
using EventMonitoring.Components.Layout.Identity;
using EventMonitoring.Hubs;
using EventMonitoring.ph.Components;
using EventMonitoring.States.Administration;
using EventMonitoring.States.User;
using Microsoft.AspNetCore.Components.Authorization;
using EventMonitoring.ph.Data.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastractureService(builder.Configuration);
builder.Services.AddApplicationService();
builder.Services.AddControllers().AddNewtonsoftJson();


builder.Services.AddScoped<IRepositoryWrapper, RepositoryWrapper>();

builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthStateProvider>();
builder.Services.AddScoped<ICustomAuthorizationService, CustomAuthorizationService>();
builder.Services.AddScoped<NetcodeHubConnectionService>();
builder.Services.AddScoped<AdminActiveOrderCountState>();
builder.Services.AddScoped<UserActiveOrderCountState>();
builder.Services.AddScoped<GenericHomeHeaderState>();
builder.Services.AddScoped<ChangePasswordState>();
builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetService<AppHelper>();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.MapSignOutEndpoint();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.UseSession();
app.MapControllers();
app.UseNodeModules(app.Environment.ContentRootPath);
app.UseAntiforgery();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();