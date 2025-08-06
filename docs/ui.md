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