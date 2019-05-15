using System.Linq;
using System.Threading.Tasks;
using DataSpaceMicroservice.Data.Context;
using DataSpaceMicroservice.Data.Models;
using Ping.Commons.Dtos.Models.DataSpace;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using AutoMapper;
using System;

namespace DataSpaceMicroservice.Data.Services.Impl
{
    public class DataSpaceService : IDataSpaceService
    {
        private readonly MyDbContext dbContext;
        private readonly IMapper autoMapper;
        public DataSpaceService(MyDbContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.autoMapper = mapper;
        }

        public async Task<bool> DeleteDirectory(string ownerPhoneNumber, string directoryPath)
        {
            if (String.IsNullOrWhiteSpace(directoryPath))
            {
                return false;
            }

            var ownerAccount = dbContext.Accounts
                .Where(a => a.PhoneNumber == ownerPhoneNumber)
                .SingleOrDefault();

            if (ownerAccount == null)
            {
                return false;
            }

            int lastSegmentPos = directoryPath.LastIndexOf('/') + 1;

            string directoryName = directoryPath.Substring(lastSegmentPos); // index + 1
            string dirPath = directoryPath.Remove(lastSegmentPos); // out

            // TODO: check for dirpath segments and find as in a tree-like structure
            DSDirectory dsDirectory = dbContext.DSDirectories
                .Where(directory => directory.Node.Name == directoryName && 
                    directory.Node.Path == dirPath &&
                    directory.Node.OwnerId == ownerAccount.Id)
                .SingleOrDefault();

            if (dsDirectory == null)
            {
                return false;
            }

            dbContext.DSDirectories.Remove(dsDirectory);
            await dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteFile(string ownerPhoneNumber, string fileName)
        {
            var ownerAccount = dbContext.Accounts
                .Where(a => a.PhoneNumber == ownerPhoneNumber)
                .SingleOrDefault();

            if (ownerAccount == null)
            {
                return false;
            }

            DSFile dsFile = dbContext.DSFiles
                .Where(f => f.Node.Name == fileName && f.Node.OwnerId == ownerAccount.Id)
                .SingleOrDefault();

            if (dsFile == null)
            {
                return false;
            }

            dbContext.DSFiles.Remove(dsFile);
            await dbContext.SaveChangesAsync();

            return true;
        }

        // Check how we'll create new dirs based on path not on parentDirName
        public async Task<NodeDto> NewDirectory(string ownerPhoneNumber, DirectoryDto directoryDto)
        { 
            var ownerAccount = dbContext.Accounts
                .Where(a => a.PhoneNumber == ownerPhoneNumber)
                .SingleOrDefault();

            if (ownerAccount == null)
            {
                return null;
            }

            DSDirectory dsDirectory = await dbContext.DSDirectories
                .Where(d => d.Node.Name == directoryDto.DirName && d.Node.OwnerId == ownerAccount.Id)
                .SingleOrDefaultAsync();

            DSNode dsNode;
            if (dsDirectory == null)
            {
                dsNode = new DSNode();
                dsDirectory = new DSDirectory();

                dbContext.DSNodes.Add(dsNode);
                dsDirectory.Node = dsNode;

                dbContext.DSDirectories.Add(dsDirectory);
            }

            if (!String.IsNullOrWhiteSpace(directoryDto.ParentDirName))
            {
                var parentDir = await dbContext.DSDirectories.Where(d => d.Node.Name == directoryDto.ParentDirName).SingleOrDefaultAsync();
                dsDirectory.ParentDirectoryId = parentDir.Id;
            }

            dsDirectory.Node.Name = directoryDto.DirName;
            dsDirectory.Node.Path = directoryDto.Path;
            dsDirectory.Node.Private = directoryDto.Private;
            dsDirectory.Node.NodeType = NodeType.Directory;
            dsDirectory.Node.Owner = ownerAccount;

            await dbContext.SaveChangesAsync();
            return autoMapper.Map<NodeDto>(dsDirectory);
        }

        public async Task<FileUploadDto> FileUpload(FileUploadDto fileUploadDto)
        {
            var ownerAccount = dbContext.Accounts
                .Where(a => a.PhoneNumber == fileUploadDto.OwnerPhoneNumber)
                .SingleOrDefault();

            if (ownerAccount == null)
            {
                return null;
            }

            DSFile dsFile = dbContext.DSFiles
                .Where(f => f.Node.Name == fileUploadDto.FileName && f.Node.OwnerId == ownerAccount.Id)
                .SingleOrDefault();

            DSNode dsNode;
            if (dsFile == null)
            {
                dsNode = new DSNode();
                dsFile = new DSFile();

                dbContext.DSNodes.Add(dsNode);
                dsFile.Node = dsNode;

                dbContext.DSFiles.Add(dsFile);
            }

            //TODO Check FileUploadDto vs FileDto
            dsFile.Node.Name = fileUploadDto.FileName;
            dsFile.Node.Path = fileUploadDto.FilePath;
            dsFile.Node.Owner = ownerAccount;
            dsFile.Node.NodeType = NodeType.File;
            dsFile.MimeType = fileUploadDto.MimeType;
            //TODO Add parent dir (by parent dir name)

            await dbContext.SaveChangesAsync();

            return fileUploadDto;
        }

        // TODO: Implement a mapper helper to map all the files, directories and all sub items (2)
        // TODO: Change this to return back a list of Node items (same type) not files & dirs (1)
        public DataSpaceMetadata GetAllByOwner(string ownerPhoneNumber)
        {
            Account ownerAccount = dbContext.Accounts
                .Include(a => a.OwningNodes)
                //.Include(a => a.OwningFiles)
                //.Include(a => a.OwningDirectories)
                //    .ThenInclude(dir => dir.Files)
                .Where(a => a.PhoneNumber == ownerPhoneNumber)
                .SingleOrDefault();

            if (ownerAccount != null)
            {
                var files = dbContext.DSFiles
                    .Where(f => f.Node.OwnerId == ownerAccount.Id && f.ParentDirectoryId == null)
                    .ToList();

                var dirs = dbContext.DSDirectories
                    .Include(d => d.Files)
                    .Include(d => d.Directories)
                    .Where(d => d.Node.OwnerId == ownerAccount.Id && d.ParentDirectoryId == null)
                    .ToList();

                var dataSpace = new DataSpaceMetadata();
                dataSpace.DiskSize = "5G";
                dataSpace.Nodes = new List<NodeDto>();

                dataSpace.Nodes.AddRange(autoMapper.Map<List<NodeDto>>(dirs));
                dataSpace.Nodes.AddRange(autoMapper.Map<List<NodeDto>>(files));

                dataSpace.Nodes = dataSpace.Nodes.OrderByDescending(node => node.LastModifiedTime).ToList();

                return dataSpace;
                // MANUAL MAPPING IS BAD!
                //dataSpace.AllNodes.AddRange(dirs.Select(d => new NodeDto
                //{
                //    Name = d.Node.Name,
                //    Path = d.Node.Path,
                //    CreationTime = d.Node.CreationTime,
                //    LastModifiedTime = d.Node.LastModifiedTime,
                //    OwnerFirstname = d.Node.Owner.Firstname,
                //    OwnerLastname = d.Node.Owner.Lastname,
                //    Private = d.Node.Private,
                //    NodeType = d.Node.NodeType.ToString(),
                //    ParentDirName = d.ParentDirectory?.Node.Name,
                //    Files = d.Files.Select(f => new FileDto
                //    {
                //        FileName = f.Node.Name,
                //        Path = f.Node.Path,
                //        CreationTime = f.Node.CreationTime,
                //        LastModifiedTime = f.Node.LastModifiedTime,
                //        MimeType = f.MimeType,
                //        OwnerFirstname = "", //TODO: Check for owner
                //        OwnerLastname = "",
                //        Private = f.Node.Private
                //    }).ToList(),
                //    Directories = d.Directories.Select(dd => new DirectoryDto
                //    {
                //        DirName = dd.Node.Name,
                //        Path = dd.Node.Path,
                //        Private = dd.Node.Private,
                //        CreationTime = dd.Node.CreationTime,
                //        LastModifiedTime = dd.Node.LastModifiedTime,
                //        Directories = dd.Directories.Select(dir => new DirectoryDto
                //        {
                //            CreationTime = dir.Node.CreationTime,
                //            DirName = dir.Node.Name,
                //            LastModifiedTime = dir.Node.LastModifiedTime,
                //            Directories = dir.Directories.Select(dirchild => new DirectoryDto
                //            {
                //                CreationTime = dirchild.Node.CreationTime,
                //                DirName = dirchild.Node.Name,
                //                LastModifiedTime = dirchild.Node.LastModifiedTime,
                //                Path = dirchild.Node.Path,
                //                Private = dirchild.Node.Private
                //            }).ToList(),
                //            Path = dir.Node.Path,
                //            Private = dir.Node.Private
                //        }).ToList(),
                //        Files = dd.Files.Select(f => new FileDto
                //        {
                //            FileName = f.Node.Name,
                //            Path = f.Node.Path,
                //            CreationTime = f.Node.CreationTime,
                //            LastModifiedTime = f.Node.LastModifiedTime,
                //            MimeType = f.MimeType,
                //            OwnerFirstname = "", //TODO: Check for owner
                //            OwnerLastname = "",
                //            Private = f.Node.Private
                //        }).ToList()
                //    }).ToList()
                //}).ToList());


                // SPECIFIC LIST OF DIRECTORIES (REMOVE)
                //dataSpace.Directories = dirs
                //    .Where(d => d.ParentDirectoryId == null)
                //    .Select(d => new DirectoryDto
                //    {
                //        DirName = d.Node.Name,
                //        Path = d.Node.Path,
                //        Private = d.Node.Private,
                //        CreationTime = d.Node.CreationTime,
                //        LastModifiedTime = d.Node.LastModifiedTime,
                //        Directories = d.Directories.Select(dir => new DirectoryDto
                //        {
                //            CreationTime = dir.Node.CreationTime,
                //            DirName = dir.Node.Name,
                //            LastModifiedTime = dir.Node.LastModifiedTime,
                //            Directories = dir.Directories.Select(dirchild => new DirectoryDto
                //            {
                //                CreationTime = dirchild.Node.CreationTime,
                //                DirName = dirchild.Node.Name,
                //                LastModifiedTime = dirchild.Node.LastModifiedTime,
                //                Path = dirchild.Node.Path,
                //                Private = dirchild.Node.Private
                //            }).ToList(),
                //            Path = dir.Node.Path,
                //            Private = dir.Node.Private
                //        }).ToList(),
                //        Files = d.Files.Select(f => new FileDto
                //        {
                //            FileName = f.Node.Name,
                //            Path = f.Node.Path,
                //            CreationTime = f.Node.CreationTime,
                //            LastModifiedTime = f.Node.LastModifiedTime,
                //            MimeType = f.MimeType,
                //            OwnerFirstname = "", //TODO: Check for owner
                //            OwnerLastname = "",
                //            Private = f.Node.Private
                //        }).ToList()
                //    })
                //    .ToList();

                //// SPECIFIC LIST OF FILES (REMOVE)
                //dataSpace.Files = files.Where(f => f.ParentDirectoryId == null)
                //    .Select(f => new FileDto
                //    {
                //        FileName = f.Node.Name,
                //        Path = f.Node.Path,
                //        CreationTime = f.Node.CreationTime,
                //        LastModifiedTime = f.Node.LastModifiedTime,
                //        MimeType = f.MimeType,
                //        OwnerFirstname = "", //TODO: Check for owner
                //        OwnerLastname = "",
                //        Private = f.Node.Private
                //    })
                //    .ToList();

                return dataSpace;
            }

            return null;
        }
    }
}
