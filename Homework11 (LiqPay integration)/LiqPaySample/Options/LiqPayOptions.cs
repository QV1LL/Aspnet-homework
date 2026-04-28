namespace LiqPaySample.Options;

public class LiqPayOptions
{
    public string PrivateKey { get; set; } = string.Empty;
    public string PublicKey { get; set; } = string.Empty;
    public string ApiUrl { get; set; } = string.Empty;
    public string CheckoutBaseUrl { get; set; } = string.Empty;
    public bool IsSandbox { get; set; } = true;
}
