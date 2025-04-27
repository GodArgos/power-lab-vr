using System.Linq;
using PowerLab;

public class Base36Encoder : IEncoder
{
    private const string chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    public string Encode(long value)
    {
        string result = "";
        while (value > 0)
        {
            result = chars[(int)(value % 36)] + result;
            value /= 36;
        }
        return result.PadLeft(5, '0');
    }

    public long Decode(string input)
    {
        return input.Aggregate(0L, (current, c) => current * 36 + chars.IndexOf(c));
    }
}