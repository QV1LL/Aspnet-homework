namespace TestsSample.Tests.Models;

public class UserTestData : TheoryData<string, string, int, string>
{
    public UserTestData()
    {
        Add("", "omar@gmail.com", 20, "hash123");
        Add("Omar", "not-an-email", 20, "hash123");
        Add("Omar", "omar@gmail.com", -1, "hash123");
        Add("Omar", "omar@gmail.com", 20, "");
    }
}