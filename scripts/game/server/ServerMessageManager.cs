using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Godot;
using GodotTask;
using Unknown.Game.Protocol;
using Unknown.Utilities;

namespace Unknown.Game;

public partial class ServerMessageManager : Node {
	public static ServerMessageManager Instance;

	public MessageHub MessageHub = new();

	private UdpClient _client;
	private readonly List<IPEndPoint> _connectedClients = new();

	public override void _Ready() {
		Instance = this;

		_client = new UdpClient(new IPEndPoint(IPAddress.Parse(Configuration.Ip), Configuration.Port));

		var thread = new Thread(StartProcess);
		thread.Start();
	}
	
	private void StartProcess() {
		ProcessAsync().Forget();
	}
	
	private async GDTask ProcessAsync() {
		while (true) {
			var result = await _client.ReceiveAsync();
				
			var reader = new Reader(result.Buffer);
				
			var messageId = reader.ReadInt16();
			var messageSize = reader.ReadInt24();

			var message = messageSize > 0 ? MessageFactory.GetMessage((MessageId)messageId, result.RemoteEndPoint, reader) : MessageFactory.GetMessage((MessageId)messageId, result.RemoteEndPoint);

			message.Decode();
			
			Callable.From(() => message.Process().Forget()).CallDeferred();
			Callable.From(() => MessageHub.InvokeCallback((MessageId)messageId, message)).CallDeferred();

			/*
			if (!_connectedClients.Contains(result.RemoteEndPoint)) {
				_connectedClients.Add(result.RemoteEndPoint);
				
				GD.Print("New Device connected");
				Callable.From(() => OnClientConnect?.Invoke(result.RemoteEndPoint)).CallDeferred();
			}

			var stream = new Reader(result.Buffer);
			var tick = stream.ReadInt32();
			var x = stream.ReadSingle();
			var y = stream.ReadSingle();

			GD.Print($"[Server] {tick} " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));

			Callable.From(() => OnInputMessage?.Invoke(result.RemoteEndPoint, tick, x, y)).CallDeferred();
			//OnInputMessage?.Invoke(result.RemoteEndPoint, tick, x, y);
			*/
		}
	}

	public void AddClient(IPEndPoint ipEndPoint) {
		if (!_connectedClients.Contains(ipEndPoint)) {
			_connectedClients.Add(ipEndPoint);
		}
	}
	
	public async GDTask SendMessage(IPEndPoint ipEndPoint, ProtocolMessage message) {
		await message.Encode();
		var buffer = await message.Build();
		await _client.SendAsync(buffer, buffer.Length, ipEndPoint);
	}

	public async GDTask BroadcastMessage(ProtocolMessage message) {
		foreach (var connectedClient in _connectedClients) {
			SendMessage(connectedClient, message).Forget();
		}
	}
}