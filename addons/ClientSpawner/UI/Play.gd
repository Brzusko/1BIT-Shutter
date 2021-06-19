tool
extends VBoxContainer
signal launch_clients();
signal on_txt_change(new_text);

func _on_SpawnBTN_pressed():
	emit_signal("launch_clients");

func _on_Prompt_text_changed(new_text):
	emit_signal("on_txt_change", new_text);
	var isNumber = int(new_text);
	if isNumber == 0:
		reset_input();

func reset_input():
	$SpawnForm/Prompt.text = "";
