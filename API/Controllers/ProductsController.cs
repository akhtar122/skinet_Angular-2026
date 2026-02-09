using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ILogger<ProductsController> _logger;
    private readonly IProductRepository _repo;

    public ProductsController(ILogger<ProductsController> logger, IProductRepository repo)
    {
        _logger = logger;
        _repo = repo;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetProducts([FromQuery] string? brand, [FromQuery] string? type, [FromQuery] string? sort)
    {
        var products = await _repo.GetProductsAsync(brand, type, sort);
        return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        var product = await _repo.GetProductByIdAsync(id);
        if (product == null) return NotFound();
        return Ok(product);
    }

    [HttpGet("byname/{name}")]
    public async Task<ActionResult<Product>> GetProductByName(string name)
    {
        // Interface doesn't expose GetByName; use GetProductsAsync to look up by name.
        var products = await _repo.GetProductsAsync(null, null, null);
        var product = products.FirstOrDefault(p => p.Name == name);
        if (product == null) return NotFound();
        return Ok(product);
    }

    [HttpPost]
    public async Task<ActionResult<Product>> CreateProduct([FromBody] Product product)
    {
        _repo.AddProduct(product);
        if (await _repo.SaveChangesAsync())
        {
            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }

        return BadRequest("Could not create product.");
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateProduct(int id, [FromBody] Product product)
    {
        if (id != product.Id) return BadRequest();

        if (!_repo.ProductExists(id)) return NotFound();

        _repo.UpdateProduct(product);
        if (await _repo.SaveChangesAsync())
            return NoContent();

        return BadRequest("Could not update product.");
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteProduct(int id)
    {
        var product = await _repo.GetProductByIdAsync(id);
        if (product == null) return NotFound();

        _repo.DeleteProduct(product);
        if (await _repo.SaveChangesAsync())
            return NoContent();

        return BadRequest("Could not delete product.");
    }
}
