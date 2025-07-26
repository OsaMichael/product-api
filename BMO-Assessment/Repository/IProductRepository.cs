using BMO_Assessment.Data;
using BMO_Assessment.Models;
using Microsoft.EntityFrameworkCore;

namespace BMO_Assessment.Repository
{
    public interface IProductRepository
    {
        Task<IEnumerable<ProductResponse>> GetAll();
        Task<ProductResponse> GetById(int id);
        Task<Response> Create(ProductRequest model);
        Task<Response> Update(int id, ProductRequest updatedProduct);
        Task<Response> Delete(int id);

    }
}
