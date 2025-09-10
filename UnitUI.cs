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
		Refresh();
		GD.Print($"UnitUI 设置单位: {u?.unitName}");
	}
	
	public void Refresh()
	{
		if (u != null)
		{
			unitType.Text = u.unitName;
			hp.Text = "Health: 100"; // 暂时硬编码
			moves.Text = $"Moves: {u.movementPoints}/{u.maxMovementPoints}";
		}
	}
}
