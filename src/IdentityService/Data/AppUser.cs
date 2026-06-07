using Microsoft.AspNetCore.Identity;

namespace IdentityService.Data;

public sealed class AppUser : IdentityUser<Guid>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public Guid? DoctorId { get; set; }
    public Guid? ClinicId { get; set; }

    public string FullName => string.IsNullOrWhiteSpace(LastName)
        ? FirstName
        : $"{FirstName} {LastName}";
}
