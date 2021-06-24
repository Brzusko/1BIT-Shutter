using Godot;
using Godot.Collections;
using System;
using bit_shuter.Autoload.Structs;

public class Network : Node
{	
	[Signal]
	public delegate void RequestUIChange(string className);

	[Signal]
	public delegate void LobbyStateChanged(Dictionary<string, object> lobbyState);
	public bool IsClientConnected {
		get => _network_peer.GetConnectionStatus() > 0;
	}
	public const string _debug_address = "ws://127.0.0.1:7171";
	public const string _debug_client_name = "Zdzisiek";

	private string _userName;
	private WebSocketClient _network_peer = new WebSocketClient();
	public void CreateClient(string url = _debug_address, string userName = _debug_client_name) {
		var error = _network_peer.ConnectToUrl(url, null, true, null);
		_network_peer.Connect("connection_succeeded", this, "OnConnection");
		_userName = userName;
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

	[Remote]
	public void StartClockSync() {
		var clock = GetNode<Clock>("/root/Clock");
		clock.StartClockSyncing();
	}

	[Remote]
	public void ReciveLobbyState(Dictionary<string, object> lobbyState) => EmitSignal(nameof(LobbyStateChanged), lobbyState);

	[Remote]
	public void ChangeUIScene(string className) => EmitSignal(nameof(RequestUIChange), className);

	public void SendReadyState(bool state) => RpcId(1, "ReciveReadyState", state);
	public void SendCredentials() {
		RpcId(1, "ReciveClientCredentials", new Credentials {
			ClientName = _userName,
		}.ToGodotDict());
	}
	
	public void LobbyLoaded() => RpcId(1, "ClientLoadedLobby");
	public void ClockSyncFinished() => RpcId(1, "ClientClockSyncFinished");
	#endregion

	#region Virutal Methods
	public override void _Process(float delta) {
		if(!IsClientConnected) return;
		_network_peer.Poll();
	}

	#endregion
}
