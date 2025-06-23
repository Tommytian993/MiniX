using Godot;
using System;

public partial class TerrainTileUi : Panel
{
	 // 当前选中的六边形数据引用，用于存储和访问六边形信息
	 Hex h = null;

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

	 /// 设置当前显示的六边形数据并更新UI
	 /// <param name="h">要显示的六边形对象</param>
	 public void SetHex(Hex h)
	 {
		  // 保存六边形引用，用于后续访问
		  this.h = h;

		  // 更新食物标签显示，显示当前六边形的食物产量
		  foodLabel.Text = $"Food: {h.food}";

		  // 更新生产力标签显示，显示当前六边形的生产力产量
		  productionLabel.Text = $"Production: {h.production}";
	 }
}
