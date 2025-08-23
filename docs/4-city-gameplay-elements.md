This is a collection of ideas and experiment about implementing the city gameplay system

# 1. Creating and Initializing the City Scenes
- Let's create a 2D scene called "City" and 2 child nodes, one is a Sprite2D to contain the city's graphic image, the other is a Label which will represent the city's name. For the Sprite2D, we drag the city.png image into the texture slot and center the offset. Then we set the Label's default text to "city name" and edit other attributes like size or color to make it more visible. Lastly we attach a script to the city for future functionalities, including adding territories, populations and spawning units.

- Now let's in our city class store and initialize some fundamental datas:

using Godot;
using System;
using System.Collections.Generic;
public partial class City : Node2D
{
    // Main Map Coordinates
    public HexTileMap map;

    // City Center Coordinates
    public Vector2I centerCoordinates;

    // City's territory, list of hexes
    public List<Hex> territory;

    // List of tiles the city could expand to
    public List<Hex> borderTilePool;

    // City's name
    public string name;

    // City's subnode's references
    Label label;
    Sprite2D sprite;
}

- Let's not forget in the _Ready() function to append values to some of them to prevent initialization errors:

label = GetNode<Label>("Label");
sprite = GetNode<Sprite2D>("Sprite2D");
label.Text = name;
territory = new List<Hex>();
borderTilePool = new List<Hex>();

# 2. Creating Civilization Class
- Each civilization will have color and name, and control multiple cities. Technically, I don't think they need graphics or visual scenes for now, let's create only a script: Civilization.cs. We give it an identifier, cities, color, name, and an boolean checking if this is the player's city. 

using Godot;
using System;
using System.Collections.Generic;
public class Civilization
{
    public int id;
    public List<City> cities;
    public Color territoryColor;
    public string name;
    public bool playerCiv;

    public Civilization()
    {
        cities = new List<City>();
    }
}

# 3. Creating Cities in HexTileMap
- Let's create a function to add a city to the civilization, first create and import our packed scene:

PackedScene cityScene;
public override void _Ready()
{
    cityScene = ResourceLoader.Load<PackedScene>("City.tscn");
    // Other code to initialize scenes and etc...
}

- And a helper function taking in the civilization, city center coordinates and the city name:

public void CreateCity(Civilization civ, Vector2I coords, string name)
{
    City city = cityScene.Instantiate() as City;
}

- To make our CreateCity work, we need to set up references and addition operations in the involving classes:

- First let's add a civilization reference to our city class, because each city will be belonging to a civilization:

public Civilization civ;

- Inside HexTileMap, when a city is created, we can do this:

city.map = this; // inside city there's "public HexTileMap map;"

- Also in HexTileMap, we can then add the city to the civilization's cities list:

civ.cities.Add(city);

- Conversely, we assign the civilization to the city, but note each city would only have one civilization(at a time):

city.civ = civ;

- Lastly we add the city scene, to the scene tree and let it appear on the map:

AddChild(city);

# 4. Assigning relevent data to Cities

- Now these are the initial properties of the city, we will fill up some of these placeholders. First, the map coordinates of the city's center, so inside CreateCity:

city.centerCoordinates = coords;

// this is where to draw it, Position is the Node2D position property in unit of pixels, MapToLocal will transform the map coordinate to the world's pixel coordinates
// Mainly for drawing and label showing 
city.Position = baseLayer.MapToLocal(coords); 

- Let's create an AddTerritory function to add hexes to our city as territories, so inside our City class, it will take in a list of hexes as parameter:

public void AddTerritory(List<Hex> territoryToAdd)
{
    // each hex will point to the ownercity as this
    foreach (Hex h in territoryToAdd)
    {
        h.ownerCity = this;
    }

    // add to the list and will implemet AddRange
    territory.AddRange(territoryToAdd);
}

- Then in our Hex class, we will add and initialize the ownerCity variable

public City ownerCity; // New member variable
public Hex(Vector2I coords)
{
     //this.coordinates = coords;
     ownerCity = null; // Initialize ownerCity to null
}

- When we call the createCity, we also need to add the initial territory:

// Adding territory to the city
city.AddTerritory(new List<Hex>{mapData[coords]});

// Add the surrounding territory 

- Then to find the surrounding hexes, we'll have this code, GetSurroundingCells is TileMapLayer's method to return the neighoring cells, in our case of hex map there will be 6

