public class Like
{
    public int Id { get; set; }
    public string LikerId { get; set; }
    public string LikedId { get; set; }
    public bool IsLiked { get; set; }  // true for like, false for reject

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
