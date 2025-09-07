This is a collection of ideas and experiment about implementing the turn and city expansion system

# 1. Creating GeneralUI Scene
- To create a turn system, we create a new scene Panel, and call it GeneralUI. We add a Label to denote the turn number and a Button that we can press and end the turn.

- We attach a script, include initial variables, references and the basic turn-counter function

using Godot;
using System;
public partial class GeneralUI : Panel
{
    int turns = 0;
    Label turnLabel;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        turnLabel = GetNode<label>("TurnLabel");
        turnLabel.Text = "Turn: " + turns;
    }
    public void IncrementTurnCounter()
    {
        turns += 1;
        turnLabel.Text = "Turn: " + turns;
    }
}

# 2. Integrating GeneralUI to UIManager

- CityUI, TerrainUI are ephemeral, GeneralUi should be persistent. Thus we instantiate it as a child scene under UIManger. And it will display throughout the game.

- In UIManager script, we do not need to import a packed scene since we already made an instance. We still need to get the reference like this:

GeneralUI generalUI = GetNode<GeneralUI>("GeneralUI");

- Let's create the end turn signal in UIManager:

[Signal]
public delegate void EndTurnEventHandler();

- We get the button reference from GeneralUI:

Button endTurnButton = generalUI.GetNode<Button>("EndTurnButton");

- An intermediate function needs to be created to handle the button signal and emit the end turn signal:

public void SignalEndTurn()
{
    EmitSignal(SignalName.EndTurn);
    generalUI.IncrementTurnCounter();
}

endTurnButton.Pressed += SignalEndTurn;

- Let's finally create this skeletal reaction function in HexTileMap and call UIManager's instance on its endTurn function to test thsi process.

public void ProcessTurn()
{
    GD.Print("Turn ended");
}

// _Ready() of HexTileMap
UIManager uiManager = GetNode<UIManager>("/root/Game/CanvasLayer/UiManager");
uiManager.EndTurn += ProcessTurn;

# 3. City Population Growth

- Our city have food and production values, as well as a population that starts with one. The mechanic is designed to be: as food value increase, population and city tiles will expand.

- To know the potential tiles to add to the city, we will define this list:

public List<Hex> borderTilePool;
public override void _Ready()
{
    borderTilePool = new List<Hex>();
}

- We define population growth related values:

public static int POPULATION_THRESHOLD_INCREASE = 15;
public int populationGrowthThreshold;
public int populationGrowthTracker;

- Now each turn, the tracker would add the food value and check against the thredhold:

public void ProcessTurn()
{
    populationGrowthTracker += totalFood;
    if (populationGrowthTracker > populationGrowthThreshold) // Grow population
    {
        population++;
        populationGrowthTracker = 0;
        populationGrowthThreshold += POPULATION_THRESHOLD_INCREASE;
    }
}

- We need to define this invalid_tiles dictionary to prevent city overlaps:

public static Dictionary<Hex, City> invalidTiles = new Dictionary<Hex, City>();

- Now we should modify AddTerritory to add the new territoriy's surrounding tiles to the border pool.

public void AddTerritory(List<Hex> territoryToAdd)
{
    foreach (Hex h in territoryToAdd)
    {
        h.ownerCity = this;
        // Add new border hexes to the border tile pool
        AddValidNeighborsToBorderPool(h);
    }
    territory.AddRange(territoryToAdd);
    CalculateTerritoryResourceTotals();
}

- To check if the tile could be valid, here is the function, terrain whether occupied by other cities, whether pooled by other cities will all need to be considered

public bool IsValidNeighborTile(Hex n)
{
    if (n.terrainType == TerrainType.WATER ||
        n.terrainType == TerrainType.ICE ||
        n.terrainType == TerrainType.SHALLOW_WATER ||
        n.terrainType == TerrainType.MOUNTAIN)
    {
        return false;
    }
    if (n.ownerCity != null && n.ownerCity.civ != null)
    {
        return false;
    }
    if (invalidTiles.ContainsKey(n) && invalidTiles[n] != this)
    {
        return false;
    }
    return true;
}