using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SignInApi.utils.hateaos;

namespace SignInApi.DTOs
{
    public class UserDTO : Resource
    {
        public string? Id { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
    }
}