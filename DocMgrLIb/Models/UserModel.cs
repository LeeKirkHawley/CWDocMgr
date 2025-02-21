using System.ComponentModel.DataAnnotations;

namespace DocMgrLib.Models
{
    public class UserModel
    {
        [Key]
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Pwd { get; set; }
        public string Role { get; set; }
    }
}
