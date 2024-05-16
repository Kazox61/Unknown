using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using GodotTask;
using Unknown.Utilities;

namespace Unknown.Game; 

public partial class ClientMessageManager : Node {
	public Action<int, float, float> OnMessage;
	
	private UdpClient _client;
	public override void _Ready() {
		_client = new UdpClient();
		_client.Client.Bind(new IPEndPoint(IPAddress.Any, 19098));
		var thread = new Thread(StartProcess);
		thread.Start();
	}

	private void StartProcess() {
		ProcessAsync();
	}
	
	private async Task ProcessAsync() {
		while (true) {
			try {
				var result = await _client.ReceiveAsync();

				var reader = new Reader(result.Buffer);
				var tick = reader.ReadInt32();
				var x = reader.ReadSingle();
				var y = reader.ReadSingle();
				
				GD.Print($"[Client] {tick} received " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
				Callable.From(() => OnMessage?.Invoke(tick, x, y)).CallDeferred();
				//OnMessage?.Invoke(tick, x, y);
			}
			catch (Exception e) {
				GD.Print(e);
			}
		}
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