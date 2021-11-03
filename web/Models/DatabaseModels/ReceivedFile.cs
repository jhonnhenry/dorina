using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace web.Models.DatabaseModels
{
    public class ReceivedFile
    {
        public ReceivedFile()
        {
            Id = Guid.NewGuid();
            CreateDate = DateTime.Now;
        }

        [Key]
        [Required]
        public Guid Id { get; set; }
        [Required]
        public DateTime CreateDate { get; set; }

        public string Filename { get; set; }
        public string Result { get; set; }
        public string Username { get; set; }
    }
}
