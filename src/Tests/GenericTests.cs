using JsonVS;

namespace Tests;

public class GenericTests
{
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
}