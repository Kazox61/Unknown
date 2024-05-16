using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using GodotTask;
using Unknown.Utilities;

namespace Unknown.Game;

public partial class ServerMessageManager : Node {
	public static ServerMessageManager Instance;

	private UdpClient _client;

	private readonly List<IPEndPoint> _connectedClients = new();

	
	public delegate void ClientConnectEventHandler(IPEndPoint ipEndPoint);
	public ClientConnectEventHandler OnClientConnect;
	public Action<IPEndPoint, int, float, float> OnInputMessage;

	public override void _Ready() {
		Instance = this;

		_client = new UdpClient(new IPEndPoint(IPAddress.Parse(Configuration.Ip), Configuration.Port));

		var thread = new Thread(StartProcess);
		thread.Start();
	}
	
	private void StartProcess() {
		ProcessAsync();
	}
	
	private async GDTask ProcessAsync() {
		while (true) {
			var result = await _client.ReceiveAsync();

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
		}
	}

	public async GDTask Broadcast(byte[] bytes) {
		foreach (var connectedClient in _connectedClients) {
			await _client.SendAsync(bytes, bytes.Length, connectedClient);
		}
	}
}