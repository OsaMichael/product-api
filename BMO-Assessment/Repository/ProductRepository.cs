using AutoMapper;
using BMO_Assessment.Controllers;
using BMO_Assessment.Data;
using BMO_Assessment.Models;
using Dapper;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Data.SqlClient;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace BMO_Assessment.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly ProductContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductRepository> _logger;
        private readonly IConfiguration _config;
        public ProductRepository(ProductContext context, IMapper mapper,
            ILogger<ProductRepository> logger, IConfiguration config)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
            _config = config;
        }

        // i used entity framework to enable me run migration and test with raw data.
        // i use entity framework sqlite to enable me have my data within the project.
        // if i dont want to test with raw data, then using dapper could have been best 
        // option for me. then i could use query or store-proceedures.
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

        // I want to also implement to update a product using dapper
        public async Task<bool> UpdateProduct(Product product)
        {
            using var db = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            var sql = @"UPDATE Products SET 
                      ProductName = @ProductName,
                      Price = @Price,
                      Category = @Category,
                      Description = @Description
                    WHERE Id = @Id";
            var result = await db.ExecuteAsync(sql, product);
            return result > 0;
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

