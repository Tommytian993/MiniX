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

// remove old highlight and highlight new one if new cell clicked, change currentSelectCell to store current coordinates.
if (mapCoords != currentSelectedCell)
{
overlayLayer.SetCell(currentSelectedCell, -1);
}
overlayLayer.SetCell(mapCoords, 0, new Vector2I(0, 1));
currentSelectedCell = mapCoords;

// in case selecting outside the map, just deselect the current tile
else
{
    overlayLayer.SetCell(currentSelectedCell, -1);
}

# 2. Resource Generation
- For now, each hex has terrain and coordinates, we should start adding resource attributes, first we can start with the 2 basic values, food and production. They will be defined as int values:

public int food;
public int production;

- We can override the toString to print them out along with coordinates and terrian: 

public override string ToString()
{
    return $"Coordinates: ({this.coordinates.X}, {this.coordinates.Y}). Terrain: {this.terrainType}). Food value: {this.food}. Production value: {this.production}";
}

- We want to set these resource values in our map generation loop in HexTileMap, let's define a function called generateResources, it will generate random numbers with boundaries based on the type of the hex's terrain: 

public void GenerateResources()
{
    Random r = new Random();

    for (int x = 0; x < width; x++)
    {
        for (int y = 0; y < height; y++)
        {
            Hex h = mapData[new Vector2I(x, y)];
            switch (h.terrainType)
            {
                case TerrainType.PLAINS:
                    h.food = r.Next(2, 6);
                    h.production = r.Next(0, 3);
                    break;
                case TerrainType.FOREST:
                    h.food = r.Next(1, 4);
                    h.production = r.Next(2, 6);
                    break;
                case TerrainType.DESERT:
                    h.food = r.Next(0, 2);
                    h.production = r.Next(0, 2);
                    break;
                case TerrainType.BEACH:
                    h.food = r.Next(0, 4);
                    h.production = r.Next(0, 2);
                    break;
            }
        }
    }
}

- We'll call this function in the _Ready right after the generateTerrain.

# 3. Terrain UI Creation
- We should create a new scene for the basic UI, use a "Panel" as the root node type, name it "TerrainTileUI". (Note that when we define games, per Godot naming conventions, sometimes it automatically converts the final "i" to capital letters)

- We'll need to expand it, change its dimension to 250 ** 2 pixels first.

- Add a "TextureRect" node under it, serve as terrain image placeholder, fix it size to match the width of the panel.

- Add 3 labels node under TextureRect: TerrainLabel, FoodLabel, and ProductionLabel.

# 4. Creating an UI Manager, and integrating the previous terrain UI to it

- Before adding the new UI Manager, we need to append a script to the current TerrainTileUI, this will be helpful for the manager later to access the functions and properties in there. 

using Godot;
public partial class TerrainTileUI : Panel
{
    // will add contents later
}

- To add a UI Manager, we can set a new canvas layer, it will be a seperate 2D drawing surface, to be placed above our map canvas.

- So set a new node under our game CanvasLayer, new child scene - UI Manager, and set it as a generic 2D node.

- So just to reiterate, we will have a lot of UI panels later, this UI Manager determines which UI panel should be on the screen at a given time. It will manage a group of packed scenes that represent different types of UI elements, and handle signals, that updates the UI.

- We'll have more packed scenes for UI later, for now, we can load into UI Manager the TerrainTileUI scene:

PackedScene terrainUiScene;
public override void _Ready()
{
    terrainUiScene = ResourceLoader.Load("TerrainTileUI.tscn");
}

- Then we can import and instantiate them any time if needed.

- We need to get reference to the terrainUI panel's components, which are TextureRect image and the 3 labels. We'll create them as member variables and get their references in the _Ready() function:

TextureRect terrainImage;
Label terrainLabel, foodLabel, productionLabel;
public override void _Ready()
{
    terrainLabel = GetNode<Label>("TerrainLabel");
    foodLabel = GetNode<Label>("FoodLabel");
    productionLabel = GetNode<Label>("ProductionLabel");
    terrainImage = GetNode<TextureRect>("TerrainImage");
}

- We then need an reference to the hex object we want to view its data, we create a new hex object, note this is just a pointer used by UI to reference the actual tile:

Hex h = null;

- As mentioned, our UIManager will held the specific references to UI elements:

TerrainTileUI terrainUi = null;

- Accordingly, we can create a set function:

public void SetTerrainUI(Hex h)
{
    # check and refresh current UI board
    if (terrainUi is not null) terrainUi.QueueFree();

    # instanitate a new scene as node 
    terrainUi = terrainUiScene.Instantiate() as TerrainTileUI;

    # add to UIManger (or canvas)?
    AddChild(terrainUi);
    terrainUi.SetHex(h);
}

