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
    public partial class IngameManager : Node2D
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
        
        [Export]
        public CameraManager cmr;

        public static UserStatus?[] players = new UserStatus?[4] { null, null, null, null };
        public static Character[] characters = new Character[4] { null, null, null, null };
        public static Boss boss = null;
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
            Node n = GetNode("./IngameInterface");
            if (n is IngameInterface igUI)
                this.igUI = igUI;
            else
                GD.PrintErr("저기요 인게임 인터페이스가 없어요! 미친 이를 어떠케");

            SpawnAllChar();
            SpawnBoss(BossData.Type.KNIGHT);
        }

        bool SpawnBoss(BossData.Type type)
        {
            //if (boss != null) throw new("보스가 이미 있어요! : " + boss.Name);

            PackedScene bossScene = GetPackedSceneByBossType(type);
            
            if(bossScene == null) return false;

            boss = bossScene.Instantiate<Boss>();

            AddChild(boss);

            Node2D spawnPoint = GetTree().GetFirstNodeInGroup("bossSpawnPoints") as Node2D;
            if (spawnPoint == null) GD.PushWarning("SpawnBoss SpawnBosspawnPoint == null");
            boss.GlobalPosition = spawnPoint.GlobalPosition;
            
            return true;
        }

        bool SpawnChar(int idx) 
        {

            if (characters[idx] != null) return false;
            if (players[idx].HasValue == false) return false;

            var ingameChars = GetTree().GetNodesInGroup("Player");

            foreach (var item in ingameChars)
                if (item.Name == players[idx].Value.id)
                    return false;

            UserStatus uStat = players[idx].Value;
            CharacterData.Type type = uStat.type;
            PackedScene playerScene = GetPackedSceneByCharType(type);

            if (playerScene == null) throw new Exception($"SpawnChar [{type.ToString()}]playerScene == null");

            Character currentPlayer = playerScene.Instantiate<Character>();

            currentPlayer.Name = uStat.rpcId.ToString();
            currentPlayer.SetupPlayer(uStat.name);
            characters[idx] = currentPlayer;

            AddChild(currentPlayer);

            foreach (Node2D spawnPoint in GetTree().GetNodesInGroup("playerSpawnPoints"))
                if (spawnPoint.Name == idx.ToString())
                    currentPlayer.GlobalPosition = spawnPoint.GlobalPosition;

            if (idx == InroomInterface.instance.userIdx)
            {
                igUI.SetCharacter(currentPlayer);
                cmr.SetTarget(currentPlayer);
            }

            GD.Print("Spawn Char of " + idx);
            return true;
        }

        [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
        public bool SpawnEnemy(BossData.Type type, Vector2 position) 
        {

            return false;
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
        PackedScene prefabFuhrer;
        [Export]
        PackedScene prefabSmoke;
        [Export]
        PackedScene prefabSpotlight;
        [Export]
        PackedScene prefabStyx;


        [ExportGroup("BossScene")]
        [Export]
        PackedScene bossSchadenfreude;
        [Export]
        PackedScene bossKnight;


        PackedScene GetPackedSceneByBossType(BossData.Type type)
        {
            switch (type)
            {
                case BossData.Type.NONE: return null;
                case BossData.Type.SCHADENFREUDE: return bossSchadenfreude;
                case BossData.Type.KNIGHT: return bossKnight;
            }
            return null;
        }
        PackedScene GetPackedSceneByCharType(CharacterData.Type type) 
        {
            switch (type) 
            {
                case CharacterData.Type.NONE: return null;
                case CharacterData.Type.FUHRER: return prefabFuhrer;
                case CharacterData.Type.STYX: return prefabStyx;
                case CharacterData.Type.SMOKE: return prefabSmoke;
                case CharacterData.Type.SPOTLIGHT: return prefabSpotlight;
            }
            return null;
        }

    }
}
