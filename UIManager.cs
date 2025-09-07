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
	
	// 通用UI引用
	GeneralUI generalUI;

	[Signal]
	public delegate void EndTurnEventHandler();

	public override void _Ready()
	{
		// 在节点准备就绪时加载地形UI场景
		// 从指定路径加载打包的场景资源，用于后续实例化UI面板
		terrainUiScene = ResourceLoader.Load<PackedScene>("res://TerrainTileUI.tscn");
		cityUiScene = ResourceLoader.Load<PackedScene>("res://city_ui.tscn"); 

		// 调试：检查场景是否正确加载
		if (terrainUiScene == null)
		{
			GD.PrintErr("错误：无法加载TerrainTileUI.tscn场景！请检查文件路径。");
		}
		else
		{
			GD.Print("成功：TerrainTileUI.tscn场景已加载。");
		}
		
		if (cityUiScene == null)
		{
			GD.PrintErr("错误：无法加载CityUI.tscn场景！请检查文件路径。");
		}
		else
		{
			GD.Print("成功：CityUI.tscn场景已加载。");
		}
		
		// 获取GeneralUI引用
		generalUI = GetNode<GeneralUI>("GeneralUI");
		
		// 获取结束回合按钮引用
		Button endTurnButton = generalUI.GetNode<Button>("EndTurnButton");
		
		// 连接按钮信号到SignalEndTurn函数
		endTurnButton.Pressed += SignalEndTurn;
	}


	/// 隐藏所有弹出UI（如地形信息面板），释放相关节点资源
	public void HideAllPopups()
	{
		if (terrainUi is not null)
		{
			terrainUi.QueueFree();
			terrainUi = null;
		}
		if (cityUi is not null)
		{
			cityUi.QueueFree();
			cityUi = null;
		}
	}

	public void SetCityUi(City city)
	{
		GD.Print($"SetCityUi 被调用，城市: {city.name}");
		HideAllPopups();
		GD.Print("开始实例化城市UI...");
		GD.Print($"cityUiScene 是否为null: {cityUiScene == null}");
		if (cityUiScene == null)
		{
			GD.PrintErr("错误：cityUiScene 为 null！");
			return;
		}
		cityUi = cityUiScene.Instantiate() as CityUI;
		GD.Print($"城市UI实例化结果: {cityUi != null}");
		
		GD.Print("添加城市UI到场景树...");
		AddChild(cityUi);
		GD.Print("城市UI已添加到场景树");
		
		// 设置UI位置到屏幕右上角（使用固定大小，因为UI可能还没完全初始化）
		Vector2 screenSize = GetViewport().GetVisibleRect().Size;
		cityUi.Position = new Vector2(screenSize.X - 250 - 10, 10); // 250是UI的宽度
		GD.Print($"UI位置设置为: {cityUi.Position}, 屏幕大小: {screenSize}");
		
		GD.Print("开始设置城市UI数据...");
		cityUi.SetCityUI(city);
		GD.Print("城市UI数据设置完成");
	}

	/// <summary>
	/// 设置地形UI面板，动态创建并显示六边形信息
	/// </summary>
	/// <param name="h">要显示的六边形数据</param>
	public void SetTerrainUi(Hex h)
	{
		GD.Print($"SetTerrainTileUi被调用，六边形数据：{h}");

		// 隐藏所有弹出UI，包括城市UI和地形UI
		HideAllPopups();

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

	public void SignalEndTurn()
	{
		EmitSignal(SignalName.EndTurn);
		generalUI.IncrementTurnCounter();
	}

	public void RefreshUI()
	{
		if (cityUi is not null)
			cityUi.Refresh();
	}
}
