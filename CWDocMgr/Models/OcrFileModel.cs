using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace CWDocMgr.Models
{
    public record OcrFileModel
    {
        public string OCRText { get; set; }
        public string OriginalFileName { get; set; }

        public string CacheFilename { get; set; }

        [Required]
        [Display(Name = "Language")]
        public string Language { get; set; }

        [Required]
        [Display(Name = "Avatar")]
        public string Avatar { get; set; }

        public List<SelectListItem> Languages { get; set; }

    }
}
