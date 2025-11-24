using Microsoft.EntityFrameworkCore;
using EmployeeManagementSystem.Data;
using EmployeeManagementSystem.Models;

namespace EmployeeManagementSystem.Services
{
    public class PositionService : IPositionService
    {
        private readonly ApplicationDbContext _context;

        public PositionService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Position>> GetAllPositionsAsync()
        {
            return await _context.Positions
                .Where(p => p.IsActive)
                .OrderBy(p => p.PositionTitle)
                .ToListAsync();
        }

        public async Task<Position?> GetPositionByIdAsync(int id)
        {
            return await _context.Positions
                .FirstOrDefaultAsync(p => p.PositionId == id);
        }

        public async Task<Position> CreatePositionAsync(Position position)
        {
            _context.Positions.Add(position);
            await _context.SaveChangesAsync();
            return position;
        }

        public async Task<Position> UpdatePositionAsync(Position position)
        {
            _context.Positions.Update(position);
            await _context.SaveChangesAsync();
            return position;
        }

        public async Task<bool> DeletePositionAsync(int id)
        {
            var position = await _context.Positions.FindAsync(id);
            if (position == null) return false;

            position.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> PositionExistsAsync(int id)
        {
            return await _context.Positions.AnyAsync(p => p.PositionId == id);
        }
    }
}