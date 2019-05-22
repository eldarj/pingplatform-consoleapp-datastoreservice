using Ping.Commons.Dtos.Models.DataSpace;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DataSpaceMicroservice.Data.Services
{
    public interface IDataSpaceService
    {
        Task<DataSpaceMetadata> GetAllByOwner(string ownerPhoneNumber);

        NodeDto FileUpload(string ownerPhoneNumber, FileDto fileUploadDto);

        Task<bool> DeleteFile(string ownerPhoneNumber, string filePath);

        Task<bool> DeleteDirectory(string ownerPhoneNumber, string directoryPath);

        Task<bool> BatchDeleteNodes(string ownerPhoneNumber, List<SimpleNodeDto> nodes);

        Task<NodeDto> NewDirectory(string ownerPhoneNumber, DirectoryDto directoryDto); 

    }
}
