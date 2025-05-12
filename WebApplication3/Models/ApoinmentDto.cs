namespace TestApbdGroupA.Models;

public class ApoinmentDto
{
    public DateTime Date { get; set; }
    public PatientDto Patient { get; set; }
    public DoctorDto Doctor { get; set; }
    public List<AppointmentServicesDto> Services { get; set; }
}
public class PatientDto
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime DateOfBirth { get; set; }
}

public class DoctorDto
{
    public int Id { get; set; }
    public string Pwz { get; set; }
}

public class AppointmentServicesDto
{
    public string Name { get; set; }
    public decimal ServiceFee { get; set; }
}

public class NewApoinmentDto
{
    public int ApoinmentId{ get; set; }
    public int PatientId { get; set; }
    public string Pwz { get; set; }
    public List<AppointmentServicesDto> Services { get; set; }
}