This is a collection of ideas and experiment about creating the procedural terrain generation in this game(not a formal design document, just a notebook about updates during the development)

# 1. Terrain type Representations
- First we can use an enum structure to represent all the available types:

public enum TerrainType { PLAINS, WATER, DESERT, MOUNTAIN, ICE, SHALLOW_WATER, FOREST, BEACH }

- Then we create the Hex class, which will encapsulate all the data about a single hex tile, for now we have its coordinates and terraintype as follows:

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

