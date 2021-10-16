using Caliburn.Micro;
using System.ComponentModel;

namespace TRMDesktopUI.ViewModels
{
    public class SalesViewModel : Screen
    {
        private BindingList<string> _products;
        private string _itemQuantity;
        private BindingList<string> _cart;

        public BindingList<string> Products
        {
            get { return _products; }
            set
            {
                _products = value;
                NotifyOfPropertyChange(() => Products);
            }
        }

        public string ItemQuantity
        {
            get { return _itemQuantity; }
            set
            {
                _itemQuantity = value;
                NotifyOfPropertyChange(() => ItemQuantity);
            }
        }

        public BindingList<string> Cart
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
            get { return "R$ 0,00"; }
        }

        public string Tax
        {
            get { return "R$ 0,00"; }
        }

        public string Total
        {
            get { return "R$ 0,00"; }
        }

        public bool CanAddToCart
        {
            get
            {
                bool output = false;
                // Check
                return output;
            }
        }

        public void AddToCart()
        {
        }

        public bool CanRemoveFromCart
        {
            get
            {
                bool output = false;
                // Check
                return output;
            }
        }

        public void RemoveFromCart()
        {
        }

        public bool CanCheckOut
        {
            get
            {
                bool output = false;
                // Check
                return output;
            }
        }

        public void CheckOut()
        {
        }
    }
}