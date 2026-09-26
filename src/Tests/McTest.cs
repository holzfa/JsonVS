using JsonVS;
using JsonVS.Models;
using JsonObject = JsonVS.Models.JsonObject;

namespace Tests;

public class McTest
{
    private Dictionary<string, string> Variants = new()
    {
        { "powered=false", "minecraft:block/stone_pressure_plate" },
        { "powered=true",  "minecraft:block/stone_pressure_plate_down" },
    };

    [Fact]
    public void Test()
    {
        // Arrange
        var filePath = "stone_pressure_plate.json";
        
        // Act
        var obj = JsonLoader.LoadFile(filePath);
        var variants = obj.Get<JsonObject>("variants");

        Dictionary<string, string> results = new();
        foreach (var prop in variants.GetProperties<JsonObject>())
        {
            results[prop.Name] = prop.Value.Get<JsonString>("model").Value;
        }
        
        // Assert
        Assert.Equal(Variants, results);
    }
}