# 5. Sending signals from HexTileMap to UI Manager

- With these pipeline set up finished, we can now propergate data down to TerrainTile UI. First let's store a reference in HexTileMap:

public partial class HexTileMap : Node2D
{
    // reference 
    UIManager uimanager;

    public override void _Ready()
    {
        // other code
        uimanager = GetNode<UIManager>("/root/Game/CanvasLayer/UiManager");
    }
}

- Now we can set up our signals, using the C# signal systems, with events and delegates. There is also a built-in event system in Godot. However since we are using "Hex" here, not a traditional node type, then it would be more appropriate to use the C# event system. (Technically the Godot's system is also built on top of that)

public partial class HexTileMap : Node2D
{
    // when clicked, suppose to send out a hex's data
    public delegate void SendHexDataEventHandler(Hex h);
    public event SendHexDataEventHandler SendHexData;
}

- After that, we can attach the signal and invoke it to our UI manager, in HexTileMap's on_ready function.

// after initial uimanager object, attach signal to this function
this.SendHexData += uimanager.SetTerrainUI;

- And we can invoke the event when its clicked within the bounds in UnhandledInput:

// within if (mapCoords.X >= 0 && mapCoords.X < width && mapCoords.Y >= 0 && mapCoords.Y < height)

SendHexData?.Invoke(h);

- Lastly we want to ensure the order of setHex and add node, we cannot reference the labels until terrain UI is added to the scene:

public void SetTerrainUI(Hex h)
{
  if (terrainUi is not null) terrainUi.QueueFree();
  terrainUi = terrainUiScene.Instantiate() as TerrainTileUI; 
  AddChild(terrainUi);
  terrainUi.SetHex(h);
}

# 6. Displaying food and production data
- To accomplish this, first we need to create dictionary mapping of terrain types to strings, so we can add the specfic text to label based on the type:

// static members are shared among all instances so it is good for enumerations and textures.

public static Dictionary<TerrainType, string> terrainTypeStrings = new Dictionary<TerrainType, string>
{
    { TerrainType.PLAINS, "Plains" },
    { TerrainType.BEACH, "Beach" },
    { TerrainType.DESERT, "Desert" },
    { TerrainType.MOUNTAIN, "Mountain" },
    { TerrainType.ICE, "Ice" },
    { TerrainType.WATER, "Water" },
    { TerrainType.SHALLOW_WATER, "Shallow Water" },
    { TerrainType.FOREST, "Forest" },
};

// usage
terrainLabel.Text = "Terrain: " + terrainTypeStrings[hex.terrainType];

- That's for the names, then we need to load the terrain textures or terrain images, by creating a dictionary mapping terrain types to Texture2D objects, and we'll load them at once when the game starts:

public static Dictionary<TerrainType, Texture2D> terrainTypeImages = new Dictionary<TerrainType, Texture2D>();
public static void LoadTerrainImages()
{
    Texture2D plains = ResourceLoader.Load("res://textures/plains.jpg") as Texture2D;
    Texture2D beach = ResourceLoader.Load("res://textures/beach.jpg") as Texture2D;
    Texture2D desert = ResourceLoader.Load("res://textures/desert.jpg") as Texture2D;
    Texture2D mountain = ResourceLoader.Load("res://textures/mountain.jpg") as Texture2D;
    Texture2D ice = ResourceLoader.Load("res://textures/ice.jpg") as Texture2D;
    Texture2D ocean = ResourceLoader.Load("res://textures/ocean.jpg") as Texture2D;
    Texture2D shallow = ResourceLoader.Load("res://textures/shallow.jpg") as Texture2D;
    Texture2D forest = ResourceLoader.Load("res://textures/forest.jpg") as Texture2D;
    terrainTypeImages = new Dictionary<TerrainType, Texture2D>
    {
        { TerrainType.PLAINS, plains },
        { TerrainType.BEACH, beach },
        { TerrainType.DESERT, desert },
        { TerrainType.MOUNTAIN, mountain },
        { TerrainType.ICE, ice },
        { TerrainType.WATER, ocean },
        { TerrainType.SHALLOW_WATER, shallow },
        { TerrainType.FOREST, forest },
    };
}

- We will call it in the overriden EnterTree() function, which will be executed right after we went into the game:

public override void _EnterTree()
{
    TerrainTileUI.LoadTerrainImages();
}

- Lastly in the SetHex we set the texture:

terrainImage.Texture = terrainTypeImages[hex.terrainType];