using Godot;
using System;

public partial class UIManager : Node2D
{
	// 地形UI场景的打包场景引用，用于动态加载地形信息面板
	PackedScene terrainUiScene;
	PackedScene cityUiScene;

	// 当前活跃的地形UI面板实例，用于管理UI生命周期
	TerrainTileUi terrainUi;
	CityUI cityUi;

	public override void _Ready()
	{
		// 在节点准备就绪时加载地形UI场景
		// 从指定路径加载打包的场景资源，用于后续实例化UI面板
		terrainUiScene = ResourceLoader.Load<PackedScene>("res://TerrainTileUI.tscn");
		cityUiScene = ResourceLoader.Load<PackedScene>("res://CityUI.tscn"); 

		// 调试：检查场景是否正确加载
		if (terrainUiScene == null)
		{
			GD.PrintErr("错误：无法加载TerrainTileUI.tscn场景！请检查文件路径。");
		}
		else
		{
			GD.Print("成功：TerrainTileUI.tscn场景已加载。");
		}
	}


	/// 隐藏所有弹出UI（如地形信息面板），释放相关节点资源
	public void HideAllPopups()
	{
		if (terrainUi is not null)
		{
			terrainUi.QueueFree();
		}
		if (cityUi is not null)
		{
			cityUi.QueueFree();
		}
	}


	/// <summary>
	/// 设置地形UI面板，动态创建并显示六边形信息
	/// </summary>
	/// <param name="h">要显示的六边形数据</param>
	public void SetTerrainUi(Hex h)
	{
		GD.Print($"SetTerrainTileUi被调用，六边形数据：{h}");

		// 如果已存在UI面板，先清理旧的实例
		if (terrainUi is not null)
		{
			GD.Print("清理旧的UI面板");
			terrainUi.QueueFree();  // 将旧UI面板加入释放队列
		}

		// 从打包场景实例化新的UI面板
		terrainUi = terrainUiScene.Instantiate() as TerrainTileUi;
		GD.Print($"新UI面板已实例化：{terrainUi != null}");

		// 将新UI面板添加到场景树中，使其可见
		// 这一步很重要：UI节点必须先添加到场景树中，其子节点才能被正确访问
		AddChild(terrainUi);
		GD.Print("UI面板已添加到场景树");

		// 设置新UI面板的六边形数据并更新显示
		// 必须在AddChild之后调用，确保UI组件已经正确初始化
		terrainUi.SetHex(h);
		GD.Print("六边形数据已设置到UI面板");
	}
}
