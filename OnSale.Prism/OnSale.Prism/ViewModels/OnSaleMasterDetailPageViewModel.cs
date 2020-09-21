using Newtonsoft.Json;
using OnSale.Common.Helpers;
using OnSale.Common.Models;
using OnSale.Common.Requests;
using OnSale.Common.Responses;
using OnSale.Prism.ItemViewModels;
using OnSale.Prism.Views;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace OnSale.Prism.ViewModels
{
    
        public class OnSaleMasterDetailPageViewModel : ViewModelBase
        {
      
        private readonly INavigationService _navigationService;
            private UserResponse _user;
        private static OnSaleMasterDetailPageViewModel _instance;

        public OnSaleMasterDetailPageViewModel(INavigationService navigationService) : base(navigationService)
            {
            _instance = this;
            _navigationService = navigationService;
                LoadMenus();
                LoadUser();
            }
            public ObservableCollection<MenuItemViewModel> Menus { get; set; }

        public UserResponse User
        {
            get => _user;
            set => SetProperty(ref _user, value);
        }

        public static OnSaleMasterDetailPageViewModel GetInstance()
        {
            return _instance;
        }

        public void LoadUser()
        {
            if (Settings.IsLogin)
            {
                TokenResponse token = JsonConvert.DeserializeObject<TokenResponse>(Settings.Token);
                User = token.User;
            }
        }


        private void LoadMenus()
            {
                List<Menu> menus = new List<Menu>
        {
            new Menu
            {
                Icon = "ic_card_giftcard",
                PageName = $"{nameof(ProductsPage)}",
                Title = "Products"
            },
            new Menu
            {
                Icon = "ic_shopping_cart",
                PageName = $"{nameof(ShowCartPage)}",
                Title = "Shopping Cart"
            },
            new Menu
            {
                Icon = "ic_history",
                PageName = $"{nameof(ShowHistoryPage)}",
                Title = "Orders History",
                IsLoginRequired = true
            },
            new Menu
            {
                Icon = "ic_person",
                PageName = $"{nameof(ModifyUserPage)}",
                Title = "My Profile",
                IsLoginRequired = true
            },
            new Menu
            {
                Icon = "ic_exit_to_app",
                PageName = $"{nameof(LoginPage)}",
                Title = Settings.IsLogin ? "Logout" : "Login"
            }
        };

                Menus = new ObservableCollection<MenuItemViewModel>(
                    menus.Select(m => new MenuItemViewModel(_navigationService)
                    {
                        Icon = m.Icon,
                        PageName = m.PageName,
                        Title = m.Title,
                        IsLoginRequired = m.IsLoginRequired
                    }).ToList());
            }
        }

    }

