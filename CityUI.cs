using Godot;
using System;

public partial class CityUI : Panel
{
	Label cityName, population, food, production;
	// City data
	City city;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		cityName = GetNode<Label>("CityName");
		population = GetNode<Label>("Population");
		food = GetNode<Label>("Food");
		production = GetNode<Label>("Production");
	}

	public void SetCityUI(City city)
	{
		this.city = city;
		cityName.Text = this.city.name;
		population.Text = "Population: " + this.city.population;
		food.Text = "Food: " + this.city.totalFood;
		production.Text = "Production: " + this.city.totalProduction;
	}
}
