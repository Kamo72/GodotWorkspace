
using Godot;

public partial class Chunk : Node2D
{
    [Export]
    public Vector2I chunkPos;
    public const float interval = 1024;


    //public Chunk(Vector2I chunkPos)
    //{
    //    this.chunkPos = chunkPos;
    //}
}
