namespace ClipVault.Utils;
public static class ValidationUtils
{
    public static bool IsBase64String(string base64)
    {
        if (string.IsNullOrEmpty(base64)) return false;

        Span<byte> buffer = new(new byte[base64.Length]);
        return Convert.TryFromBase64String(base64, buffer, out _);
    }
}
