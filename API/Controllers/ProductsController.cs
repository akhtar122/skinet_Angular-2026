using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Controllers;
using API.RequestHelpers;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Mvc;

public class ProductsController : BaseApiController
{
    private readonly ILogger<ProductsController> _logger;
    //private readonly IProductRepository _repo;
    private readonly IGenericRepository<Product> repo;

    public ProductsController(ILogger<ProductsController> logger, IGenericRepository<Product> _repo)
    {
        _logger = logger;
        repo = _repo;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetProducts([FromQuery] string? brand, [FromQuery] ProductSpecParams specParams)
    {
        //var products = await _repo.GetProductsAsync(brand, type, sort);
        var spec = new ProductSpecification(specParams);
        //var product =  await repo.ListAsync(spec);
        //var count = await repo.CountAsync(spec);
        //var pagination = new Pagination<Product>(specParams.PageIndex, specParams.PageSize, count, product);

        return await CretePageResult(repo, spec, specParams.PageIndex, specParams.PageSize);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        var product = await repo.GetByIdAsync(id);
        if (product == null) return NotFound();
        return Ok(product);
    }

    //[HttpGet("byname/{name}")]
    //public async Task<ActionResult<Product>> GetProductByName(string name)
    //{
    //    // Interface doesn't expose GetByName; use GetProductsAsync to look up by name.
    //    var products = await repo.;
    //    var product = products.FirstOrDefault(p => p.Name == name);
    //    if (product == null) return NotFound();
    //    return Ok(product);
    //}

    [HttpPost]
    public async Task<ActionResult<Product>> CreateProduct([FromBody] Product product)
    {
        repo.Add(product);
        if (await repo.SaveAllAsync())
        {
            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }

        return BadRequest("Could not create product.");
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateProduct(int id, [FromBody] Product product)
    {
        if (id != product.Id) return BadRequest();

        if (!repo.Exists(id)) return NotFound();

        repo.Update(product);
        if (await repo.SaveAllAsync())
            return NoContent();

        return BadRequest("Could not update product.");
    }

    [HttpGet("brands")]
    public async Task<ActionResult<IEnumerable<string>>> GetBrands()
    {
        var spec = new BrandListSpecification();
        //var products = await repo.ListAllAsync();
        //var brands = products.Select(p => p.Brand).Distinct().ToList();
        return Ok(await repo.ListAsync(spec));
    }

    [HttpGet("types")]
    public async Task<ActionResult<IEnumerable<string>>> GetTypes()
    {
        var spec = new TypeListSpecification();       
        return Ok(await repo.ListAsync(spec));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteProduct(int id)
    {
        var product = await repo.GetByIdAsync(id);
        if (product == null) return NotFound();

        repo.Remove(product);
        if (await repo.SaveAllAsync())
            return NoContent();

        return BadRequest("Could not delete product.");
    }
}
