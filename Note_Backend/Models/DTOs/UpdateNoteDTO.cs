namespace Note_Backend.Models.DTOs
{
    public class UpdateNoteDTO
    {
        public required string Title { get; set; }
        public string? Content { get; set; }
    }
}
