This is a collection of ideas and experiment about map design(not a formal design document, just a notebook about updates during the development)

# 1. TileMap Layers System Design
! Godot 4.3 TileMap had made changes, TileMap got updated to represent a series of "tilemaplayers"

TileMapLayers * 3
- Base Layer: color (and basically the terrain type)
- Hex Borders: borders on all hexagons.
- Selection Overlay: allows clicking, selection and color change

So base node to represent the map - Node2D “HexTileMap"
3 child nodes TileMapLayers - BaseLayer, HexBorders and SelectionOverlayLayer.

- Create Tileset for each layer, shape need to be changed to hexagon and tile size need to be 128 by 128 pixels

- Map width, heigth set to 100, 60 and as export public

# 2 Terrain Generation Function Set Up

Attach script to HexTileMap and create: public void GenerateTerrain() for all the potential generation logic. 

Declare 3 map layer variables: TileMapLayer baseLayer, borderLayer, overlayLayer; In the beginning of _Ready() function, get their reference by like "baseLayer = GetNode<TileMapLayer>("BaseLayer");"

Map Generation Loop: 
// X direction
for (int x = 0; x < width; x++)
{
  // Y direction
  for (int y = 0; y < height; y++)
  {
    // Terrain generation code
  }
}

- For each XY pairs we visit above, will call setCell function
- setCell has 3 parameters: first is xy tilemap coordinate, second tileset atlas id(default 0), third is coordinate of the texture on the atlas.
- Thus "baseLayer.SetCell(new Vector2I(x, y), 0, new Vector2I(0, 0));"
- To have low coupling we can seperate the assignments “borderLayer.SetCell(new Vector2I(x, y), 0, new Vector2I(0, 0));”

# 3 Camera Control Initialization

- Create a Camera2D node camera under game node
- Unlike most other nodes' perspectives in Godot, where the origin is at the top left, the camera’s origin is at the center.