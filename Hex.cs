using Godot;

/// <summary>
/// 六边形瓦片类，存储单个六边形的所有数据
/// </summary>
public class Hex
{
     public readonly Vector2I coordinates;  // 六边形的坐标位置
     public TerrainType terrainType;       // 六边形的地形类型

     public int food;
     public int production;

     public City ownerCity;

     // 构造函数，初始化六边形with coordinates
     public Hex(Vector2I coords)
     {
          this.coordinates = coords;
          this.ownerCity = null;
     }


     // 重写 ToString 方法，提供六边形的可读信息
     public override string ToString()
     {
          return $"Coordinates: ({this.coordinates.X}, {this.coordinates.Y}). TerrainType: {this.terrainType}. Food: {this.food}. Production: {this.production})";
     }
}