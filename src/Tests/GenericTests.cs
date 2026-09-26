using JsonVS;
using JsonVS.Models;

namespace Tests;

public class GenericTests
{
    private readonly string _json = 
        """
        {
            "elements": [ { "from": [ 6, 0, 6 ] } ]
        }
        """;
    
    [Fact]
    public void PathTest()
    {
        // Arrange
        var expectedPath = new ScalarPath("test") { Next = new ScalarPath("prop") { Next = new IndexPath(0)}};
        var pathString = "test/prop[0]";
            
        // Act
        var path = PathParser.Parse(pathString);
        
        // Assert
        Assert.Equal(expectedPath, path);
    }

    [Fact]
    public void ExistsTest()
    {
        // Arrange
        var obj = JsonLoader.Load(_json);
        var path = "elements[0]/from";
        
        // Act
        var result = obj.Exists(path);
        
        // Assert
        Assert.True(result);
    }
}