using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using xema.Data;
using xema.Models;
using Amazon.S3;
using Amazon.Runtime;
using Microsoft.Extensions.Options;
using xema.Services;

var builder = WebApplication.CreateBuilder(args);

// ApplicationDbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// ASP.NET Identity
builder.Services
    .AddDefaultIdentity<ApplicationUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.Configure<R2Options>(
    builder.Configuration.GetSection("R2"));

builder.Services.AddSingleton<IAmazonS3>(sp =>
{
    var options = sp
        .GetRequiredService<IOptions<R2Options>>()
        .Value;

    var credentials = new BasicAWSCredentials(
        options.AccessKeyId,
        options.SecretAccessKey);

    return new AmazonS3Client(
        credentials,
        new AmazonS3Config
        {
            ServiceURL = options.Endpoint,
            AuthenticationRegion = "auto",
            ForcePathStyle = true
        });
});

builder.Services.AddScoped<IFileStorage, R2FileStorage>();

var app = builder.Build();

// Seed database data and create admin account
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services
        .GetRequiredService<ApplicationDbContext>();

    var userManager = services
        .GetRequiredService<UserManager<ApplicationUser>>();

    var roleManager = services
        .GetRequiredService<RoleManager<IdentityRole>>();

    await DbInitializer.Seed(
        context,
        userManager,
        roleManager,
        builder.Configuration);
}


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages();

app.Run();