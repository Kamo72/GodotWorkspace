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
            //GameManager.players에 따라 캐릭터를 각각 생성
            Player currentPlayer = playerScene.Instantiate<Player>();
            currentPlayer.Name = item.Id.ToString();
            currentPlayer.SetupPlayer(item.Name);
            AddChild(currentPlayer);

            //PlayerSpawnPoints에 포함된 애들 중 0부터 자리를 채워넣음
            foreach (Node2D spawnPoint in GetTree().GetNodesInGroup("PlayerSpawnPoints"))
                if (int.Parse(spawnPoint.Name) == index) 
                    currentPlayer.GlobalPosition = spawnPoint.GlobalPosition;
                
            
            index++;
        }
    }

    public override void _Process(double delta)
    {
    }

}

