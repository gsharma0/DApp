using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities
{
    [Table("Photos")]
    public class Photo
    {
        public int ID { get; set; }

        public string Url { get; set; }

        public bool isMain { get; set; }

        public string PublicID { get; set; }

        public AppUser AppUser { get; set; }

        public int AppUserId { get; set; }
    }
}