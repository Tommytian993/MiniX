using Godot;
using System;
using System.Collections.Generic;

public class Civilization
{
	public int id;
	public List<City> cities;
	public Color territoryColor;
	public string name;
	public bool playerCiv;

	public Civilization()
	{
		cities = new List<City>();
	}
}
