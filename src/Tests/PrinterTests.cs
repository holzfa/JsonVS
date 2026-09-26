using JsonVS.Lexer;
using JsonVS.Parser;

namespace Tests;

public class PrinterTests
{
    [Fact]
    public void PrintTest()
    {
        // Arrange
        var content = File.ReadAllText("stone_pressure_plate.json");
        
        // Act
        var tokens = new Tokenizer().Tokenize(content);
        var obj = new JsonParser().Parse(tokens);
        var text = new JsonPrinter().Run(obj);
        
        // Assert
        Assert.Equal(content, text);
    }
}