using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace LiqPaySample.Providers.LiqPay;

public static class LiqPaySignatureHelper
{
    /// <summary>
    /// Генерує підпис для запиту до LiqPay.
    /// Алгоритм: Base64(SHA1(private_key + data + private_key))
    /// </summary>
    public static string GenerateSignature(string data, string privateKey)
    {
        // Крок 1: конкатенація private_key + data + private_key
        var rawString = privateKey + data + privateKey;

        // Крок 2: SHA1-хеш від сконкатенованого рядка
        var bytes = Encoding.UTF8.GetBytes(rawString);
        var hash = SHA1.HashData(bytes);

        // Крок 3: Base64 кодування хешу
        return Convert.ToBase64String(hash);
    }

    /// <summary>
    /// Кодує параметри запиту в Base64(JSON).
    /// </summary>
    public static string EncodeData(object parameters)
    {
        var json = JsonSerializer.Serialize(parameters,
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower });
        var bytes = Encoding.UTF8.GetBytes(json);
        return Convert.ToBase64String(bytes);
    }

    /// <summary>
    /// Декодує Base64 data з webhook у словник.
    /// </summary>
    public static Dictionary<string, object>? DecodeWebhookData(string base64Data)
    {
        var bytes = Convert.FromBase64String(base64Data);
        var json = Encoding.UTF8.GetString(bytes);
        return JsonSerializer.Deserialize<Dictionary<string, object>>(json);
    }

    /// <summary>
    /// Перевіряє підпис вхідного webhook.
    /// Повертає true, якщо підпис валідний.
    /// </summary>
    public static bool VerifySignature(string data, string signature, string privateKey)
    {
        var expectedSignature = GenerateSignature(data, privateKey);
        // Порівняння через CryptographicOperations.FixedTimeEquals 
        // запобігає timing attacks
        return CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(expectedSignature),
            Encoding.UTF8.GetBytes(signature));
    }
}