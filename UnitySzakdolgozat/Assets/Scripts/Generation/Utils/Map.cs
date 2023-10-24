using Utils;

public class Map
{
    public int Size { get; private set; }
    private FieldType[,] fieldTypes;
    private bool[,] items;

    public Map(int size) {
        Size = size;
        fieldTypes = new FieldType[size, size];
        items = new bool[size, size];
    }
    
    
    public FieldType this[float x, float y] {
        get => fieldTypes[(int) x, (int) y];
        set => fieldTypes[(int)x, (int)y] = value;
    }
    
    public FieldType this[int x, int y] {
        get => fieldTypes[x, y];
        set => fieldTypes[x, y] = value;
    }
}
