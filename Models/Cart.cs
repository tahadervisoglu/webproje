namespace StoreApp.Models
{
    public class CartItem
    {
        public int CartItemID { get; set; }
        public int ProductID { get; set; }
        public decimal? Price { get; set; }
        public int? Quantity { get; set; } = 1;

        public Product Product { get; set; }  // Navıgasyon özelliği, ürüne erişim sağlar
    }
}
