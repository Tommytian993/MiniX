using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class HexTileMap : Node2D
{
    PackedScene cityScene;

    [Export]
    public int width = 100;

    [Export]
    public int height = 60;

    public Hex[,] hexes;

    TileMapLayer baseLayer,
        borderLayer,
        overlayLayer,
        civColorsLayer; // 三个图层引用

    // 地图数据字典，用于快速查找六边形数据
    Dictionary<Vector2I, Hex> mapData;

    // 地形纹理映射字典，将地形类型映射到对应的瓦片坐标
    Dictionary<TerrainType, Vector2I> terrainTextures;

    // 当前选中的单元格坐标，初始化为无效坐标 (-1, -1)
    Vector2I currentSelectedCell = new Vector2I(-1, -1);

    // UI管理器引用，用于与UI系统进行通信
    UIManager uiManager;

    public Dictionary<Vector2I, City> cities;
    public List<Civilization> civs;

    [Signal]
    public delegate void ClickOffMapEventHandler();

    // 六边形数据发送事件委托，用于松耦合的组件通信
    public delegate void SendHexDataEventHandler(Hex h);

    // 六边形数据发送事件，当选中六边形时触发
    public event SendHexDataEventHandler SendHexData;

    public override void _Ready()
    {
        cityScene = ResourceLoader.Load<PackedScene>("res://City.tscn");
        // 获取三个图层的引用
        baseLayer = GetNode<TileMapLayer>("BaseLayer");
        borderLayer = GetNode<TileMapLayer>("HexBordersLayer");
        overlayLayer = GetNode<TileMapLayer>("SelectionOverlayLayer");
        civColorsLayer = GetNode<TileMapLayer>("CivColorsLayer");
        // 获取UI管理器引用，使用绝对路径确保可靠性
        uiManager = GetNode<UIManager>("/root/Game/CanvasLayer/UiManager");

        // 调试：检查UI管理器是否正确获取
        if (uiManager == null)
        {
            GD.PrintErr("错误：无法找到UIManager节点！请检查场景结构。");
        }
        else
        {
            GD.Print("成功：UIManager节点已找到。");
        }

        // 初始化地图数据字典
        mapData = new Dictionary<Vector2I, Hex>();

        // 初始化地形纹理映射
        // 每个地形类型对应瓦片图集中的特定坐标
        terrainTextures = new Dictionary<TerrainType, Vector2I>()
        {
            { TerrainType.PLAINS, new Vector2I(0, 0) }, // 平原：第0行第0列
            { TerrainType.WATER, new Vector2I(1, 0) }, // 水域：第0行第1列,以此类推
            { TerrainType.DESERT, new Vector2I(0, 1) },
            { TerrainType.MOUNTAIN, new Vector2I(1, 1) },
            { TerrainType.SHALLOW_WATER, new Vector2I(1, 2) },
            { TerrainType.BEACH, new Vector2I(0, 2) },
            { TerrainType.FOREST, new Vector2I(1, 3) },
            { TerrainType.ICE, new Vector2I(0, 3) },
            { TerrainType.CIV_COLOR_BASE, new Vector2I(0, 3) },
        };

        GenerateTerrain();
        GenerateResources();

        civs = new List<Civilization>();
        cities = new Dictionary<Vector2I, City>();

        // 订阅六边形数据发送事件，将UI管理器的方法绑定到事件上
        // 当SendHexData事件触发时，自动调用uiManager.SetTerrainTileUi方法
        this.SendHexData += uiManager.SetTerrainUi;
    }

    public override void _Process(double delta) { }

    /// 处理未处理的输入事件，用于调试和交互
    public override void _UnhandledInput(InputEvent @event)
    {
        // 检查是否是鼠标按钮事件
        if (@event is InputEventMouseButton mouse)
        {
            // 将全局鼠标位置转换为地图坐标
            Vector2I mapCoords = baseLayer.LocalToMap(GetGlobalMousePosition());
            if (mapCoords.X >= 0 && mapCoords.X < width && mapCoords.Y >= 0 && mapCoords.Y < height)
            {
                if (mouse.ButtonMask == MouseButtonMask.Left)
                {
                    // 获取当前点击位置的六边形数据，使用局部变量提高代码可读性
                    Hex h = mapData[mapCoords];

                    // 打印该坐标位置的六边形信息，用于调试
                    GD.Print(h);

                    // 触发六边形数据发送事件，通知UI系统更新显示
                    // 使用空条件运算符(?.)确保事件有订阅者时才调用
                    GD.Print($"触发事件：准备发送六边形数据 - {h}");
                    SendHexData?.Invoke(h);
                    GD.Print("事件已触发");

                    // 如果点击的是新的单元格（不是当前已选中的）
                    if (mapCoords != currentSelectedCell)
                    {
                        // 清除之前选中单元格的叠加效果（设置为-1表示移除瓦片）
                        overlayLayer.SetCell(currentSelectedCell, -1);
                    }

                    // 在新选中的单元格位置设置叠加瓦片，提供视觉反馈
                    overlayLayer.SetCell(mapCoords, 0, new Vector2I(0, 1));

                    // 更新当前选中的单元格坐标
                    currentSelectedCell = mapCoords;
                }
            }
            else
            {
                // 如果点击在地图边界外，清除当前选中状态
                overlayLayer.SetCell(currentSelectedCell, -1);
                EmitSignal(SignalName.ClickOffMap);
            }
        }
    }

    /// 根据地形类型为每个六边形生成资源值
    public void GenerateResources()
    {
        // 创建随机数生成器，用于生成随机的资源值
        Random r = new Random();

        // 遍历整个地图网格，为每个六边形分配资源
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // 获取当前坐标的六边形对象
                Hex h = mapData[new Vector2I(x, y)];

                // 根据地形类型分配不同的资源值
                switch (h.terrainType)
                {
                    case TerrainType.PLAINS:
                        // 平原：适合农业，食物丰富，生产力一般
                        h.food = r.Next(2, 6); // 食物：2-5点
                        h.production = r.Next(0, 3); // 生产力：0-2点
                        break;
                    case TerrainType.FOREST:
                        // 森林：木材资源丰富，生产力高，食物较少
                        h.food = r.Next(1, 4); // 食物：1-3点
                        h.production = r.Next(2, 6); // 生产力：2-5点
                        break;
                    case TerrainType.DESERT:
                        // 沙漠：资源贫瘠，食物和生产力都很低
                        h.food = r.Next(0, 2); // 食物：0-1点
                        h.production = r.Next(0, 2); // 生产力：0-1点
                        break;
                    case TerrainType.BEACH:
                        // 海滩：有一定食物资源，生产力较低
                        h.food = r.Next(0, 4); // 食物：0-3点
                        h.production = r.Next(0, 2); // 生产力：0-1点
                        break;
                }
            }
        }
    }

    public void CreateCity(Civilization civ, Vector2I coords, string name)
    {
        City city = cityScene.Instantiate() as City;
        city.map = this;
        civ.cities.Add(city);
        city.civ = civ;

        AddChild(city);

        // color
        city.SetIconColor(civ.territoryColor);
        // name
        city.SetCityName(name);
        // center coordinates
        city.centerCoordinates = coords;
        city.Position = baseLayer.MapToLocal(coords);
        mapData[coords].isCityCenter = true;
        // add territory
        city.AddTerritory(new List<Hex> { mapData[coords] });

        // add surrounding territory
        List<Hex> surrounding = GetSurroundingHexes(coords);
        foreach (Hex h in surrounding)
        {
            if (h.ownerCity == null)
                city.AddTerritory(new List<Hex> { h });
        }
        UpdateCivTerritoryMap(civ);

        cities[coords] = city;
    }

    public void UpdateCivTerritoryMap(Civilization civ)
    {
        foreach (City c in civ.cities)
        {
            foreach (Hex h in c.territory)
            {
                civColorsLayer.SetCell(
                    h.coordinates,
                    0,
                    terrainTextures[TerrainType.CIV_COLOR_BASE],
                    civ.territoryColorAltTileId
                );
            }
        }
    }

    public List<Hex> GetSurroundingHexes(Vector2I coords)
    {
        List<Hex> result = new List<Hex>();
        foreach (Vector2I coord in baseLayer.GetSurroundingCells(coords))
        {
            if (HexInBounds(coord))
                result.Add(mapData[coord]);
        }
        return result;
    }

    public bool HexInBounds(Vector2I coords)
    {
        if (coords.X < 0 || coords.X >= width || coords.Y < 0 || coords.Y >= height)
            return false;

        return true;
    }

    public void GenerateTerrain()
    {
        // 创建多个噪声地图用于不同类型的地形生成
        float[,] noiseMap = new float[width, height]; // 基础地形噪声图
        float[,] forestMap = new float[width, height]; // 森林分布噪声图
        float[,] desertMap = new float[width, height]; // 沙漠分布噪声图
        float[,] mountainMap = new float[width, height]; // 山脉分布噪声图

        // 生成随机种子，确保每次生成的地图都不同
        Random r = new Random();
        int seed = r.Next(100000);

        // 创建噪声生成器实例
        FastNoiseLite noise = new FastNoiseLite();
        noise.Seed = seed; // 设置噪声种子
        noise.Frequency = 0.008f; // 设置噪声频率（控制地形变化速度）
        noise.FractalType = FastNoiseLite.FractalTypeEnum.Fbm; // 使用分形布朗运动
        noise.FractalOctaves = 4; // 设置分形八度（控制细节层次）
        noise.FractalLacunarity = 2.25f; // 设置分形间隙（控制频率变化）

        // 用于记录噪声的最大值，用于后续归一化
        float noiseMax = 0f;

        // 创建森林噪声生成器，使用不同的噪声类型和参数
        FastNoiseLite forestNoise = new FastNoiseLite();

        // 配置森林噪声参数
        forestNoise.NoiseType = FastNoiseLite.NoiseTypeEnum.Cellular; // 使用细胞噪声，产生更自然的森林分布
        forestNoise.Seed = seed; // 使用相同的种子，确保一致性
        forestNoise.Frequency = 0.04f; // 较高的频率，产生更密集的森林分布
        forestNoise.FractalType = FastNoiseLite.FractalTypeEnum.Fbm; // 使用分形布朗运动
        forestNoise.FractalLacunarity = 2f; // 标准间隙值，产生均匀的细节分布
        float forestNoiseMax = 0f;

        // 创建沙漠噪声生成器，使用不同的噪声类型和参数
        FastNoiseLite desertNoise = new FastNoiseLite();

        // 配置沙漠噪声参数
        desertNoise.NoiseType = FastNoiseLite.NoiseTypeEnum.SimplexSmooth; // 使用平滑单形噪声，产生广阔的沙漠区域
        desertNoise.Seed = seed; // 使用相同的种子，确保一致性
        desertNoise.Frequency = 0.015f; // 中等频率，产生适中的沙漠分布
        desertNoise.FractalType = FastNoiseLite.FractalTypeEnum.Fbm; // 使用分形布朗运动
        desertNoise.FractalLacunarity = 2f; // 标准间隙值，产生均匀的细节分布

        // 用于记录沙漠噪声的最大值
        float desertNoiseMax = 0f;

        // 创建山脉噪声生成器，使用不同的噪声类型和参数
        FastNoiseLite mountainNoise = new FastNoiseLite();
        mountainNoise.NoiseType = FastNoiseLite.NoiseTypeEnum.Simplex; // 使用单形噪声，适合生成山脉的起伏感
        mountainNoise.Seed = seed; // 使用相同的种子，确保一致性
        mountainNoise.Frequency = 0.02f; // 较高频率，产生较多的山脉细节
        mountainNoise.FractalType = FastNoiseLite.FractalTypeEnum.Ridged; // 使用脊状分形，生成尖锐的山脉效果
        mountainNoise.FractalLacunarity = 2f; // 标准间隙值，产生均匀的细节分布
        float mountainNoiseMax = 0f;

        // 第一遍遍历：生成所有噪声地图并找到最大值
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // 生成基础地形噪声并记录最大值
                noiseMap[x, y] = Mathf.Abs(noise.GetNoise2D(x, y));
                if (noiseMap[x, y] > noiseMax)
                    noiseMax = noiseMap[x, y];

                // 生成沙漠噪声并记录最大值
                desertMap[x, y] = Math.Abs(desertNoise.GetNoise2D(x, y));
                if (desertMap[x, y] > desertNoiseMax)
                    desertNoiseMax = desertMap[x, y];

                // 生成森林噪声并记录最大值
                forestMap[x, y] = Math.Abs(forestNoise.GetNoise2D(x, y));
                if (forestMap[x, y] > forestNoiseMax)
                    forestNoiseMax = forestMap[x, y];

                // Mountain
                mountainMap[x, y] = Math.Abs(mountainNoise.GetNoise2D(x, y));
                if (mountainMap[x, y] > mountainNoiseMax)
                    mountainNoiseMax = mountainMap[x, y];
            }
        }

        // 定义地形生成规则：根据噪声值范围确定地形类型
        // 每个元组包含：(最小噪声值, 最大噪声值, 对应的地形类型)
        List<(float Min, float Max, TerrainType type)> terrainGenValues = new List<(
            float Min,
            float Max,
            TerrainType type
        )>()
        {
            // 水域：噪声值最低的区域（0 到 25% 的最大值）
            (0, noiseMax / 10 * 2.5f, TerrainType.WATER),
            // 浅水：噪声值较低的区域（25% 到 40% 的最大值）
            (noiseMax / 10 * 2.5f, noiseMax / 10 * 4, TerrainType.SHALLOW_WATER),
            // 海滩：噪声值中等的区域（40% 到 45% 的最大值）
            (noiseMax / 10 * 4, noiseMax / 10 * 4.5f, TerrainType.BEACH),
            // 平原：噪声值较高的区域（45% 到最大值）
            (noiseMax / 10 * 4.5f, noiseMax + 0.05f, TerrainType.PLAINS),
        };

        // 森林生成阈值：只有森林噪声值大于该范围才会生成森林
        Vector2 forestGenValues = new Vector2(forestNoiseMax / 10 * 7, forestNoiseMax + 0.05f);
        // 沙漠生成阈值：只有沙漠噪声值大于该范围才会生成沙漠
        Vector2 desertGenValues = new Vector2(desertNoiseMax / 10 * 6, desertNoiseMax + 0.05f);
        // 山脉生成阈值：只有山脉噪声值大于该范围才会生成山脉
        Vector2 mountainGenValues = new Vector2(
            mountainNoiseMax / 10 * 5.5f,
            mountainNoiseMax + 0.05f
        );

        // 遍历整个地图网格
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // 创建新的六边形对象，设置其坐标位置
                Hex h = new Hex(new Vector2I(x, y));
                // 获取当前位置的噪声值
                float noiseValue = noiseMap[x, y];
                // 根据噪声值确定地形类型，使用地形生成规则
                h.terrainType = terrainGenValues
                    .First(range => noiseValue >= range.Min && noiseValue <= range.Max)
                    .type;
                // 将六边形数据存储到地图数据字典中
                mapData[new Vector2I(x, y)] = h;

                if (
                    desertMap[x, y] >= desertGenValues[0]
                    && desertMap[x, y] <= desertGenValues[1]
                    && h.terrainType == TerrainType.PLAINS
                )
                {
                    h.terrainType = TerrainType.DESERT;
                }

                if (
                    forestMap[x, y] >= forestGenValues[0]
                    && forestMap[x, y] <= forestGenValues[1]
                    && h.terrainType == TerrainType.PLAINS
                )
                {
                    h.terrainType = TerrainType.FOREST;
                }

                if (
                    mountainMap[x, y] >= mountainGenValues[0]
                    && mountainMap[x, y] <= mountainGenValues[1]
                    && h.terrainType == TerrainType.PLAINS
                )
                {
                    h.terrainType = TerrainType.MOUNTAIN;
                }

                // 设置基础图层瓦片，使用对应地形的纹理坐标
                baseLayer.SetCell(new Vector2I(x, y), 0, terrainTextures[h.terrainType]);
                // 设置边框图层瓦片
                borderLayer.SetCell(new Vector2I(x, y), 0, new Vector2I(0, 0));
            }
        }

        // 先生成所有基础和叠加地形，最后再叠加极地冰盖，确保极地不会被其他地形覆盖
        int maxIce = 5;
        for (int x = 0; x < width; x++)
        {
            // 顶部冰原北极：在每一列的最上方随机生成1~maxIce行冰原
            // 这样可以模拟北极圈的冰盖效果，每一列的冰原厚度是随机的，增加自然感
            for (int y = 0; y < r.Next(maxIce) + 1; y++)
            {
                Hex h = mapData[new Vector2I(x, y)];
                h.terrainType = TerrainType.ICE; // 强制设置为冰原
                baseLayer.SetCell(new Vector2I(x, y), 0, terrainTextures[h.terrainType]);
            }
            // 底部冰原南极：在每一列的最下方随机生成1~maxIce行冰原
            // 同理，模拟南极圈的冰盖效果，保证南北两极都有冰原分布
            for (int y = height - 1; y > height - 1 - r.Next(maxIce) - 1; y--)
            {
                Hex h = mapData[new Vector2I(x, y)];
                h.terrainType = TerrainType.ICE; // 强制设置为冰原
                baseLayer.SetCell(new Vector2I(x, y), 0, terrainTextures[h.terrainType]);
            }
        }
    }

    /// <summary>
    /// 将地图坐标转换为本地世界坐标
    /// </summary>
    /// <param name="coords">地图坐标 (x, y)</param>
    /// <returns>对应的本地世界坐标</returns>
    public Vector2 MapToLocal(Vector2I coords)
    {
        // 使用基础图层的坐标转换方法
        return baseLayer.MapToLocal(coords);
    }
}
