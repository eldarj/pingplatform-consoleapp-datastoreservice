using Api.DtoModels.Auth;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DataSpaceMicroservice.Data.Services
{
    public interface IAccountService
    {
        Task<bool> CreateNewUser(AccountDto accountDto);
    }
}
