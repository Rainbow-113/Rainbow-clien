namespace Rainbow.Models
{
    public class ShopViewModel
    {
        public IEnumerable<CategoryDto> CategoryDto { get; set; } 
        public IEnumerable<ProductDto> ProductsDto { get; set; }
    }
}
