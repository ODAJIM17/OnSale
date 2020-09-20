using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OnSale.Common.Helpers;
using OnSale.Common.Responses;
using OnSale.Common.Services;
using OnSale.Prism.Helpers;
using OnSale.Prism.Views;
using Prism.Commands;
using Prism.Navigation;
using Stripe;
using Xamarin.Essentials;
using PaymentMethod = OnSale.Common.Models.PaymentMethod;

namespace OnSale.Prism.ViewModels
{
    public class FinishOrderPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IApiService _apiService;
        private readonly string _testApiKey = "pk_test_51HTSwqHyZs91COWRN6RyQnyiJjzQ81mR0fBk2XlsCPbNC3ClUNFkhh3KqbiW9ljEDUk0lASIFKCyA6Th0AA9nANn00az56IO1e";
        private readonly string _testApiKeySecret = "sk_test_51HTSwqHyZs91COWRioidL8BwMySeqYSmeoN8fxO7afuCrdgQgTEPXwYX569vEmgkwk1AJwIJfJKPQ8yScSZZlIJN00oTYRtTsZ";
        private bool _isRunning;
        private bool _isEnabled;
        private bool _isCreditCard;
        private decimal _totalValue;
        private int _totalItems;
        private float _totalQuantity;
        private string _deliveryAddress;
        private ObservableCollection<PaymentMethod> _paymentMethods;
        private List<OrderDetailResponse> _orderDetails;
        private TokenResponse _token;
        private PaymentMethod _paymentMethod;
        private Token _stripeToken;
        private TokenService _tokenService;
        private DelegateCommand _finishOrderCommand;

        public FinishOrderPageViewModel(INavigationService navigationService, ICombosHelper combosHelper, IApiService apiService)
            : base(navigationService)
        {
            _navigationService = navigationService;
            _apiService = apiService;
            Title = "Finish Order";
            IsEnabled = true;
            PaymentMethods = new ObservableCollection<PaymentMethod>(combosHelper.GetPaymentMethods());
        }

        public DelegateCommand FinishOrderCommand => _finishOrderCommand ?? (_finishOrderCommand = new DelegateCommand(FinishOrderAsync));

        public string Remarks { get; set; }
        public string CreditCard { get; set; }
        public string Expiry { get; set; }

        public string CVV { get; set; }

        public ObservableCollection<PaymentMethod> PaymentMethods
        {
            get => _paymentMethods;
            set => SetProperty(ref _paymentMethods, value);
        }

        public PaymentMethod PaymentMethod
        {
            get => _paymentMethod;
            set
            {
                SetProperty(ref _paymentMethod, value);
                if (_paymentMethod.Id == 2)
                {
                    IsCreditCard = true;
                }
                else
                {
                    IsCreditCard = false;
                }
            }
        }

        public string DeliveryAddress
        {
            get => _deliveryAddress;
            set => SetProperty(ref _deliveryAddress, value);
        }

        public decimal TotalValue
        {
            get => _totalValue;
            set => SetProperty(ref _totalValue, value);
        }

        public int TotalItems
        {
            get => _totalItems;
            set => SetProperty(ref _totalItems, value);
        }

        public float TotalQuantity
        {
            get => _totalQuantity;
            set => SetProperty(ref _totalQuantity, value);
        }

        public bool IsRunning
        {
            get => _isRunning;
            set => SetProperty(ref _isRunning, value);
        }

        public bool IsCreditCard
        {
            get => _isCreditCard;
            set => SetProperty(ref _isCreditCard, value);
        }

        public bool IsEnabled
        {
            get => _isEnabled;
            set => SetProperty(ref _isEnabled, value);
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            LoadOrderTotals();
        }

        private void LoadOrderTotals()
        {
            _token = JsonConvert.DeserializeObject<TokenResponse>(Settings.Token);
            _orderDetails = JsonConvert.DeserializeObject<List<OrderDetailResponse>>(Settings.OrderDetails);
            if (_orderDetails == null)
            {
                _orderDetails = new List<OrderDetailResponse>();
            }

            TotalItems = _orderDetails.Count;
            TotalValue = _orderDetails.Sum(od => od.Value).Value;
            TotalQuantity = _orderDetails.Sum(od => od.Quantity);
            DeliveryAddress = $"{_token.User.Address}, {_token.User.City.Name}";
        }

        private async void FinishOrderAsync()
        {
            bool isValid = await ValidateDataAsync();
            if (!isValid)
            {
                return;
            }

            IsRunning = true;
            IsEnabled = false;

            if (PaymentMethod.Id == 2)
            {
                bool wasPayed = await PayWithStripeAsync();
                if (!wasPayed)
                {
                    IsRunning = false;
                    IsEnabled = true;
                    return;
                }
            }

            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                IsRunning = false;
                IsEnabled = true;
                await App.Current.MainPage.DisplayAlert(
                    "Error",
                    "Please check your internet connection",
                    "Ok");
                return;
            }

