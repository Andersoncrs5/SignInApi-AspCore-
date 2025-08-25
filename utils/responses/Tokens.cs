using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignInApi.utils.responses
{
    public class Tokens
    {
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? ExpirationToken  { get; set; }
        public DateTime? ExpirationRefreshToken  { get; set; }
    }
}