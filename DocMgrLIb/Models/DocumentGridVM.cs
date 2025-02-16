using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocMgrLib.Models
{
    public record DocumentGridVM
    {
        public required int Id { get; set; }
        public required string UserName { get; set; }
        public required string DocumentName { get; set; }
        public required string OriginalDocumentName { get; set; }
        public required long DocumentDate { get; set; }

    }
}
