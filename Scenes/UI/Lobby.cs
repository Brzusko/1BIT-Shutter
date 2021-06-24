using Godot;
using System;
using bit_shuter.Scenes.UI.Interfaces;
public class Lobby : Control, IUI
{
    public void Active() {
        Show();
    }

    public void Disactive() {
        Hide();
    }
}
