namespace TRMDesktopUI.Library.Models
{
    public class CartItemModel
    {
        public ProductModel Product { get; set; }
        public int QuantityInCart { get; set; }

        public string DisplayText
        {
            get
            {
                var output = $"{Product.ProductName}";
                if (QuantityInCart > 1)
                {
                    output = $"{output} ({QuantityInCart})";
                }
                return output;
            }
        }
    }
}