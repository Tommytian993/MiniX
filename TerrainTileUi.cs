using Godot;
using System;

public partial class TerrainTileUi : Panel
{
     // UI组件引用：地形图片显示区域
     TextureRect terrainImage;

     // UI组件引用：各种信息标签
     Label terrainLabel, foodLabel, productionLabel;

     public override void _Ready()
     {
          // 获取场景中的UI节点引用，用于后续动态更新内容
          terrainImage = GetNode<TextureRect>("TerrainImage");    // 地形图片显示区域
          terrainLabel = GetNode<Label>("TerrainLabel");          // 地形类型标签
          foodLabel = GetNode<Label>("FoodLabel");                // 食物产量标签
          productionLabel = GetNode<Label>("ProductionLabel");    // 生产力产量标签
     }
}
