using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Buffers.Binary;
using System.Text.Json;
using System.Text.Json.Serialization;

public enum MessageType
{
    FoundMatch,
    PlayerInfoUpdate,
    StoreUpdate
}

public sealed record FramedMessage(MessageType Type, string Message);

static class Framing
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };

    // Serialize to UTF-8 bytes
    public static byte[] ToBytes(FramedMessage msg)
    {
        return JsonSerializer.SerializeToUtf8Bytes(msg, JsonOptions);
    }

    // Deserialize from UTF-8 bytes
    public static FramedMessage FromBytes(ReadOnlySpan<byte> buffer)
    {
        return JsonSerializer.Deserialize<FramedMessage>(buffer, JsonOptions)
               ?? throw new InvalidDataException("Unable to deserialize message");
    }

    // Read length-prefixed message. Returns null if stream closed.
    public static async Task<FramedMessage?> ReadMessageAsync(NetworkStream stream, CancellationToken token = default)
    {
        // Read 4-byte length prefix
        var header = new byte[4];
        var read = await ReadExactlyAsync(stream, header, token).ConfigureAwait(false);
        if (read == 0) return null; // closed
        if (read < 4) throw new EndOfStreamException("Incomplete message header");

        var len = BinaryPrimitives.ReadInt32BigEndian(header);
        if (len <= 0) throw new InvalidDataException("Invalid message length");

        var payload = new byte[len];
        read = await ReadExactlyAsync(stream, payload, token).ConfigureAwait(false);
        if (read < len) throw new EndOfStreamException("Incomplete message payload");

        return FromBytes(payload);
    }

    // Helper to read exactly buffer.Length bytes or return bytes read if stream closed
    private static async Task<int> ReadExactlyAsync(Stream stream, byte[] buffer, CancellationToken token)
    {
        var offset = 0;
        while (offset < buffer.Length)
        {
            var n = await stream.ReadAsync(buffer, offset, buffer.Length - offset, token).ConfigureAwait(false);
            if (n == 0) return offset; // closed
            offset += n;
        }
        return offset;
    }
}

public static class SimpleTcpServer
{
    public static async Task RunAsync(int port, CancellationToken cancellationToken)
    {
        var listener = new TcpListener(IPAddress.Any, port);
        listener.Start();
        Console.WriteLine($"TCP server listening on port {port}");

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var client = await listener.AcceptTcpClientAsync(cancellationToken).ConfigureAwait(false);
                _ = HandleClientAsync(client, cancellationToken); // fire-and-forget per-client task
            }
        }
        catch (OperationCanceledException) { /* expected on shutdown */ }
        finally
        {
            try { listener.Stop(); } catch { }
            Console.WriteLine("TCP listener stopped");
        }
    }

    private static async Task HandleClientAsync(TcpClient client, CancellationToken token)
    {
        var remote = client.Client.RemoteEndPoint;
        Console.WriteLine($"Client connected: {remote}");

        try
        {
            using var stream = client.GetStream();

            while (!token.IsCancellationRequested && client.Connected)
            {
                //add foctualaty here
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Connection error ({remote}): {ex.Message}");
        }
        finally
        {
            try { client.Close(); } catch { }
            Console.WriteLine($"Client disconnected: {remote}");
        }
    }
}
