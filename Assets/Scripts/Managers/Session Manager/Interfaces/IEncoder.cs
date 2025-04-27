namespace PowerLab
{
    public interface IEncoder
    {
        string Encode(long value);
        long Decode(string input);
    }
}