using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Api.DtoModels.Auth;
using DataSpaceMicroservice.Data.Context;
using DataSpaceMicroservice.Data.Models;

namespace DataSpaceMicroservice.Data.Services.Impl
{
    public class AccountService : IAccountService
    {
        private MyDbContext dbContext;

        public AccountService(MyDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<bool> CreateNewUser(AccountDto accountDto)
        {
            var account = dbContext.Accounts.Where(a => a.PhoneNumber == accountDto.PhoneNumber).SingleOrDefault();
            if (account != null) return false; // implement update

            account = new Account
            {
                Id = accountDto.Id,
                PhoneNumber = accountDto.PhoneNumber,
                Firstname = accountDto.Firstname,
                Lastname = accountDto.Lastname,
                AvatarImageUrl = accountDto.AvatarImageUrl
            };

            dbContext.Accounts.Add(account);

            await dbContext.SaveChangesAsync();

            return true;
        }
    }
}
