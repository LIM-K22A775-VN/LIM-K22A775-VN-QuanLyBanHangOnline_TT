using quanlybanhangonline.DTO.Cart;
using QuanLyBanHangOnline.DTO.Cart;

namespace QuanLyBanHangOnline.Services.Interfaces
{
    public interface ICartService
    {
        Task<IEnumerable<object>> GetAllCartsForAdminAsync();
        Task<CartResultDto> GetMyCartAsync(int userId);
        Task<string> AddToCartAsync(AddToCartDto dto, int userId);
        Task<string> UpdateQuantityAsync(UpdateCartDto dto, int userId);
        Task<string> RemoveProductFromCartAsync(int idSP, int userId);
    }
}
