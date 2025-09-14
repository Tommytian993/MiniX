using Godot;

public partial class Settler : Unit
{
	[Export]
	public bool canFoundCity = true;
	
	public override void _Ready()
	{
		base._Ready();
		
		// Set settler unit properties
		unitName = "Settler";
		productionRequired = 100;
		maxHp = 1;
		hp = 1;
		movePoints = 2;
		maxMovePoints = 2;
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
	
	// Found a new city at the current position
	public void FoundCity()
	{
		if (map.GetHex(this.coords).ownerCity is null && !City.invalidTiles.ContainsKey(map.GetHex(this.coords)))
		{
			bool valid = true;
			foreach (Hex h in map.GetSurroundingHexes(this.coords))
			{
				valid = h.ownerCity is null && !City.invalidTiles.ContainsKey(h);
			}
			if (valid)
			{
				map.CreateCity(this.civ, this.coords, $"Settled City {coords.X}");
				this.DestroyUnit();
			}
		}
	}
}