public List<Hex> GetSurroundingHexes(Vector2I coords)
{
    List<Hex> result = new List<Hex>();
    foreach (Vector2I coord in baseLayer.GetSurroundingCells(coords))
    {
        // More code
    }
    return result;
}

- Before adding each surrounding hex, we will apply this function to check if it is within the map bounds:

public bool HexInBounds(Vector2I coords)
{
  if (coords.X < 0 || coords.X >= width ||
    coords.Y < 0 || coords.Y >= height)
    return false;
  
  return true;
}

- Now we will complete our GetSurroundingHexes function:

public List<Hex> GetSurroundingHexes(Vector2I coords)
{
    List<Hex> result = new List<Hex>();
    foreach (Vector2I coord in baseLayer.GetSurroundingCells(coords))
    {
        if (HexInBounds(coord))
            result.Add(mapData[coord]);
    }
    return result;
}

- Then we could simply populate the surrouding hexes, then we can iterate through them to check if they don't have an ownercity and is occupieable, if so we 

List<Hex> surroundingHexes = GetSurroundingHexes(city.centerCoordinates);

foreach (Hex hex in surroundingHexes)
{
    if (hex.ownerCity == null)
    {
        city.AddTerritory(new List<Hex> { hex });
    }
}

- Lastly we set the city name and icon color

public void SetCityName(string newName)
{
    name = newName;
    label.Text = newName;
}
public void SetIconColor(Color c)
{
    sprite.Modulate = c;
}

// In CreateCity
city.SetCityName(name);
city.SetIconColor(civ.territoryColor);

- In our HexTileMap we should keep track of the cities and instantiate them in Ready()

public Dictionary<Vector2I, City> cities;
public List<Civilization> civs;

// in Ready()
cities = new Dictionary<Vector2I, City>();
civs = new List<Civilization>();

- Then we add this to the Hex class, to check if a hex is a cityCenter

public bool isCityCenter = false;

- Lastly in our CreateCity function we set these values:

public void CreateCity(Civilization civ, Vector2I coords, string name)
{
    City city = cityScene.Instantiate() as City;
    city.map = this;
    civ.cities.Add(city);
    city.civ = civ;
    AddChild(city);

    // color + name
    city.SetIconColor(civ.territoryColor);
    city.SetCityName(name);

    // set the coordinates of the city
    city.centerCoordinates = coords;
    city.Position = baseLayer.MapToLocal(coords);
    mapData[coords].isCityCenter = true;

    // add territory to the city
    city.AddTerritory(new List<Hex>{mapData[coords]});
    List<Hex> surrounding = GetSurroundingHexes(coords);
    foreach (Hex h in surrounding)
    {
        if (h.ownerCity == null)
            city.AddTerritory(new List<Hex>{h});
    }

    UpdateCivTerritoryMap(civ);

    // add the city in the cities dictionary
    cities[coords] = city;
}

# City and Civilization Coloring

- In order to color the map base on the city and civilization, first we need to create a new TileMapLayer -> CivColorsLayer, and place it between BaseLayer and HexBordersLayer, we need to give it a TileSet, and set the Modulate Alpha to 60%(Half transparent so we can see the terrain color below)

- Let's add our civColorsLayer to load the resource.

baseLayer = GetNode<TileMapLayer>("BaseLayer");
borderLayer = GetNode<TileMapLayer>("HexBordersLayer");
overlayLayer = GetNode<TileMapLayer>("SelectionOverlayLayer");
civColorsLayer = GetNode<TileMapLayer>("CivColorsLayer");

- To paint the territories, we would create a function: UpdateCivTerritoryMap, to loop through the cities and hexes to paint them by setting the colors on their cells.

public void UpdateCivTerritoryMap(Civilization civ)
{
    foreach (City c in civ.cities)
    {
        foreach (Hex h in c.territory)
        {
            civColorsLayer.SetCell(h.coordinates, 0, terrainTextures[TerrainType.CIV_COLOR_BASE], civ.territoryColorAltTileId);
        }
    }
}

- Alternative tiles is a godot map feature that allows us to create variants of existing base tiles. We'll make use of this to create  alternative tiles for different civilizations' colors.

- First in our Civilization class we add a "public int territoryColorAltTileId;" to store its id.
