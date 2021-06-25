using Godot;
using System;
using bit_shuter.Scenes.UI.Interfaces;

public class MainMenu : Control, IUI
{
    [Export]
    private NodePath _nameFiledPath;
    [Export]
    private NodePath _serverFieldPath;
    [Export]
    private NodePath _connectBTNPath;

    private LineEdit _nameField;
    private LineEdit _serverField;
    private Button _connectBTN;

    public override void _Ready()
    {
        _nameField = GetNode<LineEdit>(_nameFiledPath);
        _serverField = GetNode<LineEdit>(_serverFieldPath);
        _connectBTN = GetNode<Button>(_connectBTNPath);
        _connectBTN.Connect("pressed", this, nameof(OnConnectClick));
    }

    public void OnConnectClick() {
        if(_nameField.Text.Length < 2 && _serverField.Text.Length < 5) return;
        GetNode<Network>("/root/Network").CreateClient(_serverField.Text, _nameField.Text);
    }

    public void Active() => Show();
    public void Disactive() => Hide();
}
