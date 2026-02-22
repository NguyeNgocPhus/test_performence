using System.Security.Cryptography;
using System.Text;

public interface IRywTokenService
{
    string Create(string userId, DateTimeOffset expiresAt);
    bool TryValidate(string token, string expectedUserId, DateTimeOffset now, out DateTimeOffset expiresAt);
}

public sealed class RywTokenService : IRywTokenService
{
    private readonly byte[] _key;
    private readonly int _maxTtlSeconds;

    public RywTokenService(string base64Key, int maxTtlSeconds = 10)
    {
        _key = Convert.FromBase64String(base64Key);
        if (_key.Length < 32) throw new ArgumentException("RYW HMAC key must be at least 32 bytes.");
        _maxTtlSeconds = maxTtlSeconds;
    }

    public string Create(string userId, DateTimeOffset expiresAt)
    {
        var exp = expiresAt.ToUnixTimeSeconds();
        var payload = $"{userId}|{exp}";
        var payloadB64 = B64Url(Encoding.UTF8.GetBytes(payload));

        var sig = Sign(payloadB64);
        return $"{payloadB64}.{sig}";
    }

    public bool TryValidate(string token, string expectedUserId, DateTimeOffset now, out DateTimeOffset expiresAt)
    {
        expiresAt = default;

        var parts = token.Split('.', 2);
        if (parts.Length != 2) return false;

        var payloadB64 = parts[0];
        var sig = parts[1];

        var expectedSig = Sign(payloadB64);
        if (!FixedTimeEquals(sig, expectedSig)) return false;

        var payload = Encoding.UTF8.GetString(B64UrlDecode(payloadB64));
        var fields = payload.Split('|', 2);
        if (fields.Length != 2) return false;

        var uid = fields[0];
        if (!string.Equals(uid, expectedUserId, StringComparison.Ordinal)) return false;

        if (!long.TryParse(fields[1], out var expUnix)) return false;
        expiresAt = DateTimeOffset.FromUnixTimeSeconds(expUnix);

        // skew tolerance
        if (expiresAt <= now.AddSeconds(-2)) return false;

        // hard-cap TTL (anti-tamper even if key leaked somewhere upstream)
        if (expiresAt > now.AddSeconds(_maxTtlSeconds)) return false;

        return true;
    }

    private string Sign(string payloadB64)
    {
        using var h = new HMACSHA256(_key);
        var sig = h.ComputeHash(Encoding.UTF8.GetBytes(payloadB64));
        return B64Url(sig);
    }

    private static bool FixedTimeEquals(string a, string b)
        => CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(a), Encoding.UTF8.GetBytes(b));

    private static string B64Url(byte[] bytes)
        => Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');

    private static byte[] B64UrlDecode(string s)
    {
        s = s.Replace('-', '+').Replace('_', '/');
        if (s.Length % 4 == 2) s += "==";
        else if (s.Length % 4 == 3) s += "=";
        return Convert.FromBase64String(s);
    }
}