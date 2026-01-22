namespace quanlybanhangonline.Import
{
    public class ImportResponseDto
    {
        public int IdImport { get; set; }
        public DateTime ImportDate { get; set; }

        // Thông tin định danh người nhập hàng (Admin/Staff)
        public int IdAccount { get; set; }
        public string Email { get; set; } = string.Empty;
        public decimal TotalCost { get; set; }
        public List<ImportDetailResponseDto> Details { get; set; } = new List<ImportDetailResponseDto>();
    }

    public class ImportDetailResponseDto
    {
        public int IdImportDetail { get; set; }
        public int IdSP { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal ImportPrice { get; set; }
        public decimal SubTotal => Quantity * ImportPrice;
    }
}