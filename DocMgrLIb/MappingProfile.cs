using AutoMapper;
using DocMgrLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocMgrLib
{
    class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<DocumentModelVM, DocumentModel>();
            
            
            //CreateMap<DocumentModel, DocumentModelVM>()
            //    .ForMember(dest => dest.DateString, opt => opt.MapFrom(src => src.DocumentDate.ToString("MM/dd/yyyy")))
            //    .ForMember(dest => dest.User, opt => opt.Ignore()) // Assuming User needs to be set separately
            //    .ForMember(dest => dest.OCRText, opt => opt.Ignore()); // Assuming OCRText needs to be set separately
        }
    }
}
