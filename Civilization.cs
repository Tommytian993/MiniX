using Godot;
using System;
using System.Collections.Generic;

public class Civilization
{
	public int id;
	public List<City> cities;
	public List<Unit> units;
	public Color territoryColor;
	public int territoryColorAltTileId;
	public string name;
	public bool playerCiv;

	public Civilization()
	{
		cities = new List<City>();
		units = new List<Unit>();
	}

	/// <summary>
	/// 设置随机颜色
	/// </summary>
	public void SetRandomColor()
	{
		Random r = new Random();
		territoryColor = new Color(r.Next(255)/255.0f, r.Next(255)/255.0f, r.Next(255)/255.0f);
	}

	public void ProcessTurn()
	{
		foreach (City c in cities)
		{
			c.ProcessTurn();
		}
	}
}
