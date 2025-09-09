using Godot;

public partial class Settler : Unit
{
	[Export]
	public bool canFoundCity = true;
	
	public override void _Ready()
	{
		base._Ready();
		
		// Set settler-specific movement points
		maxMovementPoints = 2;
		movementPoints = maxMovementPoints;
		
		GD.Print("Settler unit created");
	}
	
	public override void OnInputEvent(Node viewport, InputEvent @event, long shapeIdx)
	{
		base.OnInputEvent(viewport, @event, shapeIdx);
		
		if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
		{
			if (mouseEvent.ButtonIndex == MouseButton.Right)
			{
				GD.Print($"Settler selected - Can found city: {canFoundCity}");
			}
		}
	}
	
	public void FoundCity(Vector2I position)
	{
		if (canFoundCity)
		{
			GD.Print($"Settler founding city at position: {position}");
			// City founding logic will be implemented later
			canFoundCity = false; // Settler can only found one city
		}
		else
		{
			GD.Print("This settler has already founded a city");
		}
	}
}
