using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductsAPI.Data;
using ProductsAPI.Models;

namespace ProductAPI.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ProductDbContext _context;

        public ProductsController(ProductDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(Product product)
        {
            try
            {
                product.Oid = Guid.NewGuid();
                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetProduct), new { oid = product.Oid }, product);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error occurred while creating product: " + ex.Message);
            }
        }
        [HttpGet("deleted")]
        public async Task<IActionResult> GetDeletedProducts(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var query = _context.Products
                    .Where(p => p.IsDeleted) 
                    .OrderBy(p => p.Name); 

                var totalRecords = await query.CountAsync();
                var products = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return Ok(new
                {
                    TotalRecords = totalRecords,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    Data = products
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching deleted products.", error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var query = _context.Products
                    .Where(p => !p.IsDeleted) 
                    .OrderBy(p => p.Name); 

                var totalRecords = await query.CountAsync();
                var products = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return Ok(new
                {
                    TotalRecords = totalRecords,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    Data = products
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching products.", error = ex.Message });
            }
        }


        [HttpGet("{Oid}")]
        public async Task<ActionResult<Product>> GetProduct(Guid oid)
        {
            try
            {
                var product = await _context.Products.FirstOrDefaultAsync(p => p.Oid == oid && !p.IsDeleted);
                if (product == null)
                    return NotFound("Product not found.");

                return Ok(product);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error occurred while fetching product: " + ex.Message);
            }
        }

        [HttpPut("{oid}")]
        public async Task<IActionResult> UpdateProduct(Guid oid, [FromBody] Product updatedProduct)
        {
            {
                try
                {
                    var product = await _context.Products.FindAsync(oid);
                    if (product == null || product.IsDeleted)
                    {
                        return NotFound("Product not found or deleted.");
                    }
                    product.Name = updatedProduct.Name;
                    product.Description = updatedProduct.Description;
                    product.Price = updatedProduct.Price;
                    product.CreatedAt = DateTime.UtcNow;

                    await _context.SaveChangesAsync();
                    return Ok(product);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Error occurred while updating product with id {oid}: " + ex.Message);
                }
            }
        }
        [HttpPut("restore/{oid}")]
        public async Task<IActionResult> RestoreProduct(Guid oid)
        {
            try
            {
                var product = await _context.Products.FirstOrDefaultAsync(p => p.Oid == oid && p.IsDeleted);
                if (product == null)
                {
                    return NotFound(new { message = "Product not found or not marked as deleted." });
                }

                product.IsDeleted = false; 
                await _context.SaveChangesAsync();

                return Ok(new { message = "Product restored successfully!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while restoring the product.", error = ex.Message });
            }
        }

        [HttpDelete("{oid}")]
        public async Task<IActionResult> DeleteProduct(Guid oid)
        {
            try
            {
                var product = await _context.Products.FindAsync(oid);
                if (product == null || product.IsDeleted)
                {
                    return NotFound("Product not found or already deleted.");
                }

                product.IsDeleted = true;
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error occurred while deleting product with id {oid}: " + ex.Message);
            }
        }
       
    }
}
