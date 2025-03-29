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
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            try
            {
                return await _context.Products.Where(p => !p.IsDeleted).ToListAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error occurred while fetching products: " + ex.Message);
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

                    await _context.SaveChangesAsync();
                    return Ok(product);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Error occurred while updating product with id {oid}: " + ex.Message);
                }
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

                return Ok("Product successfully marked as deleted.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error occurred while deleting product with id {oid}: " + ex.Message);
            }
        }
        [HttpDelete("finalize")]
        public async Task<IActionResult> Finalize()
        {
            try
            {
                var deletedProducts = await _context.Products.Where(p => p.IsDeleted).ToListAsync();

                if (deletedProducts.Count == 0)
                {
                    return NotFound("No products marked as deleted.");
                }

                _context.Products.RemoveRange(deletedProducts);

                await _context.SaveChangesAsync();

                return Ok("Successfully removed all products marked as deleted.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error occurred while clearing deleted products: "+ex.Message);
            }
        }
    }
}
