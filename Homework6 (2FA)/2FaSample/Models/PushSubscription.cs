namespace _2FaSample.Models;

public class PushSubscription
{
    public Guid Id { get; set; }
    public AppUser? User { get; set; }
    public string UserId { get; set; }
    public string Endpoint { get; set; }
    public string P256dh { get; set; }
    public string Auth { get; set; }
}