            string url = App.Current.Resources["UrlAPI"].ToString();
            OrderResponse request = new OrderResponse
            {
                OrderDetails = _orderDetails,
                PaymentMethod = ToPaymentMethod(PaymentMethod),
                Remarks = Remarks
            };

            Response response = await _apiService.PostAsync(url, "api", "/Orders", request, _token.Token);
            IsRunning = false;
            IsEnabled = true;

            if (!response.IsSuccess)
            {
                await App.Current.MainPage.DisplayAlert(
                "Error",
                response.Message,
                "Ok");
                return;
            }

            _orderDetails.Clear();
            Settings.OrderDetails = JsonConvert.SerializeObject(_orderDetails);
            await App.Current.MainPage.DisplayAlert(
             "Success",
             "Your order was completed successfully.",
             "Ok");
            await _navigationService.NavigateAsync($"/{nameof(OnSaleMasterDetailPage)}/NavigationPage/{nameof(ProductsPage)}");
        }

        private async Task<bool> PayWithStripeAsync()
        {
            await CreateTokenAsync();
            if (_stripeToken == null)
            {
               
                await App.Current.MainPage.DisplayAlert(
                 "Error",
                 "Invalid Credit Card Number.",
                 "Ok");
                return false;
            }

            return await MakePaymentAsync();
        }

        public async Task<bool> MakePaymentAsync()
        {
            try
            {
                StripeConfiguration.ApiKey = _testApiKeySecret;
                ChargeCreateOptions options = new ChargeCreateOptions
                {
                    Amount = (long)TotalValue * 100,
                    Currency = "USD",
                    Description = $"Order: {DateTime.Now:yyyy/MM/dd hh:mm}",
                    Capture = true,
                    ReceiptEmail = _token.User.Email,
                    Source = _stripeToken.Id
                };

                ChargeService service = new ChargeService();
                Charge charge = await service.CreateAsync(options);
                return true;
            }
            catch (Exception ex)
            {
                ex.ToString();
              
                await App.Current.MainPage.DisplayAlert(
                "Error",
                "The payment process was not successful",
                "Ok");
                return false;
            }
        }

        public async Task<string> CreateTokenAsync()
        {
            try
            {
                StripeConfiguration.ApiKey = _testApiKey;
                ChargeService service = new ChargeService();
                int year = int.Parse(Expiry.Substring(0, 2));
                int month = int.Parse(Expiry.Substring(3, 2));
                TokenCreateOptions tokenOptions = new TokenCreateOptions
                {
                    Card = new TokenCardOptions
                    {
                        Number = CreditCard,
                        ExpYear = year,
                        ExpMonth = month,
                        Cvc = CVV,
                        Name = _token.User.FullName
                    }
                };

                _tokenService = new TokenService();
                _stripeToken = await _tokenService.CreateAsync(tokenOptions);
                return _stripeToken.Id;
            }
            catch
            {
                return null;
            }
        }

        private Common.Enums.PaymentMethod ToPaymentMethod(PaymentMethod paymentMethod)
        {
            switch (paymentMethod.Id)
            {
                case 1: return Common.Enums.PaymentMethod.Cash;
                default: return Common.Enums.PaymentMethod.CreditCard;
            }
        }

        private async Task<bool> ValidateDataAsync()
        {
            if (PaymentMethod == null)
            {
                await App.Current.MainPage.DisplayAlert(
               "Error",
               "Please select a payment method",
               "Ok");
                return false;
            }

            if (string.IsNullOrEmpty(DeliveryAddress))
            {
                await App.Current.MainPage.DisplayAlert(
                 "Error",
                 "Shipping Address is required.",
                 "Ok");
                return false;
            }

            if (PaymentMethod.Id == 2)
            {
                if (string.IsNullOrEmpty(CreditCard) || CreditCard.Contains('_'))
                {
                 await App.Current.MainPage.DisplayAlert(
                "Error",
                "Please enter a valid credit card.",
                "Ok");
                    return false;
                }

                if (string.IsNullOrEmpty(Expiry) || Expiry.Contains('_'))
                {
                   await App.Current.MainPage.DisplayAlert(
                   "Error",
                   "Please enter an expiration date",
                   "Ok");
                  
                    return false;
                }

                if (string.IsNullOrEmpty(CVV) || CVV.Contains('_'))
                {
                    await App.Current.MainPage.DisplayAlert(
                   "Error",
                   "Please enter the credit card CW number",
                   "Ok");
                    return false;
                }
            }

            return true;
        }
    }
}
