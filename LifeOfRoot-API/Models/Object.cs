using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GotTalent_API.Models
{
    [Table("tb_object")]
    public class Object
    {
        [Key]
        public int object_id { get; set; }
        public string object_name { get; set; }
        public int object_score { get; set; }
        public int difficulty { get; set; }
        public DateTime log_date { get; set; } 
    }
}