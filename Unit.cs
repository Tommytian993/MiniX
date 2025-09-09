using Godot;
using System;

public partial class Unit : Node2D
{
	[Export]
	public int movementPoints = 2;
	
	[Export]
	public int maxMovementPoints = 2;
	
	public Civilization owner;
	public Vector2I currentPosition;
	
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
}
