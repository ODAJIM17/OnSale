using System;
using System.Collections.Generic;
using System.Text;

namespace OnSale.Common.Requests
{
    public class TokenResponse
    {
        public string Token { get; set; }
        public UserResponse User { get; set; }
        public DateTime Expiration { get; set; }
        public DateTime ExpirationLocal => Expiration.ToLocalTime();
    }

}
