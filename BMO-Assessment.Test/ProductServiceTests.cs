using AutoMapper;
using BMO_Assessment.Data;
using BMO_Assessment.Models;
using BMO_Assessment.Repository;
using BMO_Assessment.Service;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace BMO_Assessment.Tests
{
    [TestFixture]
    public class ProductServiceTests
    {
        private ProductService _service;
        private Mock<IProductRepository> _mockRepo;
        private Mock<IMapper> _mockMapper;
        private Mock<ILogger<ProductService>> _mockLogger;

        [SetUp]
        public void Setup()
        {
            _mockRepo = new Mock<IProductRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<ProductService>>();

            _service = new ProductService(_mockRepo.Object, _mockMapper.Object, _mockLogger.Object);
        }

        [Test]
        public async Task GetAllProductsTest()
        {

            var products = new List<Product> {
                new Product { Id = 1, Name = "Test Product", Price = 10 }
            };

            var responses = new List<ProductResponse> {
                new ProductResponse { Id = 1, ProductName = "Test Product", Price = 10 }
            };

            _mockRepo.Setup(r => r.GetAllProducts()).ReturnsAsync(products);
            _mockMapper.Setup(m => m.Map<IEnumerable<ProductResponse>>(products)).Returns(responses);

            var result = await _service.GetAllProducts();
            var productList = result.ToList();
            Assert.IsNotNull(result);
            Assert.That(productList.Count, Is.EqualTo(1));
            Assert.That(((List<ProductResponse>)result)[0].ProductName, Is.EqualTo("Test Product"));
        }

        [Test]
        public async Task AddProduct_ValidModel()
        {
            var request = new ProductRequest
            {
                ProductName = "Laptop",
                Category = "Electronics",
                Description = "High-end",
                Price = 1500
            };

            var createdProduct = new Product
            {
                Id = 1,
                Name = request.ProductName,
                Category = request.Category,
                Description = request.Description,
                Price = request.Price,
                CreatedDate = DateTime.Now
            };

            _mockRepo.Setup(repo => repo.Create(It.IsAny<Product>())).ReturnsAsync(createdProduct);

            var result = await _service.AddProduct(request);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Status, Is.True);
            Assert.That(result.Data, Is.EqualTo(createdProduct));
            Assert.That(result.message, Is.EqualTo("Product created successfully"));
            _mockRepo.Verify(r => r.Create(It.IsAny<Product>()), Times.Once);
        }

        [Test]
        public void AddProduct_NullRequest()
        {
            var ex = Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await _service.AddProduct(null));

            Assert.That(ex.ParamName, Is.EqualTo("model"));
        }


        [Test]
        public async Task UpdateProductTest()
        {
            
            var productRequest = new ProductRequest
            {
                ProductName = "TV",
                Category = "LG",
                Price = 10000,
                Description = "Quality Television"
            };

            var response = new Product
            {
                Id = 1,
                Name = productRequest.ProductName,
                Category = productRequest.Category,
                Price = productRequest.Price,
                Description = productRequest.Description
            };

          var rr =  _mockRepo
                .Setup(repo => repo.Update(1, productRequest))
                .ReturnsAsync(response);

            var result = await _service.Update(1, productRequest);

           
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Status, Is.True);
            Assert.That(result.Data, Is.EqualTo(response));
            Assert.That(result.message, Is.EqualTo("Product with ID 1 updated successfully."));

            _mockRepo.Verify(r => r.Update(1, productRequest), Times.Once);
        }

        [Test]
        public void Update_ProductRequestIsNull()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _service.Update(1, null));
        }


        [Test]
        public async Task GetById_ProductExists()
        {
            
            var productId = 1;
            var product = new Product
            {
                Id = productId,
                Name = "Laptop",
                Category = "HP",
                Price = 20000,
                Description = "Good for your office use"
            };

            var expectedResponse = new ProductResponse
            {
                Id = productId,
                ProductName = product.Name,
                Category = product.Category,
                Price = product.Price,
                Description = product.Description,
            };

            _mockRepo.Setup(repo => repo.GetById(productId)).ReturnsAsync(product);
            _mockMapper.Setup(mapper => mapper.Map<ProductResponse>(product)).Returns(expectedResponse);

            var result = await _service.GetById(productId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(productId));
            Assert.That(result.ProductName, Is.EqualTo(expectedResponse.ProductName));
            _mockRepo.Verify(r => r.GetById(productId), Times.Once);
            _mockMapper.Verify(m => m.Map<ProductResponse>(product), Times.Once);
        }


        [Test]
        public void GetById_NonExistingId()
        {
            _mockRepo.Setup(r => r.GetById(It.IsAny<int>())).ReturnsAsync((Product?)null);

            var ex = Assert.ThrowsAsync<KeyNotFoundException>(async () => await _service.GetById(999));
            Assert.That(ex.Message, Is.EqualTo("Product not found."));
        }

        [Test]
        public async Task DeleteProductTest()
        {
            _mockRepo.Setup(r => r.Delete(It.IsAny<int>())).ReturnsAsync(true);

            var result = await _service.Delete(1);

            Assert.IsTrue(result.Status);
            Assert.That(result.message, Is.EqualTo("Product with ID 1 removed successfully."));
        }

    }
}