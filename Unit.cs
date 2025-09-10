using Godot;
using System;
using System.Collections.Generic;

public partial class Unit : Node2D
{
	[Export]
	public int movementPoints = 2;
	
	[Export]
	public int maxMovementPoints = 2;
	
	// 单位名称
	public string unitName = "DEFAULT";
	// 建造所需生产值
	public int productionRequired;
	
	// 单位所属文明
	public Civilization civ;
	public Civilization owner;
	public Vector2I currentPosition;
	
	// 单位场景资源映射
	public static Dictionary<Type, PackedScene> unitSceneResources;
	
	protected Sprite2D sprite;
	protected Area2D area2D;
	
	public override void _Ready()
	{
		sprite = GetNode<Sprite2D>("Sprite2D");
		area2D = GetNode<Area2D>("Sprite2D/Area2D");
		
		// Connect mouse input signal
		area2D.InputEvent += OnInputEvent;
	}
	
	public virtual void OnInputEvent(Node viewport, InputEvent @event, long shapeIdx)
	{
		if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
		{
			if (mouseEvent.ButtonIndex == MouseButton.Left)
			{
				GD.Print($"Unit clicked: {GetType().Name}");
			}
		}
	}
	
	public virtual void MoveTo(Vector2I newPosition)
	{
		currentPosition = newPosition;
		// Movement logic will be implemented later
		GD.Print($"Unit moving to position: {newPosition}");
	}
	
	public virtual void ResetMovement()
	{
		movementPoints = maxMovementPoints;
	}
	
	// 加载单位场景资源
	public static void LoadUnitScenes()
	{
		unitSceneResources = new Dictionary<Type, PackedScene>
		{
			{ typeof(Settler), ResourceLoader.Load<PackedScene>("res://Settler.tscn") },
			{ typeof(Warrior), ResourceLoader.Load<PackedScene>("res://Warrior.tscn") }
		};
		GD.Print("单位场景资源已加载");
	}
	
	// 设置单位所属文明
	public void SetCiv(Civilization civ)
	{
		this.civ = civ;
		GetNode<Sprite2D>("Sprite2D").Modulate = civ.territoryColor;
		this.civ.units.Add(this);
		GD.Print($"单位 {unitName} 已分配给文明 {civ.name}");
	}
}
