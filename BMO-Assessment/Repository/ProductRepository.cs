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

        public async Task<IEnumerable<Product?>> GetAllProducts()
        {
            _logger.LogInformation("Getting all products");
            var productResponses = await _context.Products.ToListAsync();       
            return productResponses;
        }
        public async Task<Product?> GetById(int id)
        {
            var productResponse = await _context.Products.FindAsync(id);
            return productResponse;
        }

        public async Task<Product> Create(Product product)
        {
            _logger.LogInformation("About to save a new product: {@Request}", product);
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
   
        }

        public async Task<Product> Update(int id, ProductRequest product)
        {
            var existing = await _context.Products.FindAsync(id);
            if (existing == null) throw new KeyNotFoundException("Product not found."); ;

            existing.Name = product.ProductName;
            existing.Price = product.Price;
            existing.Category = product.Category;
            existing.Description = product.Description;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> Delete(int id)
        {
            var existing = await _context.Products.FindAsync(id);
            if (existing == null)
                throw new KeyNotFoundException($"Product with ID : {id} not found");

            _logger.LogInformation($"About delete product with ID : {id}");
            _context.Products.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

