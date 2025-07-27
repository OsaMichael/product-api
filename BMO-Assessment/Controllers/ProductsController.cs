using BMO_Assessment.Data;
using BMO_Assessment.Models;
using BMO_Assessment.Repository;
using BMO_Assessment.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BMO_Assessment.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _service;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(IProductService service, ILogger<ProductsController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet("products")]
        public async Task<IActionResult> GetProducts()
        {
            _logger.LogInformation("Getting all products");
            var result = await _service.GetAllProducts();
            return Ok(result);
        }

        [HttpGet("getId/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _service.GetById(id);
            return product == null ? NotFound() : Ok(product);
        }

        [HttpPost("add")]
        public async Task<IActionResult> Create([FromBody] ProductRequest product)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _logger.LogInformation("Creating a new product: {@Request}", product);
            var result = await _service.AddProduct(product);
            return result.Status ? Ok(result) : BadRequest(result);       
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ProductRequest updatedProduct)
        {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                _logger.LogInformation("About to update product: {@Request}", updatedProduct);
                var result = await _service.Update(id, updatedProduct);
                return result.Status ? Ok(result) : BadRequest(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while updating the product.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.Delete(id);
            return result.Status ? Ok(result) : NotFound(result);
        }
    }
}
