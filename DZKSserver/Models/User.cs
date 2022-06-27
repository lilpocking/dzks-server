using System.ComponentModel.DataAnnotations.Schema;

namespace DZKSserver.Models
{
    public class User
    {
        [Key]
        public int userId { get; set; }
        public string login { get; set; } = String.Empty;
        public string password { get; set; } = String.Empty;
        [ForeignKey("letterKey")]
        public List<Letter> letters { get; set; } = new List<Letter>();
    }
}
