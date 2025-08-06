This is a collection of ideas and experiment about creating the ui and the map interactivities in this games

# 1. Tile Selection and information print-out
- We first can override the ToString function and later call it after the selection - Hex class

public override string ToString()
{
    return $"Coordinates: ({this.coordinates.X}, {this.coordinates.Y}). Terrain type: {this.terrainType}";
}

- Add UnhandledInput override for detecting mouse clicks - HexTileMap

public override void _UnhandledInput(InputEvent @event)
{
    if (@event is InputEventMouseButton mouse)
    {
        // implement later
    }
}

- When we click on the world there will be a mouse coordinate on the screen, first we need to translate that to map coordianates. There will be 3 steps, first is getting that mouse coordinate, then converting it to local coordiantes and lastly converting it to map coordinates. 

- The 3 functions for the conversions are get_global_mouse_position, to_local, and local_to_map(), here we can use all of them on the same line:

Vector2I mapCoords = baseLayer.LocalToMap(ToLocal(GetGlobalMousePosition()));
GD.Print(mapData[mapCoords]); // then print out

- Now if we test this, there will be edges cases like clicking off the map and throwing errors. We need to address this by by limiting our mouse click detection to be within the bounds of the map. 

- To do that, first we need have a boundary check after the click, and before the print out:

if (mapCoords.X >= 0 && mapCoords.X < width && mapCoords.Y >= 0 && mapCoords.Y < height)
{
    // proceed to handle the click
}s