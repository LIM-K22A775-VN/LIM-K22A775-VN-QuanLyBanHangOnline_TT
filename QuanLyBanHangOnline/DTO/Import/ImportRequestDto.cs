namespace quanlybanhangonline.Import
{
    // DTO cho từng dòng chi tiết trong phiếu nhập
    public class ImportDetailRequestDto
    {
        public int IdSP { get; set; }
        public int Quantity { get; set; }   
        public decimal ImportPrice { get; set; }
    }

    // DTO cho toàn bộ phiếu nhập hàng
    public class ImportRequestDto
    {
        // IdStaff sẽ được lấy từ Token, nhưng có thể để ở đây nếu muốn Admin nhập hộ
        public List<ImportDetailRequestDto> Items { get; set; } = new List<ImportDetailRequestDto>();
    }
}