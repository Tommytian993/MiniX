using Godot;
using System;

public partial class UnitUI : Panel
{
	TextureRect unitImage;
	Label unitType, moves, hp;
	Unit u;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		unitImage = GetNode<TextureRect>("TextureRect");
		unitType = GetNode<Label>("UnitTypeLabel");
		hp = GetNode<Label>("HealthLabel");
		moves = GetNode<Label>("MovesLabel");
		GD.Print("UnitUI _Ready 被调用");
	}
	
	public void SetUnit(Unit u)
	{
		this.u = u;
		if (this.u.GetType() == typeof(Settler))
		{
			VBoxContainer actionsContainer = GetNode<VBoxContainer>("VBoxContainer");
			Button foundCityButton = new Button();
			foundCityButton.Text = "Found City";
			actionsContainer.AddChild(foundCityButton);
			Settler settler = this.u as Settler;
			foundCityButton.Pressed += settler.FoundCity;
		}
		Refresh();
		GD.Print($"UnitUI 设置单位: {u?.unitName}");
	}
	
	public void Refresh()
	{
		if (u != null)
		{
			unitImage.Texture = Unit.uiImages[u.GetType()];
			unitType.Text = $"Unit Type: {u.unitName}";
			moves.Text = $"Moves: {u.movePoints}/{u.maxMovePoints}";
			hp.Text = $"HP: {u.hp}/{u.maxHp}";
		}
	}
}
