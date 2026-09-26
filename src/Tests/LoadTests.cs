using JsonVS;

namespace Tests;

public class LoadTests
{
    [Fact]
    public void LoadTest()
    {
        // Arrange
        var json = 
            """
            {
                "elements": [ { "from": [ 6, 0, 6 ] } ]
            }
            """;
        
        // Act
        var obj = JsonLoader.Load(json);
        var fromExists = obj.Exists("elements[0]/from");

        // Assert
        Assert.True(fromExists);
    }
    
    [Fact]
    public void LoadFileTest()
    {
        // Arrange
        
        // Act
        var obj = JsonLoader.LoadFromFile("stone_pressure_plate.json");
        var poweredExists = obj.Exists("variants/powered=true");

        // Assert
        Assert.True(poweredExists);
    }
}