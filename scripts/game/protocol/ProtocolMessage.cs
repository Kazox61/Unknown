using System.IO;
using System.Net;
using GodotTask;
using Unknown.Utilities;

namespace Unknown.Game.Protocol;

public abstract class ProtocolMessage {
	public abstract MessageId MessageId { get; }

	public IPEndPoint IpEndPoint;
	public readonly Reader Reader;

	protected readonly MemoryStream Stream = new();

	public ProtocolMessage() { }

	public ProtocolMessage(IPEndPoint ipEndPoint = null, Reader reader = null) {
		IpEndPoint = ipEndPoint;
		Reader = reader;
	}
	public virtual async GDTask Encode() { }

	public async GDTask<byte[]> Build() {
		using var stream = new MemoryStream();
		var length = (ushort)Stream.Length;

		await stream.WriteUShort((ushort)MessageId);

		stream.WriteByte(0);

		await stream.WriteUShort(length);

		await stream.WriteBuffer(Stream.ToArray());

		return stream.ToArray();
	}

	public virtual void Decode() { }
	public virtual async GDTask Process() { }
}