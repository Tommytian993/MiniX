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
     public static Dictionary<TerrainType, Texture2D> terrainTypeTextures = new();

     // 静态方法：加载所有地形类型的图片资源到字典中
     // 只需在游戏启动时调用一次，后续直接查字典即可
     public static void LoadTerrainImages()
     {
          // 依次加载每种地形的图片资源
          Texture2D plains = ResourceLoader.Load<Texture2D>("res://textures/terrain/plains.jpg") as Texture2D;
          Texture2D water = ResourceLoader.Load<Texture2D>("res://textures/terrain/water.jpg") as Texture2D;
          Texture2D desert = ResourceLoader.Load<Texture2D>("res://textures/terrain/desert.jpg") as Texture2D;
          Texture2D mountain = ResourceLoader.Load<Texture2D>("res://textures/terrain/mountain.jpg") as Texture2D;
          Texture2D shallowWater = ResourceLoader.Load<Texture2D>("res://textures/terrain/shallow_water.jpg") as Texture2D;
          Texture2D beach = ResourceLoader.Load<Texture2D>("res://textures/terrain/beach.jpg") as Texture2D;
          Texture2D forest = ResourceLoader.Load<Texture2D>("res://textures/terrain/forest.jpg") as Texture2D;

          // 将加载的图片与地形类型对应存入字典
          terrainTypeTextures.Add(TerrainType.PLAINS, plains);
          terrainTypeTextures.Add(TerrainType.WATER, water);
          terrainTypeTextures.Add(TerrainType.DESERT, desert);
          terrainTypeTextures.Add(TerrainType.MOUNTAIN, mountain);
          terrainTypeTextures.Add(TerrainType.SHALLOW_WATER, shallowWater);
          terrainTypeTextures.Add(TerrainType.BEACH, beach);
          terrainTypeTextures.Add(TerrainType.FOREST, forest);
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
          // 保存六边形引用，用于后续访问
          this.h = h;

          // 更新地形图片显示，使用静态字典中的纹理
          terrainImage.Texture = terrainTypeTextures[h.terrainType];

          // 更新食物标签显示，显示当前六边形的食物产量
          foodLabel.Text = $"Food: {h.food}";

          // 更新生产力标签显示，显示当前六边形的生产力产量
          productionLabel.Text = $"Production: {h.production}";

          // 更新地形类型标签显示，使用静态字典中的字符串
          terrainLabel.Text = $"Terrain: {terrainTypeStrings[h.terrainType]}";
     }
}
