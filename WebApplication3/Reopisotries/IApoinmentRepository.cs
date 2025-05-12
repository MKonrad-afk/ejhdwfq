using TestApbdGroupA.Models;

namespace TestApbdGroupA.Reopisotries;

public interface IApoinmentRepository
{
    Task<ApoinmentDto> GetApoinmnetsDetailsAsync(int id);
    Task  InsertAppoinmnetAsync(NewApoinmentDto dto);
}