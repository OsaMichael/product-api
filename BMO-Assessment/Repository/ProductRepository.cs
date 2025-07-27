using AutoMapper;
using BMO_Assessment.Controllers;
using BMO_Assessment.Data;
using BMO_Assessment.DBConfiguration;
using BMO_Assessment.Models;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Oracle.ManagedDataAccess.Client;
using System.Configuration;
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
        private readonly OracleDbService _oracleDbService;
        private readonly SqlQueries _sql;
        public ProductRepository(ProductContext context, IMapper mapper,
            ILogger<ProductRepository> logger, IConfiguration config, OracleDbService dbService, IOptions<SqlQueries> sqlOptions)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
            _config = config;
            _oracleDbService = dbService;
            _sql = sqlOptions.Value;    
        }

        // NOTE: I used Dapper, Oracle database, also used entity framwork to enable me test with raw data, because of the in-built 
       // sqlite on it, thereby running migration to update my table.
       // the project has test data when you run code.
        public async Task<IEnumerable<Product>> GetAllProducts2()
        {
            using var connection = _oracleDbService.CreateConnection();
            connection.Open();
            return await connection.QueryAsync<Product>(_sql.GetAllProducts);
        }

        public async Task<IEnumerable<Product?>> GetAllProducts()
        {
            _logger.LogInformation("Getting all products");
            var productResponses = await _context.Products.ToListAsync();
            return productResponses;
        }

        public async Task<Product?> GetById2(int id)
        {
            using var connection = _oracleDbService.CreateConnection();
            connection.Open();
            var product = await connection.QueryFirstOrDefaultAsync<Product>(_sql.GetById, new { Id = id });
            return product;
        }

        public async Task<Product?> GetById(int id)
        {
            var productResponse = await _context.Products.FindAsync(id);
            return productResponse;
        }

        public async Task<Product> Create2(Product product)
        {
            using var connection = _oracleDbService.CreateConnection();
             connection.Open();
            await connection.ExecuteAsync(_sql.AddProduct, product);
            return product;
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

        public async Task<bool> Update2(Product product)
        {
            using var connection = _oracleDbService.CreateConnection();
            connection.Open();

            var result = await connection.ExecuteAsync(_sql.UpdateProduct, product);
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

        public async Task<bool> Delete2(int id)
        {
            using var connection = _oracleDbService.CreateConnection();
            connection.Open();

            var rowsAffected = await connection.ExecuteAsync(_sql.DeleteProduct, new { Id = id });

            if (rowsAffected == 0)
                throw new KeyNotFoundException($"Product with ID : {id} not found");

            _logger.LogInformation("Product with ID: {ProductId} deleted successfully.", id);
            return true;
        }

    }
}

