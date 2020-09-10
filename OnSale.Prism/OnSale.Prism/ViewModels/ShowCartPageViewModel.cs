using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OnSale.Prism.ViewModels
{
    public class ShowCartPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        public ShowCartPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            _navigationService = navigationService;
            Title = "View Cart";
        }
    }
}

