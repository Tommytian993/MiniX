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

# 2. Creating 