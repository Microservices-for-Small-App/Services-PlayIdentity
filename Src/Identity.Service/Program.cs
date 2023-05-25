using CommonLibrary.HealthChecks;
using CommonLibrary.MassTransit;
using CommonLibrary.Settings;
using GreenPipes;
using Identity.Service.Entities;
using Identity.Service.Exceptions;
using Identity.Service.HostedServices;
using Identity.Service.Settings;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

const string AllowedOriginSetting = "AllowedOrigin";

// Add services to the container.
BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));

var serviceSettings = builder.Configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();
var mongoDbSettings = builder.Configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>();
var identityServerSettings = builder.Configuration.GetSection(nameof(IdentityServerSettings)).Get<IdentityServerSettings>();

builder.Services
    .Configure<IdentitySettings>(builder.Configuration.GetSection(nameof(IdentitySettings)))
    .AddDefaultIdentity<ApplicationUser>()
    .AddRoles<ApplicationRole>()
    .AddMongoDbStores<ApplicationUser, ApplicationRole, Guid>
        (mongoDbSettings?.ConnectionString, serviceSettings?.ServiceName);

//builder.Services.AddMassTransitWithRabbitMq(retryConfigurator =>
//{
//    retryConfigurator.Interval(3, TimeSpan.FromSeconds(5));
//    retryConfigurator.Ignore(typeof(UnknownUserException));
//    retryConfigurator.Ignore(typeof(InsufficientFundsException));
//});

builder.Services.AddMassTransitWithMessageBroker(builder.Configuration, retryConfigurator =>
{
    retryConfigurator.Interval(3, TimeSpan.FromSeconds(5));
    retryConfigurator.Ignore(typeof(UnknownUserException));
    retryConfigurator.Ignore(typeof(InsufficientFundsException));
});

builder.Services.AddIdentityServer(options =>
{
    options.Events.RaiseSuccessEvents = true;
    options.Events.RaiseFailureEvents = true;
    options.Events.RaiseErrorEvents = true;

    options.KeyManagement.KeyPath = Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location);

}).AddAspNetIdentity<ApplicationUser>()
  .AddInMemoryApiScopes(identityServerSettings!.ApiScopes)
  .AddInMemoryApiResources(identityServerSettings.ApiResources)
  .AddInMemoryClients(identityServerSettings.Clients)
  .AddInMemoryIdentityResources(identityServerSettings.IdentityResources)
  .AddDeveloperSigningCredential();

builder.Services.AddLocalApiAuthentication();

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddControllers();

builder.Services.AddRazorPages();

builder.Services.AddHostedService<IdentitySeedHostedService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
_ = builder.Services.AddEndpointsApiExplorer();
_ = builder.Services.AddSwaggerGen();

builder.Services.AddHealthChecks()
                .AddMongoDb();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    _ = app.UseMigrationsEndPoint();

    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseCors(options =>
    {
        options.WithOrigins(builder.Configuration[AllowedOriginSetting]!)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseIdentityServer();

app.UseAuthorization();

app.UseCookiePolicy(new CookiePolicyOptions
{
    MinimumSameSitePolicy = SameSiteMode.Lax
});

app.MapControllers();

app.MapRazorPages();

app.MapPlayEconomyHealthChecks();

//app.MapHealthChecks("/health/ready", new HealthCheckOptions()
//{
//    Predicate = (check) => check.Tags.Contains("ready")
//});

//app.MapHealthChecks("/health/live", new HealthCheckOptions()
//{
//    Predicate = (check) => false
//});

app.Run();
