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
city.Position = baseLayer.MapToLocal(coords); 
