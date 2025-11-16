using DBTest_BACK.Data;
using DBTest_BACK.DTOs;
using DBTest_BACK.Models;
using Microsoft.EntityFrameworkCore;

namespace DBTest_BACK.Services
{
    // ============================================
    // IMPLEMENTACIÓN: SERVICIO DE ACTIVITY LOGS
    // ============================================
    public class ActivityLogService : IActivityLogService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ActivityLogService> _logger;

        public ActivityLogService(AppDbContext context, ILogger<ActivityLogService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task LogActivityAsync(string type, string text, int? userId = null)
        {
            try
            {
                var log = new ActivityLog
                {
                    Type = type,
                    Text = text,
                    UserId = userId,
                    Timestamp = DateTime.UtcNow
                };

                _context.ActivityLogs.Add(log);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Activity logged: {Type} - {Text}", type, text);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar actividad: {Type} - {Text}", type, text);
                // No lanzamos excepción para que no afecte el flujo principal
            }
        }

        public async Task<List<ActivityDto>> GetRecentActivityAsync(int limit = 10)
        {
            try
            {
                return await _context.ActivityLogs
                    .OrderByDescending(a => a.Timestamp)
                    .Take(limit)
                    .Select(a => new ActivityDto
                    {
                        Id = a.Id,
                        Type = a.Type,
                        Text = a.Text,
                        Timestamp = a.Timestamp
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener actividad reciente");
                return new List<ActivityDto>();
            }
        }
    }
}
