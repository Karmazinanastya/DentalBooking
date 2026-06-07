using System.Text;
using IdentityService.Data;
using IdentityService.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppIdentityDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddIdentity<AppUser, IdentityRole<Guid>>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<AppIdentityDbContext>()
.AddDefaultTokenProviders();

var jwtSecret = builder.Configuration["Jwt:Secret"]!;
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
    };
});

builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddScoped<TokenService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppIdentityDbContext>();
    await db.Database.EnsureCreatedAsync();

    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

    foreach (var roleName in new[] { "Admin", "Doctor" })
    {
        if (!await roleManager.RoleExistsAsync(roleName))
            await roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
    }

    const string adminEmail = "admin@dental.ua";
    if (await userManager.FindByEmailAsync(adminEmail) is null)
    {
        var admin = new AppUser
        {
            Id = Guid.NewGuid(),
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true,
            FirstName = "Адміністратор",
            LastName = string.Empty
        };
        await userManager.CreateAsync(admin, "Admin123!");
        await userManager.AddToRoleAsync(admin, "Admin");
    }

    // Seed doctor accounts — DoctorId values match ClinicDbSeeder fixed GUIDs
    var seedDoctors = new[]
    {
        (Email: "kovalchuk@dental.ua",  Password: "Doctor123!", First: "Олена",  Last: "Ковальчук",
         DoctorId: new Guid("d0000001-0000-0000-0000-000000000001")),
        (Email: "bondarenko@dental.ua", Password: "Doctor123!", First: "Максим", Last: "Бондаренко",
         DoctorId: new Guid("d0000002-0000-0000-0000-000000000001")),
        (Email: "petrenko@dental.ua",   Password: "Doctor123!", First: "Аліна",  Last: "Петренко",
         DoctorId: new Guid("d0000003-0000-0000-0000-000000000001")),
    };

    foreach (var d in seedDoctors)
    {
        if (await userManager.FindByEmailAsync(d.Email) is not null)
            continue;

        var user = new AppUser
        {
            Id = Guid.NewGuid(),
            UserName = d.Email,
            Email = d.Email,
            EmailConfirmed = true,
            FirstName = d.First,
            LastName = d.Last,
            DoctorId = d.DoctorId
        };
        await userManager.CreateAsync(user, d.Password);
        await userManager.AddToRoleAsync(user, "Doctor");
    }
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
