using System.Buffers;
using MySqlCdc.Protocol;

namespace MySqlCdc.Packets;

/// <summary>
/// ERR_Packet indicates that an error occured.
/// <a href="https://mariadb.com/kb/en/library/err_packet/">See more</a>
/// </summary>
internal class ErrorPacket : IPacket
{
    public int ErrorCode { get; }
    public string ErrorMessage { get; }
    public string? SqlState { get; }

    public ErrorPacket(ReadOnlySequence<byte> buffer)
    {
        using var memoryOwner = new MemoryOwner(buffer);
        var reader = new PacketReader(memoryOwner.Memory.Span);

        ErrorCode = reader.ReadUInt16LittleEndian();

        var message = reader.ReadStringToEndOfFile();
        if (message.StartsWith("#"))
        {
            ErrorMessage = message.Substring(6);
            SqlState = message.Substring(1, 5);
        }
        else
        {
            ErrorMessage = message;
        }
    }

    public override string ToString()
    {
        return $"ErrorCode: {ErrorCode}, ErrorMessage: {ErrorMessage}, SqlState:{SqlState}";
    }
}