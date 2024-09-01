using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public partial class SceneManager : Node
{
    [Export]
    private PackedScene playerScene;

    public override void _Ready()
    {
        int index = 0;
        foreach (var item in GameManager.players) 
        {
            Player currentPlayer = playerScene.Instantiate<Player>();
            currentPlayer.Name = item.Id.ToString();
            currentPlayer.SetupPlayer(item.Name);
            AddChild(currentPlayer);

            foreach (Node2D spawnPoint in GetTree().GetNodesInGroup("PlayerSpawnPoints"))
            {
                if (int.Parse(spawnPoint.Name) == index) 
                {
                    currentPlayer.GlobalPosition = spawnPoint.GlobalPosition;
                }
            }
            index++;
        }
    }

    public override void _Process(double delta)
    {
    }

}

