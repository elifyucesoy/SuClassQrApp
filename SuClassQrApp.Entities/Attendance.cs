using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuClassQrApp.Entities
{
    public class Attendance
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int EventId { get; set; }

        [Required]
        public int UserId { get; set; }

        public DateTime ScanTime { get; set; } = DateTime.UtcNow;

        [ForeignKey("EventId")]
        public virtual Event Event { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}
