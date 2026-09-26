namespace JsonVS.Models;

public interface IJsonMeta<out TMeta> where TMeta : IJsonMeta<TMeta>
{
    public static abstract TMeta Load(JsonObject obj);
}