using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuClassQrApp.Entities
{
    public class Event
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime Date { get; set; }

        [Required]
        public string QrToken { get; set; }

        public DateTime QrExpire { get; set; }

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } // Active, Completed, Cancelled

        public string DeviceInfo { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int CreatedBy { get; set; }

        [ForeignKey("CreatedBy")]
        public virtual User Creator { get; set; }
    }
}
