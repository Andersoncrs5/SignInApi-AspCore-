using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SignInApi.utils.hateaos;

namespace SignInApi.Services.IServices
{
    public interface ILinkService<T> where T : Resource
    {
        T GenerateLinks(T resource);
    }   
}