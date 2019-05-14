using AutoMapper;
using DataSpaceMicroservice.Data.Context;
using DataSpaceMicroservice.Data.Models;
using Ping.Commons.Dtos.Models.DataSpace;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataSpaceMicroservice.Data.Mappers
{
    public class DataspaceDtoMapperProfile : Profile
    {
        public DataspaceDtoMapperProfile(MyDbContext db)
        {
            // Map DSDirectory to NodeDto
            CreateMap<DSDirectory, NodeDto>()
                .ForMember(dest => dest.Name, source => source.MapFrom(dsDir => dsDir.Node.Name))
                .ForMember(dest => dest.Path, source => source.MapFrom(dsDir => dsDir.Node.Path))
                .ForMember(dest => dest.CreationTime, source => source.MapFrom(dsDir => dsDir.Node.CreationTime))
                .ForMember(dest => dest.LastModifiedTime, source => source.MapFrom(dsDir => dsDir.Node.LastModifiedTime))
                .ForMember(dest => dest.OwnerFirstname, source => source.MapFrom(dsDir => dsDir.Node.Owner.Firstname))
                .ForMember(dest => dest.OwnerLastname, source => source.MapFrom(dsDir => dsDir.Node.Owner.Lastname))
                .ForMember(dest => dest.Private, source => source.MapFrom(dsDir => dsDir.Node.Private))
                .ForMember(dest => dest.NodeType, source => source.MapFrom(dsDir => dsDir.Node.NodeType.ToString()))
                .ForMember(dest => dest.ParentDirName, source => source.MapFrom(dsDir => dsDir.ParentDirectory.Node.Name))
                .ForMember(dest => dest.Directories, source => source.MapFrom(dsDir => dsDir.Directories))
                .ForMember(dest => dest.MimeType, options => options.Ignore())
                .ForMember(dest => dest.Files, source => source.MapFrom(dsDir => dsDir.Files)); // TODO: Set up DSFile to NodeDto mapping

            // Map DSFile to NodeDto
            CreateMap<DSFile, NodeDto>()
                .ForMember(dest => dest.Name, source => source.MapFrom(dsDir => dsDir.Node.Name))
                .ForMember(dest => dest.Path, source => source.MapFrom(dsDir => dsDir.Node.Path))
                .ForMember(dest => dest.CreationTime, source => source.MapFrom(dsDir => dsDir.Node.CreationTime))
                .ForMember(dest => dest.LastModifiedTime, source => source.MapFrom(dsDir => dsDir.Node.LastModifiedTime))
                .ForMember(dest => dest.OwnerFirstname, source => source.MapFrom(dsDir => dsDir.Node.Owner.Firstname))
                .ForMember(dest => dest.OwnerLastname, source => source.MapFrom(dsDir => dsDir.Node.Owner.Lastname))
                .ForMember(dest => dest.Private, source => source.MapFrom(dsDir => dsDir.Node.Private))
                .ForMember(dest => dest.NodeType, source => source.MapFrom(dsDir => dsDir.Node.NodeType.ToString()))
                .ForMember(dest => dest.ParentDirName, source => source.MapFrom(dsDir => dsDir.ParentDirectory.Node.Name))
                .ForMember(dest => dest.MimeType, source => source.MapFrom(dsFile => dsFile.MimeType))
                .ForMember(dest => dest.Directories, options => options.Ignore())
                .ForMember(dest => dest.Files, options => options.Ignore());
        }
    }
}
