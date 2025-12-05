using System.ComponentModel.DataAnnotations;



namespace QRAttendanceSystem.DTOs
{
   
    public class CreateEventDto
    {
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        [Required]
        public DateTime Date { get; set; }
    }
}
