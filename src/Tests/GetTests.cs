using JsonVS;

namespace Tests;

public class GetTests
{
    private readonly string Json = 
        """
        {
            "elements": [ { "from": [ 6, 0, 6 ] } ]
        }
        """;

    
    [Fact]
    public void UnsafeGetTest()
    {
        // Arrange
        var obj = JsonLoader.Load(Json);
        var path = "elements[0]/from[0]";
        
        // Act
        var dumbResult = obj.Get(path);
        var indexResult = obj[path];
        var smartResult = obj.Get<JsonInteger>(path);
        
        
        // Assert
        if (dumbResult is JsonInteger ji)
        {
            Assert.Equal(6, ji.Value);
        } else Assert.Fail();
        
        if (indexResult is JsonInteger ii)
        {
            Assert.Equal(6, ii.Value);
        } else Assert.Fail();
        
        Assert.Equal(6, smartResult.Value);
    }
    
    [Fact]
    public void SafeGetTest()
    {
        // Arrange
        var obj = JsonLoader.Load(Json);
        var path = "elements[0]/from[0]";
        
        // Act
        var dumbResult = obj.TryGet(path, out var dumbValue);
        var smartResult = obj.TryGet(path, out JsonInteger? smartValue);
        
        
        // Assert
        if (dumbResult && dumbValue is JsonInteger ji)
        {
            Assert.Equal(6, ji.Value);
        } else Assert.Fail();
        
        if (smartResult)
        {
            // SmartValue is guaranteed to not be null here because smartResult is true
            Assert.Equal(6, smartValue!.Value);
        } else Assert.Fail();
    }
}