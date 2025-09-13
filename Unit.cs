using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Unit : Node2D
{
	[Export]
	public int movementPoints = 2;
	
	[Export]
	public int maxMovementPoints = 2;
	
	// Health properties
	public int maxHp;
	public int hp;
	// Movement point properties
	public int maxMovePoints;
	public int movePoints;
	
	// Unit name
	public string unitName = "DEFAULT";
	// Production cost required to build this unit
	public int productionRequired;
	
	// Civilization that owns this unit
	public Civilization civ;
	public Civilization owner;
	public Vector2I currentPosition;
	// Unit coordinates on the map
	public Vector2I coords = new Vector2I();
	
	// Unit selection state
	public bool selected = false;
	
	// Valid movement hexes for this unit
	public List<Hex> validMovementHexes = new List<Hex>();
	
	// Impassable terrain types that units cannot move into
	public HashSet<TerrainType> impassible = new HashSet<TerrainType>
	{
		TerrainType.WATER,
		TerrainType.SHALLOW_WATER,
		TerrainType.ICE,
		TerrainType.MOUNTAIN
	};
	
	// Signal emitted when unit is clicked
	[Signal]
	public delegate void UnitClickedEventHandler(Unit u);
	
	// Static dictionary to track all unit positions on the map
	public static Dictionary<Hex, List<Unit>> unitLocations = new Dictionary<Hex, List<Unit>>();
	
	// Unit scene resource mapping
	public static Dictionary<Type, PackedScene> unitSceneResources;
	// Unit UI image resource mapping
	public static Dictionary<Type, Texture2D> uiImages;
	
	protected Sprite2D sprite;
	protected Area2D area2D;
	public Area2D collider;
	public HexTileMap map;
	
	public override void _Ready()
	{
		sprite = GetNode<Sprite2D>("Sprite2D");
		area2D = GetNode<Area2D>("Sprite2D/Area2D");
		collider = GetNode<Area2D>("Sprite2D/Area2D");
		
		// Connect mouse input signal
		area2D.InputEvent += OnInputEvent;
		
		// Connect UnitClicked signal to UIManager
		UIManager manager = GetNode<UIManager>("/root/Game/CanvasLayer/UiManager");
		this.UnitClicked += manager.SetUnitUI;
		
		// Get reference to HexTileMap for tile deselection
		map = GetNode<HexTileMap>("/root/Game/HexTileMap");
		this.UnitClicked += map.DeselectCurrentCell;
		
		// Calculate valid movement hexes when unit is ready
		validMovementHexes = CalculateValidAdjacentMovementHexes();
		
		// Register unit position in the static dictionary
		if (unitLocations.ContainsKey(map.GetHex(this.coords)))
		{
			unitLocations[map.GetHex(this.coords)].Add(this);
		}
		else
		{
			unitLocations[map.GetHex(this.coords)] = new List<Unit> { this };
		}
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
	
	public void SetSelected()
	{
		if (!selected)
		{
			selected = true;
			Sprite2D sprite = GetNode<Sprite2D>("Sprite2D");
			Color c = new Color(sprite.Modulate);
			c.V = c.V - 0.25f;
			sprite.Modulate = c;
			validMovementHexes = CalculateValidAdjacentMovementHexes();
		}
	}
	
	public void SetDeselected()
	{
		selected = false;
		validMovementHexes.Clear();
		GetNode<Sprite2D>("Sprite2D").Modulate = civ.territoryColor;
	}
	
	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouse && mouse.ButtonMask == MouseButtonMask.Left)
		{
			var spaceState = GetWorld2D().DirectSpaceState;
			var point = new PhysicsPointQueryParameters2D();
			point.CollideWithAreas = true;
			point.Position = GetGlobalMousePosition();
			var result = spaceState.IntersectPoint(point);
			if (result.Count > 0 && (Area2D)result[0]["collider"] == collider)
			{
				EmitSignal(SignalName.UnitClicked, this);
				SetSelected();
				GetViewport().SetInputAsHandled();
			}
			else
			{
				SetDeselected();
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
	
	// Calculate valid adjacent movement hexes for this unit
	public List<Hex> CalculateValidAdjacentMovementHexes()
	{
		List<Hex> hexes = new List<Hex>();
		hexes.AddRange(map.GetSurroundingHexes(this.coords));
		hexes = hexes.Where(h => !impassible.Contains(h.terrainType)).ToList();
		return hexes;
	}
	
	// Move function with validation checks
	public void Move(Hex h)
	{
		if (selected && movePoints > 0)
		{
			if (validMovementHexes.Contains(h))
			{
				MoveToHex(h);
				EmitSignal(SignalName.UnitClicked, this);
			}
		}
	}
	
	// Move to hex function for actual movement logic
	public void MoveToHex(Hex h)
	{
		if (!unitLocations.ContainsKey(h) || (unitLocations.ContainsKey(h) && unitLocations[h].Count == 0))
		{
			unitLocations[map.GetHex(this.coords)].Remove(this);
			Position = map.MapToLocal(h.coordinates);
			coords = h.coordinates;
		}
	}
	
	// Load unit scene resources
	public static void LoadUnitScenes()
	{
		unitSceneResources = new Dictionary<Type, PackedScene>
		{
			{ typeof(Settler), ResourceLoader.Load<PackedScene>("res://Settler.tscn") },
			{ typeof(Warrior), ResourceLoader.Load<PackedScene>("res://Warrior.tscn") }
		};
		GD.Print("Unit scene resources loaded");
	}
	
	// Load unit UI image resources
	public static void LoadTextures()
	{
		uiImages = new Dictionary<Type, Texture2D>
		{
			{ typeof(Settler), (Texture2D)ResourceLoader.Load("res://textures/settler_image.png") },
			{ typeof(Warrior), (Texture2D)ResourceLoader.Load("res://textures/warrior_image.jpg") }
		};
		GD.Print("Unit UI image resources loaded");
	}
	
	// Set the civilization that owns this unit
	public void SetCiv(Civilization civ)
	{
		this.civ = civ;
		GetNode<Sprite2D>("Sprite2D").Modulate = civ.territoryColor;
		this.civ.units.Add(this);
		GD.Print($"Unit {unitName} assigned to civilization {civ.name}");
	}
}
