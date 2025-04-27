using System;
using System.Net;
using PowerLab;

public class SessionCodeGenerator
{
    private readonly LocalIPService ipService;
    private readonly IEncoder encoder;

    public SessionCodeGenerator(LocalIPService ipProvider, IEncoder encoder)
    {
        this.ipService = ipProvider;
        this.encoder = encoder;
    }

    public string GenerateSessionCode()
    {
        string ipAddress = ipService.GetLocalIPAddress();
        long numericIP = ConvertIPToLong(ipAddress);
        string encoded = encoder.Encode(numericIP);

        return encoded;
    }

    public string DecodeSessionCode(string sessionCode)
    {
        long decodedValue = encoder.Decode(sessionCode); ;

        try
        {
            if (decodedValue < 0 || decodedValue > 0xFFFFFFFF)
            {
                throw new ArgumentException($"El valor {decodedValue} no es una IP vlida.");
            }
        }
        catch
        {
            return "invalid";
        }

        string ip = ConvertLongToIP(decodedValue);

        return ip;
    }

    private long ConvertIPToLong(string ipAddress)
    {
        byte[] bytes = IPAddress.Parse(ipAddress).GetAddressBytes();

        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(bytes);
        }

        long numericIP = BitConverter.ToUInt32(bytes, 0);

        return numericIP;
    }

    private string ConvertLongToIP(long ip)
    {
        if (ip < 0 || ip > 0xFFFFFFFF)
        {
            throw new ArgumentException($"Nmero invlido para IP: {ip}");
        }

        byte[] bytes = BitConverter.GetBytes((uint)ip); // Forzamos a uint

        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(bytes);
        }

        string ipAddress = new IPAddress(bytes).ToString();

        return ipAddress;
    }

}
