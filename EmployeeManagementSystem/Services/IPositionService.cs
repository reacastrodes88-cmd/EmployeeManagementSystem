using EmployeeManagementSystem.Models;

namespace EmployeeManagementSystem.Services
{
    public interface IPositionService
    {
        Task<IEnumerable<Position>> GetAllPositionsAsync();
        Task<Position?> GetPositionByIdAsync(int id);
        Task<Position> CreatePositionAsync(Position position);
        Task<Position> UpdatePositionAsync(Position position);
        Task<bool> DeletePositionAsync(int id);
        Task<bool> PositionExistsAsync(int id);
    }
}