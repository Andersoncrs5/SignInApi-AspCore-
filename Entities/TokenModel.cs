using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignInApi.Entities
{
    public class TokenModel
    {
        public string? AcessToken { get; set; }
        public string? RefreshToken { get; set; }
    }
}