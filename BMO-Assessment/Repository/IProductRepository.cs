using BMO_Assessment.Data;
using BMO_Assessment.Models;
using Microsoft.EntityFrameworkCore;

namespace BMO_Assessment.Repository
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product?>> GetAllProducts();
        Task<Product?> GetById(int id);
        Task<Product> Create(Product product);
        Task<Product> Update(int id, ProductRequest product);
        Task<bool> Delete(int id);

    }
}
