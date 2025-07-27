using AutoMapper;
using BMO_Assessment.Data;
using BMO_Assessment.Models;
using BMO_Assessment.Repository;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;

namespace BMO_Assessment.Service
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepo;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductService> _logger;
        private readonly IValidator<ProductRequest> _validator;
        public ProductService(IProductRepository productRepo, IMapper mapper, ILogger<ProductService> logger, IValidator<ProductRequest> validator)
        {
            _productRepo = productRepo;
            _mapper = mapper;
            _logger = logger;
            _validator = validator;
        }

        public async Task<IEnumerable<ProductResponse>> GetAllProducts()
        {
            var productEntities = await _productRepo.GetAllProducts();
            return _mapper.Map<IEnumerable<ProductResponse>>(productEntities);
        }

        public async Task<ProductResponse> GetById(int id)
        {
            var productEntities = await _productRepo.GetById(id);
            if (productEntities == null)
            {
                throw new KeyNotFoundException("Product not found.");
            }
            return _mapper.Map<ProductResponse>(productEntities);
        }

        public async Task<Response> AddProduct(ProductRequest model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            // 
            ValidationResult validationResult = await _validator.ValidateAsync(model);
            if (!validationResult.IsValid)
            {
                var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
                throw new ValidationException("Validation failed: " + errors);
            }

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
                var productEntities = await _productRepo.Create(product);
                return new Response
                {
                    message = "Product created successfully",
                    Data = productEntities,
                     Status = true
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
            if (updatedProduct == null) throw new ArgumentNullException(nameof(updatedProduct));
            try
            {
                var productEntities = await _productRepo.Update(id, updatedProduct);
                     return new Response
                     {
                         message = $"Product with ID {id} updated successfully.",
                         Data = productEntities,
                         Status = true
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
            bool deleted = await _productRepo.Delete(id);
            return new Response
            {
                message = $"Product with ID {id} removed successfully.",
                Data = id,
                Status =true
            };

        }

    }
}
