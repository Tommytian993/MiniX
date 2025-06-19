using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 地形类型枚举，定义游戏中所有可能的地形
/// </summary>
public enum TerrainType{
	 PLAINS,         // 平原
	 WATER,          // 水域
	 DESERT,         // 沙漠
	 MOUNTAIN,       // 山脉
	 ICE,            // 冰原
	 SHALLOW_WATER,  // 浅水
	 BEACH,          // 海滩
	 FOREST          // 森林
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

	// 地形纹理映射字典，将地形类型映射到对应的瓦片坐标
	Dictionary<TerrainType, Vector2I> terrainTextures;

	public override void _Ready()
	{
		// 获取三个图层的引用
		baseLayer = GetNode<TileMapLayer>("BaseLayer");
		borderLayer = GetNode<TileMapLayer>("HexBordersLayer");
		overlayLayer = GetNode<TileMapLayer>("SelectionOverlayLayer");
		
		// 初始化地图数据字典
		mapData = new Dictionary<Vector2I, Hex>();
		
		// 初始化地形纹理映射
		// 每个地形类型对应瓦片图集中的特定坐标
		terrainTextures = new Dictionary<TerrainType, Vector2I>(){
			 {TerrainType.PLAINS, new Vector2I(0, 0)},        // 平原：第0行第0列
			 {TerrainType.WATER, new Vector2I(1, 0)},         // 水域：第0行第1列
			 {TerrainType.DESERT, new Vector2I(0, 1)},        // 沙漠：第1行第0列
			 {TerrainType.MOUNTAIN, new Vector2I(1, 1)},      // 山脉：第1行第1列
			 {TerrainType.SHALLOW_WATER, new Vector2I(1, 2)}, // 浅水：第2行第1列
			 {TerrainType.BEACH, new Vector2I(0, 2)},         // 海滩：第2行第0列
			 {TerrainType.FOREST, new Vector2I(1, 3)},        // 森林：第3行第1列
			 {TerrainType.ICE, new Vector2I(0, 3)},           // 冰原：第3行第0列
		};
		
		GenerateTerrain();  // 生成地形
	}
	 
	public override void _Process(double delta){
	}
	 
	/// <summary>
	/// 生成地形，创建基础的地图结构
	/// </summary>
	public void GenerateTerrain(){
		// 创建多个噪声地图用于不同类型的地形生成
		float[,] noiseMap = new float[width, height];      // 基础地形噪声图
		float[,] forestMap = new float[width, height];     // 森林分布噪声图
		float[,] desertMap = new float[width, height];     // 沙漠分布噪声图
		float[,] mountainMap = new float[width, height];   // 山脉分布噪声图
		
		// 生成随机种子，确保每次生成的地图都不同
		Random r = new Random();
		int seed = r.Next(100000);

		// 创建噪声生成器实例
		FastNoiseLite noise = new FastNoiseLite();
		noise.Seed = seed;                    // 设置噪声种子
		noise.Frequency = 0.008f;            // 设置噪声频率（控制地形变化速度）
		noise.FractalType = FastNoiseLite.FractalTypeEnum.FBM;  // 使用分形布朗运动
		noise.FractalOctaves = 4;            // 设置分形八度（控制细节层次）
		noise.FractalLacunarity = 2.25f;     // 设置分形间隙（控制频率变化）

		// 用于记录噪声的最大值，用于后续归一化
		float noiseMax = 0f;

		// 第一遍遍历：生成基础噪声地图并找到最大值
		 for(int x = 0; x < width; x++){
			 for(int y = 0; y < height; y++){
				  // 获取当前位置的噪声值并取绝对值，确保值为正数
				  noiseMap[x, y] = Mathf.Abs(noise.GetNoise2D(x, y));
				  // 记录噪声的最大值，用于后续归一化处理
				  if (noiseMap[x, y] > noiseMax) noiseMax = noiseMap[x, y];
			 }
		 }

		// 定义地形生成规则：根据噪声值范围确定地形类型
		// 每个元组包含：(最小噪声值, 最大噪声值, 对应的地形类型)
		List<(float Min, float Max, TerrainType type)> terrainGenValues = new List<(float Min, float Max, TerrainType type)>(){
			// 水域：噪声值最低的区域（0 到 25% 的最大值）
			(0, noiseMax / 10 * 2.5f, TerrainType.WATER),
			// 浅水：噪声值较低的区域（25% 到 40% 的最大值）
			(noiseMax / 10 * 2.5f, noiseMax / 10 * 4, TerrainType.SHALLOW_WATER),
			// 海滩：噪声值中等的区域（40% 到 45% 的最大值）
			(noiseMax / 10 * 4, noiseMax / 10 * 4.5f, TerrainType.BEACH),
			// 平原：噪声值较高的区域（45% 到最大值）
			(noiseMax / 10 * 4.5f, noiseMax + 0.05f, TerrainType.PLAINS),
		};

		
		// 遍历整个地图网格
		for(int x = 0; x < width; x++){
			 for(int y = 0; y < height; y++){
				  // 创建新的六边形对象，设置其坐标位置
				  Hex h = new Hex(new Vector2I(x, y));
				  // 获取当前位置的噪声值
				  float noiseValue = noiseMap[x, y];
				  // 根据噪声值确定地形类型，使用地形生成规则
				  h.terrainType = terrainGenValues.First(range => noiseValue >= range.Min && noiseValue <= range.Max).type;
				  // 将六边形数据存储到地图数据字典中
				  mapData[new Vector2I(x, y)] = h;
				  
				  // 设置基础图层瓦片，使用对应地形的纹理坐标
				  baseLayer.SetCell(new Vector2I(x, y), 0, terrainTextures[h.terrainType]);
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
