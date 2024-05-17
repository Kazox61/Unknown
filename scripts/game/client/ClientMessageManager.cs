using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using GodotTask;
using Unknown.Game.Protocol;
using Unknown.Utilities;

namespace Unknown.Game;

public partial class ClientMessageManager : Node {
	public Action<int, float, float> OnMessage;

	private UdpClient _client;

	public override void _Ready() {
		_client = new UdpClient();
		_client.Client.Bind(new IPEndPoint(IPAddress.Any, GetAvailablePort(2000)));
		var thread = new Thread(StartProcess);
		thread.Start();
	}

	private void StartProcess() {
		ProcessAsync().Forget();

		StartAsync().Forget();
	}

	private async GDTask StartAsync() {
		await Task.Delay(TimeSpan.FromSeconds(1));
		SendMessage(new MatchJoinMessage()).Forget();
	}


	private async GDTask ProcessAsync() {
		while (true) {
			try {
				var result = await _client.ReceiveAsync();

				var reader = new Reader(result.Buffer);

				var messageId = reader.ReadInt16();
				var messageSize = reader.ReadInt24();

				var message = messageSize > 0
					? MessageFactory.GetMessage((MessageId)messageId, reader: reader)
					: MessageFactory.GetMessage((MessageId)messageId);

				message.Decode();
				Callable.From(() => message.Process().Forget()).CallDeferred();
				/*
				var tick = reader.ReadInt32();
				var x = reader.ReadSingle();
				var y = reader.ReadSingle();
				
				GD.Print($"[Client] {tick} received " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
				Callable.From(() => OnMessage?.Invoke(tick, x, y)).CallDeferred();
				//OnMessage?.Invoke(tick, x, y);
				*/
			}
			catch (Exception e) {
				Console.WriteLine(e);
			}
		}
	}

	private static int GetAvailablePort(int startingPort) {
		var portArray = new List<int>();

		var properties = IPGlobalProperties.GetIPGlobalProperties();

		//getting active connections
		var connections = properties.GetActiveTcpConnections();
		portArray.AddRange(from n in connections
			where n.LocalEndPoint.Port >= startingPort
			select n.LocalEndPoint.Port);

		//getting active tcp listeners - WCF service listening in tcp
		var endPoints = properties.GetActiveTcpListeners();
		portArray.AddRange(from n in endPoints
			where n.Port >= startingPort
			select n.Port);

		//getting active udp listeners
		endPoints = properties.GetActiveUdpListeners();
		portArray.AddRange(from n in endPoints
			where n.Port >= startingPort
			select n.Port);

		portArray.Sort();

		for (var i = startingPort; i < ushort.MaxValue; i++) {
			if (!portArray.Contains(i)) {
				return i;
			}
		}

		return 0;
	}

	public async GDTask SendMessage(ProtocolMessage message) {
		await message.Encode();
		var buffer = await message.Build();
		await _client.SendAsync(buffer, buffer.Length, Configuration.Ip, Configuration.Port);
	}

	public async GDTask SendAsync(string text) {
		var data = Encoding.UTF8.GetBytes(text);
		await _client.SendAsync(data, data.Length, Configuration.Ip, Configuration.Port);
	}

	public async GDTask SendInput(int tick, Vector2 input) {
		var stream = new MemoryStream();
		await stream.WriteInt(tick);
		var x = (float)Math.Round(input.X, 2);
		var y = (float)Math.Round(input.Y, 2);

		await stream.WriteSingle(x);
		await stream.WriteSingle(y);
		var bytes = stream.GetBuffer();
		await _client.SendAsync(bytes, bytes.Length, Configuration.Ip, Configuration.Port);
	}
}