using _favorClient.controls;
using _favorClient.Entity;
using _favorClient.library;
using _favorClient.library.DataType;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _favorClient.System.Ingame
{
    public partial class IngameManager : Node
    {
        public static IngameManager instance = null;

        public enum State 
        {
            PRE_LOADING,
            INITIATING,
            INGAME,
            FINALIZING,
            POST_LOADING,
        }
        public State state = State.INITIATING;

        public IngameInterface igUI;

        public static UserStatus?[] players = new UserStatus?[4] { null, null, null, null };
        public static Character[] characters = new Character[4] { null, null, null, null };
        public static int playersCount { get {
                int count = 0;
                foreach (var item in players)
                    if(item.HasValue) count++;
                return count;
            } }


        public override void _EnterTree()
        {
            instance = this;
            base._EnterTree();
        }

        public override void _Ready()
        {
            Node n = GetNode("../IngameInterface");
            if (n is IngameInterface igUI)
                this.igUI = igUI;
            else
                GD.PrintErr("저기요 인게임 인터페이스가 없어요! 미친 이를 어떠케");

            SpawnAllChar();
        }


        bool SpawnChar(int idx) 
        {
            if (characters[idx] != null) return false;
            if (players[idx].HasValue == false) return false;

            var ingameChars = GetTree().GetNodesInGroup("Player");

            foreach (var item in ingameChars)
                if (item.Name == players[idx].Value.id)
                    return false;



            PackedScene playerScene = null;
            //GameManager.players에 따라 캐릭터를 각각 생성
            Character currentPlayer = playerScene.Instantiate<Character>();

            //fix needed
            //currentPlayer.Name = item.Id.ToString();
            //currentPlayer.SetupPlayer(item.Name);

            AddChild(currentPlayer);

            //in GODOT, PlayerSpawnPoints group needed
            //PlayerSpawnPoints에 포함된 애들 중 0부터 자리를 채워넣음
            foreach (Node2D spawnPoint in GetTree().GetNodesInGroup("PlayerSpawnPoints"))
                if (int.Parse(spawnPoint.Name) == idx)
                    currentPlayer.GlobalPosition = spawnPoint.GlobalPosition;
                
            



            return true;
        }

        public bool DespawnChar(int idx)
        {
            if (players[idx].HasValue) return false;

            if (characters[idx] == null) return false;

            characters[idx].QueueFree();
            
            return true;
        }

        void SpawnAllChar()
        {
            for (int i = 0; i < 4; i++)
                SpawnChar(i);
        }


        [ExportGroup("CharacterScene")]
        [Export]
        PackedScene prefabTest;

        PackedScene GetPackedSceneByCharType(CharacterData.Type type) 
        {
            switch (type) 
            {
                case CharacterData.Type.NONE: return null;
                case CharacterData.Type.WARRIOR: return prefabTest;
            }
            return null;
        }

    }
}
