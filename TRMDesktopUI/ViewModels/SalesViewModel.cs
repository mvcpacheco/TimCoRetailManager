using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Caliburn.Micro;
using TRMDesktopUI.Library.API;
using TRMDesktopUI.Library.Models;
using TRMDesktopUI.Models;

namespace TRMDesktopUI.ViewModels
{
    public class SalesViewModel : Screen
    {
        private BindingList<ProductDisplayModel> _products;
        private BindingList<CartItemDisplayModel> _cart = new BindingList<CartItemDisplayModel>();
        private int _itemQuantity = 1;
        private ProductDisplayModel _selectedProduct;
        private CartItemDisplayModel _selectedCartItem;
        private IProductEndpoint _productEndpoint;
        private ISaleEndpoint _saleEndpoint;
        private IMapper _mapper;
        //private IConfigHelper _configHelper;

        public SalesViewModel(IProductEndpoint productEndpoint, ISaleEndpoint saleEndpoint, IMapper mapper)
        {
            _productEndpoint = productEndpoint;
            _saleEndpoint = saleEndpoint;
            _mapper = mapper;
            //_configHelper = configHelper;
        }

        protected override async void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            await LoadProducts();
        }

        private async Task LoadProducts()
        {
            var productList = await _productEndpoint.GetAll();
            var products = _mapper.Map<List<ProductDisplayModel>>(productList);
            Products = new BindingList<ProductDisplayModel>(products);
        }

        public BindingList<ProductDisplayModel> Products
        {
            get { return _products; }
            set
            {
                _products = value;
                NotifyOfPropertyChange(() => Products);
            }
        }

        public ProductDisplayModel SelectedProduct
        {
            get { return _selectedProduct; }
            set
            {
                _selectedProduct = value;
                NotifyOfPropertyChange(() => SelectedProduct);
                NotifyOfPropertyChange(() => CanAddToCart);
            }
        }

        public CartItemDisplayModel SelectedCartItem
        {
            get { return _selectedCartItem; }
            set
            {
                _selectedCartItem = value;
                NotifyOfPropertyChange(() => SelectedCartItem);
                NotifyOfPropertyChange(() => CanRemoveFromCart);
            }
        }

        public int ItemQuantity
        {
            get { return _itemQuantity; }
            set
            {
                _itemQuantity = value;
                NotifyOfPropertyChange(() => ItemQuantity);
                NotifyOfPropertyChange(() => CanAddToCart);
            }
        }

        public BindingList<CartItemDisplayModel> Cart
        {
            get { return _cart; }
            set
            {
                _cart = value;
                NotifyOfPropertyChange(() => Cart);
            }
        }

        public string SubTotal
        {
            get
            {
                return CalculateSubTotal().ToString("C");
            }
        }

        private decimal CalculateSubTotal()
        {
            return Cart.Sum(x => x.Product.RetailPrice * x.QuantityInCart);
        }

        public string Tax
        {
            get
            {
                return CalculateTax().ToString("C");
            }
        }

        private decimal CalculateTax()
        {
            return Cart.Where(x => x.Product.IsTaxable)
                .Sum(x => x.Product.RetailPrice * x.QuantityInCart * (x.Product.TaxRate / 100));

            //return Cart.Sum(item => item.Product.RetailPrice * item.QuantityInCart *
            //                (item.Product.IsTaxable ? item.Product.TaxRate / 100 : 0));

            // O curso do YouTube utilizava uma propriedade na aplicação para uma porcentagem fixa de imposto.
            // Alterei para um valor por produto
            //var taxRate = _configHelper.GetTaxRate();
            //var taxAmount = Cart.Sum(item => item.Product.RetailPrice * item.QuantityInCart * item.Product.TaxRate);
        }

        public string Total
        {
            get
            {
                return (CalculateSubTotal() + CalculateTax()).ToString("C");
            }
        }

        public bool CanAddToCart
        {
            get
            {
                var output = false;
                if (ItemQuantity > 0 && SelectedProduct?.QuantityInStock >= ItemQuantity)
                {
                    output = true;
                }
                return output;
            }
        }

        public void AddToCart()
        {
            var existingItem = Cart.FirstOrDefault(x => x.Product == SelectedProduct);

            if (existingItem != null)
            {
                existingItem.QuantityInCart += ItemQuantity;
            }
            else
            {
                var item = new CartItemDisplayModel
                {
                    Product = SelectedProduct,
                    QuantityInCart = ItemQuantity
                };

                Cart.Add(item);
            }

            SelectedProduct.QuantityInStock -= ItemQuantity;
            ItemQuantity = 1;
            NotifyOfPropertyChange(() => SubTotal);
            NotifyOfPropertyChange(() => Tax);
            NotifyOfPropertyChange(() => Total);
            NotifyOfPropertyChange(() => Cart);
            NotifyOfPropertyChange(() => CanCheckOut);
        }

        public bool CanRemoveFromCart
        {
            get
            {
                var output = false;
                if (SelectedCartItem != null && SelectedCartItem.QuantityInCart > 0)
                {
                    output = true;
                }
                return output;
            }
        }

        public void RemoveFromCart()
        {

            SelectedCartItem.Product.QuantityInStock++;

            if (SelectedCartItem.QuantityInCart > 1)
            {
                SelectedCartItem.QuantityInCart--;
            }
            else
            {
                Cart.Remove(SelectedCartItem);
            }

            NotifyOfPropertyChange(() => SubTotal);
            NotifyOfPropertyChange(() => Tax);
            NotifyOfPropertyChange(() => Total);
            NotifyOfPropertyChange(() => CanCheckOut);
            NotifyOfPropertyChange(() => CanAddToCart);
        }

        public bool CanCheckOut
        {
            get
            {
                var output = false;

                if (Cart?.Count > 0)
                {
                    output = true;
                }

                return output;
            }
        }

        public async Task CheckOut()
        {
            var sale = new SaleModel();
            foreach (var item in Cart)
            {
                sale.SaleDetails.Add(new SaleDetailModel
                {
                    ProductId = item.Product.Id,
                    Quantity = item.QuantityInCart
                });
            }

            await _saleEndpoint.PostSale(sale);

            await ResetSalesViewModel();
        }

        private async Task ResetSalesViewModel()
        {
            await LoadProducts();
            Cart = new BindingList<CartItemDisplayModel>();

            NotifyOfPropertyChange(() => SubTotal);
            NotifyOfPropertyChange(() => Tax);
            NotifyOfPropertyChange(() => Total);
            NotifyOfPropertyChange(() => CanCheckOut);
        }
    }
}