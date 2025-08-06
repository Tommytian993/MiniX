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
}

- Now another edge case might be right-clicking on the tile, that would also be allowed to happen with our current "@event is InputEventMouseButton mouse", we can fix this by adding another check and querying the button mask of the click:

if (mouse.ButtonMask == MouseButtonMask.Left)
{
    // proceed to handle the click
}

- After handling the edge cases, we need to add a visual feedback when clicking on the tile, we'll set the current cell to a overlay layer for highlight:

overlayLayer.SetCell(mapCoordinates, 0, new Vector2I(0, 1));

- Now we need to create the feature to deselect a cell after clicking, we need to first initialize a vector2I to hold current clicked cell coordinates:

Vector2I currentSelectedCell = new Vector2I(-1, -1);

- Then inside the final check and after printing out the tile coordinates, we need to 

GD.Print(mapData[mapCoords]);

// remove old highlight and highlight new one if new cell clicked, change currentSelectCell to store current coordinates.s
if (mapCoords != currentSelectedCell)
{
overlayLayer.SetCell(currentSelectedCell, -1);
}
overlayLayer.SetCell(mapCoords, 0, new Vector2I(0, 1));
currentSelectedCell = mapCoords;
