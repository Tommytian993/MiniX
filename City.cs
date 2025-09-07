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
	
	// Population growth tracking
	public static int POPULATION_THRESHOLD_INCREASE = 15;
	public int populationGrowthThreshold;
	public int populationGrowthTracker;
	
	// Static dictionary to prevent cities from expanding into the same tiles
	public static Dictionary<Hex, City> invalidTiles = new Dictionary<Hex, City>();

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
			// Add new border hexes to the border tile pool
			AddValidNeighborsToBorderPool(h);
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

	public void ProcessTurn()
	{
		CleanUpBorderPool();
		populationGrowthTracker += totalFood;
		if (populationGrowthTracker > populationGrowthThreshold) // Grow population
		{
			population++;
			populationGrowthTracker = 0;
			populationGrowthThreshold += POPULATION_THRESHOLD_INCREASE;
			// Grow territory
			AddRandomNewTile();
			map.UpdateCivTerritoryMap(civ);
			GD.Print($"城市 {name} 人口增长到 {population}，新阈值: {populationGrowthThreshold}");
		}
	}

	public void AddValidNeighborsToBorderPool(Hex h)
	{
		List<Hex> neighbors = map.GetSurroundingHexes(h.coordinates);
		foreach (Hex n in neighbors)
		{
			if (IsValidNeighborTile(n)) borderTilePool.Add(n);
			invalidTiles[n] = this;
		}
	}

	public bool IsValidNeighborTile(Hex n)
	{
		if (n.terrainType == TerrainType.WATER ||
			n.terrainType == TerrainType.ICE ||
			n.terrainType == TerrainType.SHALLOW_WATER ||
			n.terrainType == TerrainType.MOUNTAIN)
		{
			return false;
		}
		if (n.ownerCity != null && n.ownerCity.civ != null)
		{
			return false;
		}
		if (invalidTiles.ContainsKey(n) && invalidTiles[n] != this)
		{
			return false;
		}
		return true;
	}

	public void AddRandomNewTile()
	{
		if (borderTilePool.Count > 0)
		{
			Random r = new Random();
			int index = r.Next(borderTilePool.Count);
			this.AddTerritory(new List<Hex>{borderTilePool[index]});
			borderTilePool.RemoveAt(index);
		}
	}

	public void CleanUpBorderPool()
	{
		List<Hex> toRemove = new List<Hex>();
		foreach (Hex b in borderTilePool)
		{
			if (invalidTiles.ContainsKey(b) && invalidTiles[b] != this)
			{
				toRemove.Add(b);
			}
		}
		foreach (Hex b in toRemove)
		{
			borderTilePool.Remove(b);
		}
	}
}
