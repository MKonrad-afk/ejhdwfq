using TestApbdGroupA.Models;

namespace TestApbdGroupA.Serivces;

public interface IApoinmentSerivce
{
    public Task<ApoinmentDto> GetApoinmentsByIdAsync(int id);
    public Task AddVisitAsync(NewApoinmentDto dto);
}