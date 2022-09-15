using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnet_rpg.Services.AuthRepository
{
    public interface IAuthRepository
    {
        public Task<ServiceResponse<string>> Login(string username, string password);

        public Task<ServiceResponse<int>> Register(User user, string password);

        public Task<bool> UserExists(string username);
    }
}