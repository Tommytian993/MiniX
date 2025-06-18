using Godot;
using System;

public partial class HexTileMap : Node2D
{
	 [Export]
	 public int width = 100;  // 地图宽度
	 [Export]
	 public int height = 60;  // 地图高度

	 TileMapLayer baseLayer, borderLayer, overlayLayer;  // 三个图层引用

	 public override void _Ready()
	 {
		// 获取三个图层的引用
		baseLayer = GetNode<TileMapLayer>("BaseLayer");
		borderLayer = GetNode<TileMapLayer>("HexBordersLayer");
		overlayLayer = GetNode<TileMapLayer>("SelectionOverlayLayer");
		GenerateTerrain();  // 生成地形
	 }
	 
	 public override void _Process(double delta){
	 }
	 
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
