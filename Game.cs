using System;
using Godot;

public partial class Game : Node
{
    // 游戏进入场景树时预加载所有地形图片资源
    // 确保UI显示地形信息时不会卡顿或找不到图片
    public override void _EnterTree()
    {
        GD.Print("==== Game _EnterTree called ====");
        TerrainTileUi.LoadTerrainImages();
    }
}
