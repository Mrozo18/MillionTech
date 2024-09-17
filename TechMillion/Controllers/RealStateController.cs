using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechMillion.Model;

namespace TechMillion.Controllers
{
    public class RealStateController : Controller
    {
        private readonly RealEstateContext _context;

        public RealStateController(RealEstateContext context) 
        {
            _context = context;
        }

        [HttpPost("create-property")]
        public async Task<IActionResult> CreateProperty([FromBody] Property propertyDto)
        {
            try
            {
                var owner = await _context.Owners.FindAsync(propertyDto.IdOwner);
                if (owner == null) return NotFound("Owner not found");

                var property = new Property
                {
                    Name = propertyDto.Name,
                    Address = propertyDto.Address,
                    Price = propertyDto.Price,
                    CodeInternal = propertyDto.CodeInternal,
                    Year = propertyDto.Year,
                    IdOwner = propertyDto.IdOwner
                };

                _context.Properties.Add(property);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Property created successfully", property });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the property", error = ex.Message });
            }
        }

        [HttpPost("add-image")]
        public async Task<IActionResult> AddImage(int propertyId, [FromBody] PropertyImage imageDto)
        {
            try
            {
                var property = await _context.Properties.FindAsync(propertyId);
                if (property == null) return NotFound("Property not found");

                var propertyImage = new PropertyImage
                {
                    IdProperty = propertyId,
                    Filed = imageDto.Filed,
                    Enabled = true
                };

                _context.PropertyImages.Add(propertyImage);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Add Image successfully", propertyImage });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while sending the image", error = ex.Message });
            } 
        }

        [HttpPut("change-price")]
        public async Task<IActionResult> ChangePrice(int propertyId, decimal newPrice)
        {
            try
            {
                var property = await _context.Properties.FindAsync(propertyId);
                if (property == null) return NotFound("Property not found");

                property.Price = newPrice;
                await _context.SaveChangesAsync();

                return Ok(new { message = "Change price successfully", property });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while changing the price", error = ex.Message });
            }
        }

        [HttpPut("update-property")]
        public async Task<IActionResult> UpdateProperty(int propertyId, [FromBody] Property propertyDto)
        {
            try
            {
                var property = await _context.Properties.FindAsync(propertyId);
                if (property == null) return NotFound("Property not found");

                property.Name = propertyDto.Name;
                property.Address = propertyDto.Address;
                property.Price = propertyDto.Price;
                property.CodeInternal = propertyDto.CodeInternal;
                property.Year = propertyDto.Year;

                await _context.SaveChangesAsync();
                return Ok(new { message = "Update the property successfully", property });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the property", error = ex.Message });
            }

        }

        [HttpGet("list-properties")]
        public async Task<IActionResult> ListProperties([FromQuery] PropertyFilter filter)
        {
            try
            {
                var query = _context.Properties
                          .Include(p => p.PropertyTrace)
                          .AsQueryable();

                if (!string.IsNullOrEmpty(filter.Name))
                {
                    query = query.Where(p => p.Name.Contains(filter.Name));
                }

                if (filter.MinPrice.HasValue && filter.MaxPrice.HasValue)
                {
                    query = query.Where(p => p.Price >= filter.MinPrice.Value && p.Price <= filter.MaxPrice.Value);
                }

                if (!string.IsNullOrEmpty(filter.CodeInternal))
                {
                    query = query.Where(p => p.CodeInternal == filter.CodeInternal);
                }

                var properties = await query.Select(p => new
                {
                    p.Name,
                    p.Address,
                    p.Price,
                    p.CodeInternal,
                    p.Year,
                    Traces = p.PropertyTrace.Select(pt => new
                    {
                        pt.DateSale,
                        pt.Name,
                        pt.Value,
                        pt.Tax
                    }).ToList()
                }).ToListAsync();

                return Ok(new { message = "Show list properties successfully", properties });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while showing the property", error = ex.Message });
            }        
        }
    }
}