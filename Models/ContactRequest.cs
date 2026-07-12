namespace xema.Models
{
    public class ContactRequest
    {
        public int Id { get; set; }

        public required string Name { get; set; }

        public required string Email { get; set; }

        public string? Phone { get; set; }

        public required string Message { get; set; }

        public DateTime SubmittedAt { get; set; }
    }
}
