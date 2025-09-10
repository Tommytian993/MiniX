using Godot;
using System;

public partial class CityUI : Panel
{
	Label cityName, population, food, production;
	UnitBuildButton settlerButton, warriorButton;
	// City data
	City city;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Print("CityUI _Ready 被调用");
		cityName = GetNode<Label>("CityName");
		population = GetNode<Label>("Population");
		food = GetNode<Label>("Food");
		production = GetNode<Label>("Production");
		
		// Get unit build buttons
		settlerButton = GetNode<UnitBuildButton>("BuildScrollContainer/BuildVBox/SettlerButton");
		warriorButton = GetNode<UnitBuildButton>("BuildScrollContainer/BuildVBox/WarriorButton");
		
		GD.Print($"CityUI 节点获取完成: cityName={cityName != null}, population={population != null}, food={food != null}, production={production != null}");
		GD.Print($"按钮获取完成: settlerButton={settlerButton != null}, warriorButton={warriorButton != null}");
	}

	public void SetCityUI(City city)
	{
		GD.Print($"SetCityUI: 设置城市 {city.name}, 人口: {city.population}, 食物: {city.totalFood}, 生产: {city.totalProduction}");
		this.city = city;
		cityName.Text = this.city.name;
		population.Text = "Population: " + this.city.population;
		food.Text = "Food: " + this.city.totalFood;
		production.Text = "Production: " + this.city.totalProduction;
		GD.Print($"UI文本已设置: {cityName.Text}, {population.Text}, {food.Text}, {production.Text}");
		
		// 连接单位建造按钮信号
		ConnectUnitBuildSignals(this.city);
	}

	public void Refresh()
	{
		cityName.Text = this.city.name;
		population.Text = "Population: " + this.city.population;
		food.Text = "Food: " + this.city.totalFood;
		production.Text = "Production: " + this.city.totalProduction;
	}
	
	// Refresh重载方法，用于信号连接
	public void Refresh(Unit u)
	{
		Refresh();
		PopulateUnitQueueUI(this.city);
	}
	
	public void SetupUnitButtons()
	{
		// For now, we'll set up placeholder units
		// In the next lesson, we'll create actual unit instances
		GD.Print("设置单位按钮 - 暂时使用占位符");
	}
	
	// 填充单位队列UI显示
	public void PopulateUnitQueueUI(City city)
	{
		VBoxContainer queue = GetNode<VBoxContainer>("QueueScrollContainer/QueueVBox");
		
		// 清除现有队列显示
		foreach (Node n in queue.GetChildren())
		{
			queue.RemoveChild(n);
			n.QueueFree();
		}
		
		// 显示队列中的单位
		for (int i = 0; i < city.unitBuildQueue.Count; i++)
		{
			Unit u = city.unitBuildQueue[i];
			if (i == 0) // 当前正在建造的单位
			{
				Label currentLabel = new Label();
				currentLabel.Text = $"{u.unitName} {city.unitBuildTracker}/{u.productionRequired}";
				queue.AddChild(currentLabel);
			}
			else // 队列中等待的单位
			{
				Label queueLabel = new Label();
				queueLabel.Text = $"{u.unitName} 0/{u.productionRequired}";
				queue.AddChild(queueLabel);
			}
		}
		
		GD.Print($"更新单位队列UI: {city.unitBuildQueue.Count} 个单位在队列中");
	}
	
	// 连接单位建造按钮信号
	public void ConnectUnitBuildSignals(City city)
	{
		// 设置按钮对应的单位类型
		settlerButton.u = new Settler();
		warriorButton.u = new Warrior();
		
		// 连接信号到城市建造队列
		settlerButton.OnPressed += city.AddUnitToBuildQueue;
		settlerButton.OnPressed += this.Refresh;
		warriorButton.OnPressed += city.AddUnitToBuildQueue;
		warriorButton.OnPressed += this.Refresh;
		
		GD.Print("单位建造按钮信号已连接");
	}
}
