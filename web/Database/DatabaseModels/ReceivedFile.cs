using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace web.Database.DatabaseModels
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
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreateDate { get; set; }

        public string Hash { get; set; }
        public decimal Progress { get; set; }
        public string Result { get; set; }
    }
}
