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

# 3. FastNoiseLite Set Up
- To proceduraly generate our terrains, we can make use of Godot’s built-in noise generator, FastNoiseLite. We can create an instance like this:

FastNoiseLite noise = new FastNoiseLite();

- There are several parameters inside the FastNoiseLite that will determine the nature of the generated noise:

Noise Type: Algorithms including Perlin, Cellular, Value, and Simplex and etc. The could be the foundation of the noise's type and pattern.

Seed: We probably heard of this when playing minecraft. Depend on the algorithm, usually the same seed will produce the same pattern for an algorithm. For our generation now we'll set it to be random.

Frequency: This affects the density of the noise pattern, higher value = denser pattern.

Fractal Options: This also mainly affect the appearance of the noise, I need to do more research but so far I think it affect properties like blurriness, lacunaity, edges of the noise pattern.

- Here is an example how we can assign values to them:

noise.Seed = seed; 
noise.Frequency = 0.009f; 
noise.FractalType = FastNoiseLite.FractalTypeEnum.Fbm; 
noise.FractalOctaves = 10; 
noise.FractalLacunarity = 3.15f; 

- We'll first set up 4 runs for noise, the first is ocean and continents, the others are what their name suggests:

float[,] NoiseMap = new float[mapWidth, mapHeight];
float[,] ForestMap = new float[mapWidth, mapHeight];
float[,] DesertMap = new float[mapWidth, mapHeight];
float[,] MountainsMap = new float[mapWidth, mapHeight];

- Then we created relevent objects for generation, random function and the seed for each generation

Random random = new Random();
int seed = random.Next(100000);

- Just to reiterate, we use FastNoiseLite object and it's parameters to produce a unique noise as a return value, and we would need a variable called noiseMax, to track the max value which will be used as border seperating different terrains.

float noiseMax = 0f;

# 3. Start noise generation and storage
- To store each noise value in the noise map inside the loops, we make use of GetNoise2D function and pass in the pixel cooridante.

noiseMap[x, y] = Math.Abs(noise.GetNoise2D(x, y));

- We need a max noise value for each terrain type, we can simply via the following code track and update this value:

if (noiseMap[x, y] > noiseMax) {
  noiseMax = noiseMap[x ,y];
}

- At this point we arrived at the final step - Defining Terrain Ratios, this means each type will have a min and max noise value that uniquely enclose them:

List<(float Min, float Max, TerrainType Type)> terrainGenValues = new List<(float Min, float Max, TerrainType Type)>{
  (0, noiseMax/10 * 2.5f, TerrainType.WATER),
  (noiseMax/10 * 2.5f, noiseMax/10 * 4, TerrainType.SHALLOW_WATER),
  (noiseMax/10 * 4, noiseMax/10 * 4.5f, TerrainType.BEACH),
  (noiseMax/10 * 4.5f, noiseMax + 0.05f, TerrainType.PLAINS)
};

# 4. Add the terrain hexagons to the map
- Here we are at the final step for ocean and land terrain generation, with all the noise values in hand, we will create terrain hexagons and assign to the map with these values. Let's go to our main loop where we set the cells, there are five steps to complete. 

- 1. Create a new hex at the current position in the map
Hex hex = new Hex(new Vector2I(x, y));

- 2. Get noise value from dict, at the same current location
float noiseValue = noiseMap[x, y];

- 3. The third step is setting the hex.terrain member variable. To help with our assignment, we can use a feature in C# called "Language Integrated Query" (LINQ), this helps us query data in a more readable and concise way, we'll use the First keyword here and it will return the first element satisfying the criterias. The criterias we apply here is the noise value boundaries. In the end, we add the .TYPE to set the range to be our terrain type.

hex.terrainType = terrainGenValues.First(range => noiseValue >= range.Min  &&       noiseValue < range.Max).Type;

- 4. We add the hex to mapData
mapData[new Vector2I(x, y)] = hex;

- 5. Lastly we set the cells in tilemap, remember we created the terrainTextures dictionary that contains the mapping of terrain to texture atlas coordinates. So we set the cell with that atlas texture.
baseLayer.SetCell(new Vector2I(x, y), 0, terrainTextures[hex.terrainType]);

# 5. Forest and Desert Terrains
- For other additional terrains, we just need to generate new noise values and determine boundaries.

- For the forest FastNoiseLite initiation, we can use OpenSimplex to create continous and natural landscape, slightly increase octaves details.

FastNoiseLite forestNoise = new FastNoiseLite();
forestNoise.NoiseType = FastNoiseLite.NoiseTypeEnum.OpenSimplex2;
forestNoise.Seed = seed;
forestNoise.Frequency = 0.04f;
forestNoise.FractalType = FastNoiseLite.FractalTypeEnum.Fbm; 
forestNoise.FractalOctaves = 5; 
forestNoise.FractalLacunarity = 2.0f; 
forestNoise.FractalGain = 0.5f;

- For the desert FastNoiseLite initiation, we can use SimplexSmooth for a continous but not drastically changing terrain. 

FastNoiseLite desertNoise = new FastNoiseLite();
desertNoise.NoiseType = FastNoiseLite.NoiseTypeEnum.SimplexSmooth;
desertNoise.Seed = seed;
desertNoise.Frequency = 0.015f;
desertNoise.FractalType = FastNoiseLite.FractalTypeEnum.Fbm;
desertNoise.FractalOctaves = 4;
desertNoise.FractalLacunarity = 2.0f;
desertNoise.FractalGain = 0.45f;