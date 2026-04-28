using System.Text.Json;
using LiqPaySample.Options;
using Microsoft.Extensions.Options;

namespace LiqPaySample.Providers.LiqPay;

public class LiqPayClient(HttpClient http, IOptions<LiqPayOptions> options)
{
    private readonly LiqPayOptions _options = options.Value;

    public string PublicKey => _options.PublicKey;
    public bool IsSandbox => _options.IsSandbox;
    public string CheckoutBaseUrl => _options.CheckoutBaseUrl;
    
    /// <summary>
    /// Відправляє API-запит до LiqPay та повертає словник відповіді.
    /// </summary>
    public async Task<Dictionary<string, string>?> SendApiRequestAsync(
        object payload,
        CancellationToken ct = default)
    {
        var data = LiqPaySignatureHelper.EncodeData(payload);
        var signature = LiqPaySignatureHelper.GenerateSignature(data, _options.PrivateKey);

        var formContent = new FormUrlEncodedContent([
            new KeyValuePair<string, string>("data", data),
            new KeyValuePair<string, string>("signature", signature)
        ]);

        var response = await http.PostAsync(_options.ApiUrl, formContent, ct);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync(ct);
        return JsonSerializer.Deserialize<Dictionary<string, string>>(json);
    }

    /// <summary>
    /// Генерує параметри для Checkout redirect (data + signature).
    /// Ці параметри передаються у форму або JavaScript Widget.
    /// </summary>
    public (string Data, string Signature) GetCheckoutParams(object payload)
    {
        var data = LiqPaySignatureHelper.EncodeData(payload);
        var signature = LiqPaySignatureHelper.GenerateSignature(data, _options.PrivateKey);
        return (data, signature);
    }
}