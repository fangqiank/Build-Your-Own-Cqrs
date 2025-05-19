namespace BuildYourOwnCqrs.Models
{
    public class Todo
    {
        public int Id { get; set; }
        public required string Title { get; set; } // Using 'required' for C# 12
        public bool IsCompleted { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
