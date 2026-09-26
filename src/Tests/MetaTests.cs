using JsonVS;
using JsonVS.Models;

namespace Tests;

public class MetaTests
{
    private string _json = 
        """
        {
          "blockId": 2,
          "textures": [
            "textures/grass/top.png",
            "textures/grass/side.png",
            "textures/dirt/base.png"
          ]
        }
        """;

    private record BlockModel(int Id, string[] TexturePaths) : IJsonMeta<BlockModel>
    {
        public static BlockModel Load(JsonObject obj)
        {
            var id = obj.Get<JsonInteger>("blockId").Get32();
            var textures = obj.Get<JsonArray>("textures").GetElements<JsonString>().Select(js => js.Value).ToArray();

            return new BlockModel(id, textures);
        }
    }
    
    [Fact]
    public void MetaLoadTest()
    {
        // Arrange
        var correct = new BlockModel(2, ["textures/grass/top.png", "textures/grass/side.png", "textures/dirt/base.png"]);
        
        // Act
        var model = JsonLoader.LoadMeta<BlockModel>(_json);
        
        // Assert
        Assert.Equivalent(correct, model);
    }
}