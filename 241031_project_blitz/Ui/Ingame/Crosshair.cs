using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public partial class Crosshair : Control
{
    Control dynamicDot => this.FindByName("DynamicDot") as Control;
    Control staticDot => this.FindByName("StaticDot") as Control;

    public override void _Process(double delta)
    {
        Player player = Player.player;
        if (player == null) return;

        Visible = !player.isInventory;

        Vector2 screenSize = ((Control)GetParent()).Size;
        dynamicDot.Position = player.realAimPoint - CameraManager.current.Position + screenSize / 2f;
        //staticDot.Position = player.intelligence.vectorMap["AimPos"] - CameraManager.current.Position + screenSize / 2f;
        staticDot.Position = GetGlobalMousePosition();
    }
}
