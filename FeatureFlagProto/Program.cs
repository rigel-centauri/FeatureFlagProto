using FeatureFlagProto;
using Flagsmith;
using LaunchDarkly.Sdk.Server;
using LaunchDarkly.Sdk.Server.Interfaces;
using LoopUp.FeatureManagement.LaunchDarkly;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.FeatureManagement;
using Microsoft.FeatureManagement.FeatureFilters;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//builder.Services.AddSingleton<IFlagsmithClient>(new FlagsmithClient("d66GwKuth4ZSCnwEijsFt9"));
//builder.Services.AddSingleton<IFeatureDefinitionProvider, FlagsmithFeatureManagement>();
builder.Services.AddSingleton<ILdClient>(new LdClient(Configuration.Default("sdk-37ee0bde-8e20-45b8-a2e7-0ec0a36e5e13")));
builder.Services.AddSingleton<IFeatureDefinitionProvider, LaunchDarklyFeatureManagement>();
builder.Services.AddRazorPages();
builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddFeatureManagement().AddFeatureFilter<TargetingFilter>();
builder.Services.AddSingleton<ITargetingContextAccessor, ProtoTargetingContextAccessor>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
