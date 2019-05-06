using Ping.Commons.Dtos.Models.DataSpace;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DataSpaceMicroservice.Data.Services
{
    public interface IDataSpaceService
    {
        Task<FileUploadDto> FileUpload(FileUploadDto fileUploadDto);
    }
}
