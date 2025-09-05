using Godot;
using System;
using System.Collections.Generic;

public partial class City : Node2D
{
	public HexTileMap map;
	public Vector2I centerCoordinates;

	public List<Hex> territory;
	public List<Hex> borderTilePool;

	public Civilization civ;

	public string name;

	// Population
	public int population = 1;
	// Resources
	public int totalFood;
	public int totalProduction;

	Label label;
	Sprite2D sprite;

	public override void _Ready()
	{
		label = GetNode<Label>("Label");
		sprite = GetNode<Sprite2D>("Sprite2D");
		territory = new List<Hex>();
		borderTilePool = new List<Hex>();
		
		// 如果name已经设置，则更新label
		if (!string.IsNullOrEmpty(name))
		{
			label.Text = name;
		}
	}

	public void AddTerritory(List<Hex> territoryToAdd)
	{
		foreach (Hex h in territoryToAdd)
		{
			h.ownerCity = this;
		}
		territory.AddRange(territoryToAdd);
		CalculateTerritoryResourceTotals();
	}

	public void CalculateTerritoryResourceTotals()
	{
		totalFood = 0;
		totalProduction = 0;
		foreach (Hex h in territory)
		{
			totalFood += h.food;
			totalProduction += h.production;
		}
		GD.Print($"城市 {name} 资源计算完成: 食物={totalFood}, 生产={totalProduction}, 领土数量={territory.Count}");
	}

	public void SetCityName(string newName)
	{
		name = newName;
		label.Text = newName;
	}
	public void SetIconColor(Color c)
	{
		sprite.Modulate = c;
	}
}
