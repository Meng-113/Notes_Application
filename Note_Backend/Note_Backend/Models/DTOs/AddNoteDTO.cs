namespace Note_Backend.Models.DTOs
{
    public class AddNoteDTO
    {
        public required string Title { get; set; }
        public string Content { get; set; }
    }
}
