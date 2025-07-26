using AutoMapper;
using BMO_Assessment.Controllers;
using BMO_Assessment.Data;
using BMO_Assessment.Models;
using Microsoft.EntityFrameworkCore;

namespace BMO_Assessment.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly ProductContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductRepository> _logger;
        public ProductRepository(ProductContext context, IMapper mapper, ILogger<ProductRepository> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<ProductResponse>> GetAll()
        {
            _logger.LogInformation("Getting all products");
            var products = await _context.Products.ToListAsync();
            var productResponses = _mapper.Map<List<ProductResponse>>(products);
            return productResponses;
        }
        public async Task<ProductResponse> GetById(int id)
        {
            var productResponse = await _context.Products.FindAsync(id);
            if (productResponse == null)
            {
                throw new KeyNotFoundException("Product not found.");
            }
            var productResponses = _mapper.Map<ProductResponse>(productResponse);
            return productResponses;

        }

        public async Task<Response> Create(ProductRequest model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            var product = new Product
            {
                  Category = model.Category,
                  CreatedDate = DateTime.Now,
                  Description = model.Description,
                  Name = model.ProductName,
                  Price = model.Price
            };
            try
            {
                _logger.LogInformation("About to save a new product: {@Request}", product);
                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                return new Response
                {
                    Status = true,
                    message = "Product created successfully",
                    Data = product
                };
       
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred: {Message} \nStackTrace: {StackTrace}", ex.Message, ex.StackTrace);
                throw;
            }

        }

        public async Task<Response> Update(int id, ProductRequest updatedProduct)
        {
            try
            {
                var existing = await _context.Products.FindAsync(id);
                if (existing == null)
                    throw new KeyNotFoundException("Product not found.");

                existing.Name = updatedProduct.ProductName;
                existing.Price = updatedProduct.Price;
                existing.Category = updatedProduct.Category;
                existing.Description = updatedProduct.Description;

                await _context.SaveChangesAsync();
                _logger.LogInformation("updated successfully for product: {@Id}", id);
                return new Response
                {
                    Status = true,
                    message = $"Product with ID {id} updated successfully.",
                    Data = existing
                };
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred: {Message} \nStackTrace: {StackTrace}", ex.Message, ex.StackTrace);
                throw;
            }
            
        }

        public async Task<Response> Delete(int id)
        {
            var existing = await _context.Products.FindAsync(id);
            if (existing == null)
                throw new KeyNotFoundException($"Product with ID : {id} not found");

            _logger.LogInformation($"About delete product with ID : {id}");
            _context.Products.Remove(existing);
            await _context.SaveChangesAsync();
            return new Response
            {
                Status = true,
                message = $"Product with ID {id} removed successfully.",
                Data = id
            };

        }
    }
}

