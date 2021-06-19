using Godot;
using System;
using bit_shuter.Autoload.Structs;

public class Network : Node
{	
	public bool IsClientConnected {
		get => _network_peer.GetConnectionStatus() > 0;
	}
	private static string _debug_address = "ws://127.0.0.1:7171";
	private static string _debug_client_name = "Zdzisiek";
	private WebSocketClient _network_peer = new WebSocketClient();
	private void CreateClient() {
		var error = _network_peer.ConnectToUrl(_debug_address, null, true, null);
		_network_peer.Connect("connection_succeeded", this, "OnConnection");
		if (error != Error.Ok) return;
		GetTree().NetworkPeer = _network_peer;
	}
	
	private void ClearClient() {
		if(!IsClientConnected) return;
		_network_peer.DisconnectFromHost();
		_network_peer.Disconnect("connection_succeeded", this, nameof(OnConnection));
	}

	public void OnConnection() {
		GD.Print("Connected to the server");
		SendCredentials();
	}


	#region RPCs
	public void SendCredentials() {
		RpcId(1, "ReciveClientCredentials", new Credentials{
			ClientName = _debug_client_name,
		}.ToGodotDict());
	}
	
	public void ClockSyncFinished() {
		
	}

	[Remote]
	public void StartClockSync() {
		var clock = GetNode<Clock>("/root/Clock");
		clock.StartClockSyncing();
	}

	#endregion

	#region Virutal Methods
	public override void _Process(float delta) {
		if(!IsClientConnected) return;
		_network_peer.Poll();
	}

	#endregion
}
