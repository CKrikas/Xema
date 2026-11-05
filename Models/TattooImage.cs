namespace xema.Models
{
    public class TattooImage
    {
        public int Id { get; set; }

        public required string Title { get; set; }

        public string? Description { get; set; }

        public int MediaId { get; set; }

        public Media Media { get; set; } = null!;

        public DateTime UploadedAt { get; set; }

        public int CategoryId { get; set; }

        public Category? Category { get; set; } //navigation property 

        public bool IsFeatured { get; set; }
    }
}
