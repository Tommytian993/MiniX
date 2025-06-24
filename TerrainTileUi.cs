using Godot;
using System;
using System.Collections.Generic;

public partial class TerrainTileUi : Panel
{
	 // 地形类型到字符串的映射字典，用于UI显示地形名称
	 // 静态字典，所有实例共享，便于维护和本地化
	 public static Dictionary<TerrainType, string> terrainTypeStrings = new Dictionary<TerrainType, string>
		{
				  {TerrainType.PLAINS, "Plains"},
				  {TerrainType.WATER, "Water"},
				  {TerrainType.DESERT, "Desert"},
				  {TerrainType.MOUNTAIN, "Mountain"},
				  {TerrainType.SHALLOW_WATER, "Shallow Water"},
				  {TerrainType.BEACH, "Beach"},
				  {TerrainType.FOREST, "Forest"},
		};

	 // 地形类型到图片纹理的映射字典，用于UI显示地形图片
	 // 静态字典，避免重复加载图片资源
	 public static Dictionary<TerrainType, Texture2D> terrainTypeImages = new();

	 // 静态方法：加载所有地形类型的图片资源到字典中
	 // 只需在游戏启动时调用一次，后续直接查字典即可
	 public static void LoadTerrainImages()
	 {
		  Texture2D plains = ResourceLoader.Load("res://textures/plains.jpg") as Texture2D;
		  GD.Print($"plains loaded: {plains != null}");
		  Texture2D beach = ResourceLoader.Load("res://textures/beach.jpg") as Texture2D;
		  GD.Print($"beach loaded: {beach != null}");
		  Texture2D desert = ResourceLoader.Load("res://textures/desert.jpg") as Texture2D;
		  GD.Print($"desert loaded: {desert != null}");
		  Texture2D mountain = ResourceLoader.Load("res://textures/mountain.jpg") as Texture2D;
		  GD.Print($"mountain loaded: {mountain != null}");
		  Texture2D ice = ResourceLoader.Load("res://textures/ice.jpg") as Texture2D;
		  GD.Print($"ice loaded: {ice != null}");
		  Texture2D ocean = ResourceLoader.Load("res://textures/ocean.jpg") as Texture2D;
		  GD.Print($"ocean loaded: {ocean != null}");
		  Texture2D shallow = ResourceLoader.Load("res://textures/shallow.jpg") as Texture2D;
		  GD.Print($"shallow loaded: {shallow != null}");
		  Texture2D forest = ResourceLoader.Load("res://textures/forest.jpg") as Texture2D;
		  GD.Print($"forest loaded: {forest != null}");

		  terrainTypeImages = new Dictionary<TerrainType, Texture2D>{
	  { TerrainType.PLAINS, plains},
	  { TerrainType.BEACH, beach},
	  { TerrainType.DESERT, desert},
	  { TerrainType.MOUNTAIN, mountain},
	  { TerrainType.ICE, ice},
	  { TerrainType.WATER, ocean},
	  { TerrainType.SHALLOW_WATER, shallow},
	  { TerrainType.FOREST, forest}
	 };
	 }

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
		  this.h = h;
		  terrainImage.Texture = terrainTypeImages[h.terrainType];
		  foodLabel.Text = $"Food: {h.food}";
		  productionLabel.Text = $"Production: {h.production}";
		  terrainLabel.Text = $"Terrain: {terrainTypeStrings[h.terrainType]}";
	 }
}
