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
		GD.Print("CityUI _Ready 被调用");
		cityName = GetNode<Label>("CityName");
		population = GetNode<Label>("Population");
		food = GetNode<Label>("Food");
		production = GetNode<Label>("Production");
		GD.Print($"CityUI 节点获取完成: cityName={cityName != null}, population={population != null}, food={food != null}, production={production != null}");
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
	}

	public void Refresh()
	{
		cityName.Text = this.city.name;
		population.Text = "Population: " + this.city.population;
		food.Text = "Food: " + this.city.totalFood;
		production.Text = "Production: " + this.city.totalProduction;
	}
}
