using Ping.Commons.Dtos.Models.DataSpace;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DataSpaceMicroservice.Data.Services
{
    public interface IDataSpaceService
    {
        DataSpaceMetadata GetAllByOwner(string ownerPhoneNumber);

        Task<FileUploadDto> FileUpload(FileUploadDto fileUploadDto);

        Task<bool> DeleteFile(string ownerPhoneNumber, string fileName);

        Task<DirectoryDto> NewDirectory(string ownerPhoneNumber, DirectoryDto directoryDto); 
    }
}
