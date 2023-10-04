using Utils;

public class Map
{
    private int size;
    private FieldType[,] fieldTypes;
    private bool[,] items;

    public Map(int size) {
        this.size = size;
        fieldTypes = new FieldType[size, size];
        items = new bool[size, size];
    }

    public int Size {
        get => size;
        set => size = value;
    }
    
    public FieldType this[float x, float y] {
        get => fieldTypes[(int) x, (int) y];
        set => fieldTypes[(int)x, (int)y] = value;
    }
    
    public FieldType this[int x, int y] {
        get => fieldTypes[x, y];
        set => fieldTypes[x, y] = value;
    }

    public void SetItem(int x, int y) {
        items[x, y] = true;
    }

    public bool HasItem(int x, int y) {
        return items[x, y];
    }
}
