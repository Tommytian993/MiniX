This is a collection of ideas and experiment about creating the procedural terrain generation in this game(not a formal design document, just a notebook about updates during the development)

# 1. Terrain Type Representations
- First we can use an enum structure to represent all the available types:

public enum TerrainType { PLAINS, WATER, DESERT, MOUNTAIN, ICE, SHALLOW_WATER, FOREST, BEACH }

- We can create the Hex class, which will encapsulate all the data about a single hex tile, for now we have its coordinates and terraintype as follows:

public class Hex
{
    // This is readonly since it won't change throught the game 
    public readonly Vector2I coordinates; 
    public TerrainType terrainType;
    public Hex(Vector2I coords)
    {
        this.coordinates = coords;
    }
}

- Then back to our HexTileMap, we'll create a dictionary to store grid coordinates and map it to each Hex objects:

Dictionary<Vector2I, Hex> mapData;

public override void _Ready()
{
    mapData = new Dictionary<Vector2I, Hex>();
}

# 2. Terrain Types and Texture Atlas
- For our game at this time for simplicity, we have colored tiles as texture atlas for the terrain types. To map them, we create a dictionary similar to the coordiante-type dictionary:

Dictionary<TerrainType, Vector2I> terrainTextures;

- In the ready function, we'll fill in the key-value pairs, type enums with its corresponding texture atlas coordinates.

{ TerrainType.PLAINS, new Vector2I(0, 0) },
{ TerrainType.WATER, new Vector2I(1, 0)}
{ TerrainType.DESERT, new Vector2I(0, 1)},
{ TerrainType.MOUNTAIN, new Vector2I(1, 1)},
{ TerrainType.SHALLOW_WATER, new Vector2I(1, 2)},
{ TerrainType.BEACH, new Vector2I(0, 2)},
{ TerrainType.FOREST, new Vector2I(1, 3)},
{ TerrainType.ICE, new Vector2I(0, 3)},

# 3. FastNoiseLite
- To proceduraly generate our terrains, we can make use of Godot’s built-in noise generator, FastNoiseLite. We can create an instance like this:

FastNoiseLite noise = new FastNoiseLite();

- There are several parameters inside the FastNoiseLite that will determine the nature of the generated noise:

Noise Type: Algorithms including Perlin, Cellular, Value, and Simplex and etc. The could be the foundation of the noise's type and pattern.
