using Godot;

public partial class Warrior : Unit
{
	[Export]
	public int attackPower = 3;
	
	[Export]
	public int defensePower = 2;
	
	public override void _Ready()
	{
		base._Ready();
		
		// Set warrior-specific movement points
		maxMovementPoints = 2;
		movementPoints = maxMovementPoints;
		
		GD.Print("Warrior unit created");
	}
	
	public override void OnInputEvent(Node viewport, InputEvent @event, long shapeIdx)
	{
		base.OnInputEvent(viewport, @event, shapeIdx);
		
		if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
		{
			if (mouseEvent.ButtonIndex == MouseButton.Right)
			{
				GD.Print($"Warrior selected - Attack: {attackPower}, Defense: {defensePower}");
			}
		}
	}
	
	public void Attack(Unit target)
	{
		GD.Print($"Warrior attacking with {attackPower} attack power");
		// Combat logic will be implemented later
	}
}
