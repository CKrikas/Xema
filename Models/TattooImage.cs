namespace xema.Models
{
    public class TattooImage
    {
        public int Id { get; set; }

        public required string Title { get; set; }

        public string? Description { get; set; }

        public required string ImageUrl { get; set; }

        public DateTime UploadedAt { get; set; }

        public int CategoryId { get; set; }

        public Category? Category { get; set; } //navigation property, 
    }
}
