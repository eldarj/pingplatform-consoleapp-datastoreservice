using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataSpaceMicroservice.Data.Context;
using DataSpaceMicroservice.Data.Models;
using Ping.Commons.Dtos.Models.DataSpace;

namespace DataSpaceMicroservice.Data.Services.Impl
{
    public class DataSpaceService : IDataSpaceService
    {
        private readonly MyDbContext dbContext;

        public DataSpaceService(MyDbContext dbContext)
        {
            this.dbContext = dbContext;
        } 

        public async Task<FileUploadDto> FileUpload(FileUploadDto fileUploadDto)
        {
            var ownerAccount = dbContext.Accounts
                .Where(a => a.PhoneNumber == fileUploadDto.OwnerPhoneNumber)
                .SingleOrDefault();

            DSFile newFile = new DSFile
            {
                FileName = fileUploadDto.FileName,
                Path = fileUploadDto.FilePath,
                Owner = ownerAccount
            };

            dbContext.Files.Add(newFile);
            await dbContext.SaveChangesAsync();

            return fileUploadDto;
        }
    }
}
