using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DBTest_BACK.Data;
using DBTest_BACK.DTOs;
using DBTest_BACK.Models;
using System.Security.Claims;

namespace DBTest_BACK.Controllers
{
    [ApiController]
    [Route("api/addresses")]
    [Authorize]
    public class AddressesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<AddressesController> _logger;

        public AddressesController(AppDbContext context, ILogger<AddressesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAddresses()
        {
            try
            {
                var userId = GetCurrentUserId();
                
                var addresses = await _context.Addresses
                    .Where(a => a.UserId == userId)
                    .OrderByDescending(a => a.IsDefault)
                    .ThenByDescending(a => a.CreatedAt)
                    .Select(a => new AddressResponseDto
                    {
                        Id = a.Id,
                        UserId = a.UserId,
                        Label = a.Label,
                        Street = a.Street,
                        City = a.City,
                        State = a.State,
                        PostalCode = a.PostalCode,
                        Country = a.Country,
                        Phone = a.Phone,
                        IsDefault = a.IsDefault,
                        CreatedAt = a.CreatedAt,
                        UpdatedAt = a.UpdatedAt
                    })
                    .ToListAsync();

                return Ok(new { success = true, message = "Direcciones obtenidas exitosamente", data = addresses });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting addresses");
                return StatusCode(500, new { success = false, message = "Error interno del servidor" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateAddress([FromBody] CreateAddressDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Datos inválidos", data = new { errors = ModelState } });

            try
            {
                var userId = GetCurrentUserId();
                
                // Si es la primera dirección, hacerla predeterminada automáticamente
                var hasAddresses = await _context.Addresses.AnyAsync(a => a.UserId == userId);
                if (!hasAddresses)
                {
                    dto.IsDefault = true;
                }

                // Si se marca como predeterminada, desmarcar las demás
                if (dto.IsDefault)
                {
                    var otherAddresses = await _context.Addresses
                        .Where(a => a.UserId == userId)
                        .ToListAsync();
                    
                    foreach (var addr in otherAddresses)
                    {
                        addr.IsDefault = false;
                    }
                }

                var address = new Address
                {
                    UserId = userId,
                    Label = dto.Label,
                    Street = dto.Street,
                    City = dto.City,
                    State = dto.State,
                    PostalCode = dto.PostalCode,
                    Country = dto.Country,
                    Phone = dto.Phone,
                    IsDefault = dto.IsDefault,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Addresses.Add(address);
                await _context.SaveChangesAsync();

                var responseDto = new AddressResponseDto
                {
                    Id = address.Id,
                    UserId = address.UserId,
                    Label = address.Label,
                    Street = address.Street,
                    City = address.City,
                    State = address.State,
                    PostalCode = address.PostalCode,
                    Country = address.Country,
                    Phone = address.Phone,
                    IsDefault = address.IsDefault,
                    CreatedAt = address.CreatedAt,
                    UpdatedAt = address.UpdatedAt
                };

                return CreatedAtAction(nameof(GetAddresses), new { id = address.Id }, 
                    new { success = true, message = "Dirección creada exitosamente", data = responseDto });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating address");
                return StatusCode(500, new { success = false, message = "Error interno del servidor" });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAddress(int id, [FromBody] UpdateAddressDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Datos inválidos", data = new { errors = ModelState } });

            try
            {
                var userId = GetCurrentUserId();
                var address = await _context.Addresses
                    .FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);

                if (address == null)
                    return NotFound(new { success = false, message = "Dirección no encontrada o no pertenece al usuario" });

                // Si se marca como predeterminada, desmarcar las demás
                if (dto.IsDefault && !address.IsDefault)
                {
                    var otherAddresses = await _context.Addresses
                        .Where(a => a.UserId == userId && a.Id != id)
                        .ToListAsync();
                    
                    foreach (var addr in otherAddresses)
                    {
                        addr.IsDefault = false;
                    }
                }

                address.Label = dto.Label;
                address.Street = dto.Street;
                address.City = dto.City;
                address.State = dto.State;
                address.PostalCode = dto.PostalCode;
                address.Country = dto.Country;
                address.Phone = dto.Phone;
                address.IsDefault = dto.IsDefault;
                address.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                var responseDto = new AddressResponseDto
                {
                    Id = address.Id,
                    UserId = address.UserId,
                    Label = address.Label,
                    Street = address.Street,
                    City = address.City,
                    State = address.State,
                    PostalCode = address.PostalCode,
                    Country = address.Country,
                    Phone = address.Phone,
                    IsDefault = address.IsDefault,
                    CreatedAt = address.CreatedAt,
                    UpdatedAt = address.UpdatedAt
                };

                return Ok(new { success = true, message = "Dirección actualizada exitosamente", data = responseDto });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating address {AddressId}", id);
                return StatusCode(500, new { success = false, message = "Error interno del servidor" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var address = await _context.Addresses
                    .FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);

                if (address == null)
                    return NotFound(new { success = false, message = "Dirección no encontrada" });

                // No permitir eliminar la dirección predeterminada si hay otras
                if (address.IsDefault)
                {
                    var otherAddressesCount = await _context.Addresses
                        .CountAsync(a => a.UserId == userId && a.Id != id);
                    
                    if (otherAddressesCount > 0)
                    {
                        return BadRequest(new 
                        { 
                            success = false, 
                            message = "No se puede eliminar la dirección predeterminada. Establece otra dirección como predeterminada primero." 
                        });
                    }
                }

                _context.Addresses.Remove(address);
                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "Dirección eliminada exitosamente", data = (object?)null });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting address {AddressId}", id);
                return StatusCode(500, new { success = false, message = "Error interno del servidor" });
            }
        }

        [HttpPut("{id}/set-default")]
        public async Task<IActionResult> SetDefaultAddress(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var address = await _context.Addresses
                    .FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);

                if (address == null)
                    return NotFound(new { success = false, message = "Dirección no encontrada" });

                // Desmarcar todas las demás direcciones del usuario
                var otherAddresses = await _context.Addresses
                    .Where(a => a.UserId == userId && a.Id != id)
                    .ToListAsync();
                
                foreach (var addr in otherAddresses)
                {
                    addr.IsDefault = false;
                }

                // Marcar esta como predeterminada
                address.IsDefault = true;
                address.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                var responseDto = new AddressResponseDto
                {
                    Id = address.Id,
                    UserId = address.UserId,
                    Label = address.Label,
                    Street = address.Street,
                    City = address.City,
                    State = address.State,
                    PostalCode = address.PostalCode,
                    Country = address.Country,
                    Phone = address.Phone,
                    IsDefault = address.IsDefault,
                    CreatedAt = address.CreatedAt,
                    UpdatedAt = address.UpdatedAt
                };

                return Ok(new { success = true, message = "Dirección predeterminada establecida exitosamente", data = responseDto });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting default address {AddressId}", id);
                return StatusCode(500, new { success = false, message = "Error interno del servidor" });
            }
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                           ?? User.FindFirst("nameid")?.Value
                           ?? User.FindFirst("sub")?.Value;
            return int.Parse(userIdClaim ?? "0");
        }
    }
}
