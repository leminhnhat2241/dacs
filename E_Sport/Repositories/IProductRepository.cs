using E_Sport.Models;

namespace E_Sport.Repositories
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product> GetByIdAsync(int id);
        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(int id);

        // Tìm kiếm sản phẩm theo tên
        Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm); 
        Task<IEnumerable<Product>> GetAllSortedAsync(string searchTerm);
    }
}
