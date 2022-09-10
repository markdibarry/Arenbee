namespace GameCore.Items;

public class ItemCategoryBase
{
    public ItemCategoryBase(string id, string name, string abbreviation)
    {
        Id = id;
        Name = name;
        Abbreviation = abbreviation;
    }

    public string Id { get; set; }
    public string Name { get; set; }
    public string Abbreviation { get; set; }
}
