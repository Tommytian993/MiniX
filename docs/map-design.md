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

To control the map, we should set up an input map
Map Left: A key
Map Right: D key
Map Up: W key
Map Down: S key
Zoom In (Keyboard): =/+ key
Zoom Out (Keyboard): - key
Zoom In (Mouse): Mouse Wheel Up
Zoom Out (Mouse): Mouse Wheel Down

# 4 Camera Script

- We can attach a script to our Camera as camera.cs, in it first we define the moving and zooming speed 

[Export]
int velocity = 15;
[Export]
float zoom_speed = 0.05f;

- Then we can create our Camera panning feature in a new: public override void _PhysicsProcess(double delta). We use physics process here instead of the default process because it is called with more stable tick rates, thus allowing more smooth camera movements.

- We can then poll our input events, and change positions accordingly.

if (Input.IsActionPressed("map_right"))
{
  this.Position += new Vector2(velocity, 0);
}

- We can change the velocity's value and direction here to make other movements, just need to keep in mind that the up direction is negative on the y-axis in this system.

- Similar to panning, we can use the zoom property in Camera2D to toggle zoom, to prevent zooming in to close or too far, we can have boundary checks before execution.

if (Input.IsActionPressed("map_zoom_in")) {
    if (this.Zoom < new Vector2(3f, 3f))
        this.Zoom += new Vector2(zoom_speed, zoom_speed);
}
if (Input.IsActionPressed("map_zoom_out")) {
    if (this.Zoom > new Vector2(0.1f, 0.1f))
        this.Zoom -= new Vector2(zoom_speed, zoom_speed);
}

- Besides holding the plus and minus keys for zooming, we can also add mouse scroll zooming.
We'll initialize these boolean variables:

bool mouseWheelScrollingUp = false;
bool mouseWheelScrollingDown = false;

- Then use IsActionJustReleased function to check and give values to them:

if (Input.IsActionJustReleased("mouseZoomOut"))
    mouseWheelScrollingDown = true;
if (!Input.IsActionJustReleased("mouseZoomOut"))
    mouseWheelScrollingDown = false;

- Finally we can add them in our existing zoom control loop:

if (Input.IsActionPressed("map_zoom_out") || mouseWheelScrollingDown)
    // Zoom out code here

# 5 Setting Boundaries for Camera Panning

- We need to stop camera movement at the the boundary of the map, first we can set up 4 variables:

float leftBound, rightBound, topBound, bottomBound;

- We need to assign values to them base on our state of the map, so we need a map reference variable as well

! Please make sure you declare HexTileMap map outside the _Ready() function to make it a global variable

public partial class Camera : Camera2D
{
  // Map boundaries
  float leftBound, rightBound, topBound, bottomBound;

  // Map reference
  HexTileMap map;
  public override void _Ready()
  {
    map = GetNode<HexTileMap>("../HexTileMap");
    // ...
  }
}
