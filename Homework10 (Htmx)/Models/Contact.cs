namespace Homework10__Htmx_.Models;

public class Contact
{
    public int Id  { get; set; }
    public required string Name { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
}