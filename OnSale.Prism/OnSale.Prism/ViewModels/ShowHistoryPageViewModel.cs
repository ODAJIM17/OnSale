using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OnSale.Prism.ViewModels
{
    public class ShowHistoryPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        public ShowHistoryPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            _navigationService = navigationService;
            Title = "Purchase History";
        }
    }
}
