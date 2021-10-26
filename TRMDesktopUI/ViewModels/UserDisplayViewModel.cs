using Caliburn.Micro;
using System.ComponentModel;
using System.Dynamic;
using System.Threading.Tasks;
using System.Windows;
using TRMDesktopUI.Library.API;
using TRMDesktopUI.Library.Models;

namespace TRMDesktopUI.ViewModels
{
    public class UserDisplayViewModel : Screen
    {
        private StatusInfoViewModel _status;
        private IWindowManager _windowManager;
        private IUserEndpoint _userEndpoint;
        private BindingList<UserModel> _users;

        public BindingList<UserModel> Users
        {
            get { return _users; }
            set
            {
                _users = value;
                NotifyOfPropertyChange(() => Users);
            }
        }


        public UserDisplayViewModel(StatusInfoViewModel status, IWindowManager windowManager, IUserEndpoint userEndpoint)
        {
            _status = status;
            _windowManager = windowManager;
            _userEndpoint = userEndpoint;
        }

        protected override async void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);

            try
            {
                await LoadUsers();
            }
            catch
            {
                dynamic settings = new ExpandoObject();
                settings.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                settings.ResizeMode = ResizeMode.NoResize;
                settings.Title = "Erro";

                _status.UpdateMessage("Acesso não autorizado", "Você não tem permissão para interagir com a tela de Usuários.");
                _windowManager.ShowWindow(_status, null, settings);
            }
        }

        private async Task LoadUsers()
        {
            var userList = await _userEndpoint.GetAll();
            Users = new BindingList<UserModel>(userList);
        }
    }
}
