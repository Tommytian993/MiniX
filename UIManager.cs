using Godot;
using System;

public partial class UIManager : Node2D
{
     // 地形UI场景的打包场景引用，用于动态加载地形信息面板
     PackedScene terrainUiScene;

     public override void _Ready()
     {
          // 在节点准备就绪时加载地形UI场景
          // 从指定路径加载打包的场景资源，用于后续实例化UI面板
          terrainUiScene = ResourceLoader.Load<PackedScene>("res://Scenes/UI/TerrainTileUI.tscn");
     }
}
