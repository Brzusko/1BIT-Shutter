tool
extends EditorPlugin

const DEBUG_PORT = 6030;

var start_button = preload("res://addons/ClientSpawner/UI/Play.tscn");
var clients_to_run = 0;
var btn;

func _enter_tree():
	btn = start_button.instance();
	add_control_to_bottom_panel(btn, "Client Spawner");
	btn.connect("on_txt_change", self, "process_input");
	btn.connect("launch_clients", self, "launch_clients");

func launch_clients():
	var project_path = ProjectSettings.globalize_path("res://");
	for i in range(0, clients_to_run):
		var port = DEBUG_PORT + i + 1;
		var code = OS.execute("godot_mono", ["--path", project_path, "--remote-debug", "127.0.0.1:" + str(port), '-d'], false);

func process_input(input):
	var client_count = int(input);
	clients_to_run = client_count;

func process_button():
	if(clients_to_run > 0):
		launch_clients();

func _exit_tree():
	if btn != null:
		remove_control_from_bottom_panel(btn);
		btn.free();
