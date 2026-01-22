namespace quanlybanhangonline.DTO.Cart
{
    // Dùng để nhận dữ liệu từ Angular gửi lên (Thêm món vào giỏ)
    public class AddToCartDto
    {
        public int IdSP { get; set; }
    }

    // Dùng để trả dữ liệu về cho Angular hiển thị giỏ hàng
        public class CartResultDto
        {
            public int IdCart { get; set; }
            public int Status { get; set; }
            public List<CartDetailResultDto> Details { get; set; } = new List<CartDetailResultDto>();
            public decimal TotalPrice => Details.Sum(x => x.SubTotal);
        }

    public class CartDetailResultDto
    {
        public int IdCartDetail { get; set; }
        public int IdSP { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? Image { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal SubTotal => Price * Quantity;
    }
}