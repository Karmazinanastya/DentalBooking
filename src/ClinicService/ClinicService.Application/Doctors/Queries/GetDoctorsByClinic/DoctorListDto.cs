namespace ClinicService.Application.Doctors.Queries.GetDoctorsByClinic;

public sealed record DoctorListDto(Guid Id, string FullName, string Specialization);
