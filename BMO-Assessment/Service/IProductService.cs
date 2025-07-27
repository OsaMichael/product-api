using BMO_Assessment.Models;

namespace BMO_Assessment.Service
{
    public interface IProductService
    {
        Task<IEnumerable<ProductResponse>> GetAllProducts();
        Task<ProductResponse> GetById(int id);
        Task<Response> AddProduct(ProductRequest model);
        Task<Response> Update(int id, ProductRequest updatedProduct);
        Task<Response> Delete(int id);
    }
}
