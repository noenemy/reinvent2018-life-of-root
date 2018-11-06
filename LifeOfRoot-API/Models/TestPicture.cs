using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GotTalent_API.Models
{
    [Table("tb_test_picture")]
    public class TestPicture
    {
        [Key]
        public int picture_id { get; set; }
        public string file_loc { get; set; }
        public string use_yn { get; set; }     
    }
}