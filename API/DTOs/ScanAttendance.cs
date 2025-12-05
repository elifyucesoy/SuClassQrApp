
using System.ComponentModel.DataAnnotations;


namespace QRAttendanceSystem.DTOs
{
   
    public class ScanAttendanceDto
    {
        [Required]
        public string QrToken { get; set; }
    }
}
