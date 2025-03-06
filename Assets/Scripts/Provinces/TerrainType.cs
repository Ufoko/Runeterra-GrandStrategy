using System.Collections.Generic;

public struct TerrainType
{
    private static List<TerrainType> types = new List<TerrainType>();

    public static readonly TerrainType grassLands = new TerrainType("Grass lands", 1f, 1f);
    public static readonly TerrainType mountain = new TerrainType("Mountain", 2f, 0.5f);
    public static readonly TerrainType desert = new TerrainType("Desert", 1.2f, 0.5f);
    public static readonly TerrainType frozen = new TerrainType("Frozen", 3f, 0.2f);
    public static readonly TerrainType water = new TerrainType("Water",1f,1000f);

    private int index;
    public string name;
    public float defenseMultiplier;
    public float speedMultiplier;

    private TerrainType(string name, float defenseMultiplier, float speedMultiplier)
    {
        this.name = name;
        this.defenseMultiplier = defenseMultiplier;
        this.speedMultiplier = speedMultiplier;

        index = types.Count;
        types.Add(this);
    }

    public static implicit operator int(TerrainType type)
    {
        return type.index;
    }
}
