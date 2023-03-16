using CommonLibrary.Settings;
using Identity.Service.Entities;
using Identity.Service.HostedServices;
using Identity.Service.Settings;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Identity.Service.Areas.Identity.Data;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("IdentityDataContextConnection") ?? throw new InvalidOperationException("Connection string 'IdentityDataContextConnection' not found.");

builder.Services.AddDbContext<IdentityDataContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<IdentityDataContext>();

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
    (
        mongoDbSettings?.ConnectionString,
        serviceSettings?.ServiceName
    );

builder.Services.AddIdentityServer(options =>
{
    options.Events.RaiseSuccessEvents = true;
    options.Events.RaiseFailureEvents = true;
    options.Events.RaiseErrorEvents = true;
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

app.MapControllers();

app.MapRazorPages();

app.Run();
