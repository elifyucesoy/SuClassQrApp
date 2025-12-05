namespace API.DTOs
{
    
        public class EventDto
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public DateTime Date { get; set; }
            public string QrToken { get; set; }
            public DateTime QrExpire { get; set; }
            public string Status { get; set; }
            public DateTime CreatedAt { get; set; }
        }
    

}