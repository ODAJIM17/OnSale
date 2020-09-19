using Newtonsoft.Json;
using OnSale.Common.Entities;
using OnSale.Common.Helpers;
using OnSale.Common.Responses;
using OnSale.Prism.Views;
using Prism.Commands;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Text;

namespace OnSale.Prism.ItemViewModels
{
    public class ProductItemViewModel : ProductResponse
    {
        private readonly INavigationService _navigationService;
        private DelegateCommand _selectProductCommand;
        private DelegateCommand _selectProduct2Command;

        public ProductItemViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        public float Quantity { get; set; }
        public string Remarks { get; set; }
        public decimal Value => (decimal)Quantity * Price;


        public DelegateCommand SelectProductCommand => _selectProductCommand ?? 
            (_selectProductCommand = new DelegateCommand(SelectProductAsync));

        public DelegateCommand SelectProduct2Command => _selectProduct2Command ??
           (_selectProduct2Command = new DelegateCommand(SelectProduct2Async));

        private async void SelectProduct2Async()
        {
            NavigationParameters parameters = new NavigationParameters
            {
                { "product", this }
            };

            await _navigationService.NavigateAsync(nameof(ModifiyOrderPage), parameters);
        }

        private async void SelectProductAsync()
        {
            NavigationParameters parameters = new NavigationParameters
              {
                { "product", this }
              };

            Settings.Product = JsonConvert.SerializeObject(this);
            await _navigationService.NavigateAsync(nameof(ProductTabbedPage), parameters);
        }
    }
}

