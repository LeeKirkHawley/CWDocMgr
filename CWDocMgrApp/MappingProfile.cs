using AutoMapper;
using DocMgrLib.Models;

namespace CWDocMgrApp
{
    internal class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //CreateMap<DocumentGridVM, DocumentModel>()
            //    .ForMember(dest => dest.DocumentName, opt => opt.MapFrom(src => src.DocumentName))
            //    .ForMember(dest => dest.OriginalDocumentName, opt => opt.MapFrom(src => src.OriginalDocumentName))
            //    .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.UserName))
            //    .ForMember(dest => dest.DocumentDate, opt => opt.MapFrom(src => src.DocumentDate));
        }
    }
}
