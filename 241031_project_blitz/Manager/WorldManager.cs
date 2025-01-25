using Godot;
using Godot.Bridge;
using System.Collections.Generic;
using System.IO;

public partial class WorldManager : Node2D
{
    private string savePath = "C:\\Users\\skyma\\Downloads\\241031ProjectBlitzMapFile"; // 청크 파일 저장 경로

    public static WorldManager instance;

    public override void _Ready()
    {
        base._Ready();
        SaveWorldChunks();
        instance = this;
    }

    public override void _Process(double delta)
    {
        ChunkProcess((float)delta);
    }


    #region Entitys
    public static List<Humanoid> humanoids = new ();
    public static List<Sound> sounds = new();
    public static List<IInteractable> interactables = new();


    #endregion

    #region Navigation
    #endregion

    #region Chunk
    public List<Vector2I> loadedChunks = new();

    const float ChunkUpdatePeriod = 1f;
    float chunnkUpdateTime = 0f;
    int chunkRange = 4;
    
    void ChunkProcess(float delta)
    {
        //Timer
        chunnkUpdateTime += delta;
        if (chunnkUpdateTime < ChunkUpdatePeriod)
            return;
        chunnkUpdateTime = 0;

        //Get Variables
        Vector2 tPos = Player.player != null ? Player.player.GlobalPosition : CameraManager.current.Position;
        Vector2I playerChunk = new(
            Mathf.FloorToInt(tPos.X / 1024f),
            Mathf.FloorToInt(tPos.Y / 1024f)
            );

        List<Vector2I> toUnloadList = new(), toLoadList = new();

        //find outranged chunks
        foreach (var pos in loadedChunks)
            if ((playerChunk - pos).Length() > chunkRange)
                toUnloadList.Add(pos);

        //find inranged chunks
        for (int x = (playerChunk.X - chunkRange); x <= (playerChunk.X + chunkRange); x++)
            for (int y = (playerChunk.Y - chunkRange); y <= (playerChunk.Y + chunkRange); y++)
            {
                Vector2I nowPos = new Vector2I(x, y);

                if ((playerChunk - nowPos).Length() <= chunkRange)
                    if (!loadedChunks.Contains(nowPos))
                        toLoadList.Add(nowPos);
            }

        //delete outranged chunks
        foreach (var pos in toUnloadList)
        {
            UnloadChunk(pos);
            loadedChunks.Remove(pos);
        }

        //add inranged chunks
        foreach (var pos in toLoadList)
        {
            LoadChunk(pos);
            loadedChunks.Add(pos);
        }
    }

    public void SaveWorldChunks()
    {
        // `world.tscn` 인스턴스화
        PackedScene worldScene = ResourceLoader.Load<PackedScene>("res://Scene/world.tscn");
        Node2D world = worldScene.Instantiate<Node2D>();

        // 청크 저장소 생성
        var chunkMap = new Godot.Collections.Dictionary<Vector2I, Chunk>();

        // 저장 경로 생성
        Directory.CreateDirectory(savePath);

        // 모든 노드를 탐색하여 청크에 배치
        foreach (Node child in world.GetChildren())
        {
            if (child is Node2D node)
            {
                Vector2I chunkPos = CalculateChunkPosition(node.GlobalPosition);

                // 해당 청크가 존재하지 않으면 새로 생성
                if (!chunkMap.ContainsKey(chunkPos))
                {
                    chunkMap[chunkPos] = new Chunk();
                    chunkMap[chunkPos].chunkPos = chunkPos;
                }

                // 노드를 청크에 추가
                chunkMap[chunkPos].AddChild(node.Duplicate()); // 복제하여 추가
            }
        }

        // 각 청크 저장
        foreach (var chunkEntry in chunkMap)
        {
            SaveChunk(chunkEntry.Value);
        }

        GD.Print("All chunks have been saved!");

        world.QueueFree();
    }

    private Vector2I CalculateChunkPosition(Vector2 globalPosition)
    {
        // 청크 위치 계산 (interval 크기로 나눠 좌표 계산)
        int chunkX = Mathf.FloorToInt(globalPosition.X / Chunk.interval);
        int chunkY = Mathf.FloorToInt(globalPosition.Y / Chunk.interval);
        return new Vector2I(chunkX, chunkY);
    }

    private void SaveChunk(Chunk chunk)
    {
        string chunkFilePath = $"{savePath}/chunk_{chunk.chunkPos.X}_{chunk.chunkPos.Y}.tscn";

        // 자식 노드의 Owner 설정
        foreach (Node child in chunk.GetChildren())
        {
            child.Owner = chunk; // chunk를 Owner로 설정
        }

        // 청크를 PackedScene으로 패킹
        PackedScene packedScene = new PackedScene();
        Error result = packedScene.Pack(chunk);

        if (result == Error.Ok)
        {
            // PackedScene 저장
            ResourceSaver.Save(packedScene, chunkFilePath);
            GD.Print($"Chunk saved at {chunkFilePath}");
        }
        else
        {
            GD.PrintErr($"Failed to pack chunk at {chunk.chunkPos.X}, {chunk.chunkPos.Y}: {result}");
        }
    }

    void LoadChunk(Vector2I chunkPos)
    {
        string chunkFilePath = $"{savePath}/chunk_{chunkPos.X}_{chunkPos.Y}.tscn";

        if (!File.Exists(chunkFilePath))
        {
            GD.PrintErr($"Chunk file not found: {chunkFilePath}");
            return;
        }

        PackedScene chunkScene = ResourceLoader.Load<PackedScene>(chunkFilePath);
        if (chunkScene != null)
        {
            Chunk chunk = chunkScene.Instantiate<Chunk>();
            AddChild(chunk); // 로드된 청크를 월드에 추가
            GD.Print("chunk.children : " + chunk.GetChildren().Count);
            GD.Print($"Chunk ({chunkPos.X}, {chunkPos.Y}) loaded.");
        }
        else
        {
            GD.PrintErr($"Failed to load chunk: {chunkFilePath}");
        }
    }

    void UnloadChunk(Vector2I chunkPos)
    {
        foreach (Node child in GetChildren())
            if (child is Chunk chunk && chunk.chunkPos == chunkPos)
            {
                RemoveChild(chunk);
                chunk.QueueFree();
                GD.Print($"Chunk ({chunkPos.X}, {chunkPos.Y}) unloaded.");
                return;
            }
        
    }
    #endregion
}

