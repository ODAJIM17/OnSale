using OnSale.Common.Responses;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OnSale.Prism.ViewModels
{
    public class ProductTabbedPageViewModel : ViewModelBase
    {
        public ProductTabbedPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            Title = "Product";
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            if (parameters.ContainsKey("product"))
            {
                var product = parameters.GetValue<ProductResponse>("product");
                Title = product.Name;
            }
        }
    }

}
