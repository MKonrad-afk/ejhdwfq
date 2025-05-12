using Microsoft.Data.SqlClient;
using TestApbdGroupA.Models;

namespace TestApbdGroupA.Reopisotries;

public class ApoinmentRepository : IApoinmentRepository
{
    private readonly string _connectionString;

    public ApoinmentRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }
    
    
    public async Task<ApoinmentDto> GetApoinmnetsDetailsAsync(int id)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        using var command = new SqlCommand(@"
            SELECT v.date, c.first_name, c.last_name, c.date_of_birth,
                   m.docotr_id, m.pwz,
                   s.name, vs.service_fee
            FROM Appointment v
            JOIN Patient c ON v.patient_id = c.patient_id
            JOIN Doctor m ON v.doctor_id = m.doctor_id
            JOIN Appoinment_Service vs ON v.appoinment_id = vs.appoinmnet_id
            JOIN Service s ON vs.service_id = s.service_id
            WHERE v.visit_id = @id", connection);

        command.Parameters.AddWithValue("@id", id);

        using var reader = await command.ExecuteReaderAsync();

        if (!reader.HasRows) return null;

        ApoinmentDto result = null;
        var services = new List<AppointmentServicesDto>();

        while (await reader.ReadAsync())
        {
            if (result == null)
            {
                result = new ApoinmentDto
                {
                    Date = reader.GetDateTime(0),
                    Patient= new PatientDto
                    {
                        FirstName = reader.GetString(1),
                        LastName = reader.GetString(2),
                        DateOfBirth = reader.GetDateTime(3)
                    },
                    Doctor = new DoctorDto
                    {
                        Id= reader.GetInt32(4),
                        Pwz = reader.GetString(5)
                    },
                    Services = services
                };
            }

            services.Add(new AppointmentServicesDto
            {
                Name = reader.GetString(6),
                ServiceFee = reader.GetDecimal(7)
            });
        }

        return result;
    }
    
     public async Task InsertAppoinmnetAsync(NewApoinmentDto dto)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        using var transaction = connection.BeginTransaction();

        try
        {
            var command = new SqlCommand("", connection, transaction);
            
            command.CommandText = "SELECT COUNT(*) FROM Appoinment WHERE appoinmnet_id = @id";
            command.Parameters.AddWithValue("@id", dto.ApoinmentId);
            if ((int)await command.ExecuteScalarAsync() > 0)
                throw new Exception("Appoitment ID already exists");

            command.CommandText = "SELECT COUNT(*) FROM Patient WHERE Patient_id = @cid";
            command.Parameters.Clear();
            command.Parameters.AddWithValue("@cid", dto.PatientId);
            if ((int)await command.ExecuteScalarAsync() == 0)
                throw new Exception("Patient not found");

            command.CommandText = "SELECT doctor_id FROM Doctor WHERE pwz = @lic";
            command.Parameters.Clear();
            command.Parameters.AddWithValue("@lic", dto.Pwz);
            var docId = await command.ExecuteScalarAsync();
            if (docId== null) throw new Exception("docotr not found");
            
            command.CommandText = "INSERT INTO appoinment (appoinment_id, patient_id, client_id, date) VALUES (@vid, @cid, @mid, GETDATE())";
            command.Parameters.Clear();
            command.Parameters.AddWithValue("@vid", dto.ApoinmentId);
            command.Parameters.AddWithValue("@cid", dto.PatientId);
            command.Parameters.AddWithValue("@mid", (int)docId);
            await command.ExecuteNonQueryAsync();

            foreach (var service in dto.Services)
            {

                command.CommandText = "SELECT service_id FROM Service WHERE name = @name";
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@name", service.Name);
                var sid = await command.ExecuteScalarAsync();
                if (sid == null) throw new Exception($"Service '{service.Name}' not found");

                command.CommandText = "INSERT INTO Appoinment_Service (appoinment_id, service_id, service_fee) VALUES (@vid, @sid, @fee)";
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@vid", dto.ApoinmentId);
                command.Parameters.AddWithValue("@sid", sid);
                command.Parameters.AddWithValue("@fee", service.ServiceFee);
                await command.ExecuteNonQueryAsync();
            }

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

}