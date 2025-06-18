using Godot;
using System;

/// <summary>
/// 地形类型枚举，定义游戏中所有可能的地形
/// </summary>
public enum TerrainType{
     Plains,        // 平原
     Water,         // 水域
     Desert,        // 沙漠
     Mountain,      // 山脉
     Ice,           // 冰原
     ShallowWater,  // 浅水
     Beach          // 海滩
}

/// <summary>
/// 六边形瓦片类，存储单个六边形的所有数据
/// </summary>
public class Hex{
     public readonly Vector2I coordinates;  // 六边形的坐标位置
     public TerrainType terrainType;       // 六边形的地形类型
     
     /// <summary>
     /// 构造函数，初始化六边形
     /// </summary>
     /// <param name="coords">六边形的坐标</param>
     public Hex(Vector2I coords){
          this.coordinates = coords;
     }
}

public partial class HexTileMap : Node2D
{
	[Export]
	public int width = 100;  // 地图宽度
	[Export]
	public int height = 60;  // 地图高度

	// 六边形数据数组（二维数组存储）
	public Hex[,] hexes;

	TileMapLayer baseLayer, borderLayer, overlayLayer;  // 三个图层引用

    // 地图数据字典，用于快速查找六边形数据
    Dictionary<Vector2I, Hex> mapData;

    public override void _Ready()
	{
		// 获取三个图层的引用
		baseLayer = GetNode<TileMapLayer>("BaseLayer");
		borderLayer = GetNode<TileMapLayer>("HexBordersLayer");
		overlayLayer = GetNode<TileMapLayer>("SelectionOverlayLayer");
        
        // 初始化地图数据字典
        mapData = new Dictionary<Vector2I, Hex>();
        
		GenerateTerrain();  // 生成地形
	}
	 
	public override void _Process(double delta){
	}
	 
    /// <summary>
    /// 生成地形，创建基础的地图结构
    /// </summary>
    public void GenerateTerrain(){
          // 遍历整个地图网格
          for(int x = 0; x < width; x++){
               for(int y = 0; y < height; y++){
                    // 设置基础图层瓦片
                    baseLayer.SetCell(new Vector2I(x, y), 0, new Vector2I(0, 0));
                    // 设置边框图层瓦片
                    borderLayer.SetCell(new Vector2I(x, y), 0, new Vector2I(0, 0));
               }
          }
     }
     
     /// <summary>
     /// 将地图坐标转换为本地世界坐标
     /// </summary>
     /// <param name="coords">地图坐标 (x, y)</param>
     /// <returns>对应的本地世界坐标</returns>
     public Vector2 MapToLocal(Vector2I coords){
          // 使用基础图层的坐标转换方法
          return baseLayer.MapToLocal(coords);
     }
}
