# JsonVS

[![.NET](https://img.shields.io/badge/.NET-10-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![Build and Test](https://img.shields.io/github/actions/workflow/status/holzfa/JsonVS/build-and-test.yml?label=build%20%26%20test)](https://github.com/yourusername/JsonVS/actions)
[![NuGet](https://img.shields.io/nuget/v/JsonVS.svg)](https://www.nuget.org/packages/JsonVS/)

JsonVS is a lightweight .NET 10 JSON navigation library that treats JSON documents like a file tree, letting you query nested objects, properties, and arrays using path syntax (e.g. `elements[0]/faces` or `elements/from[0]`) instead of writing nested object iteration by hand.

## Features

- Intuitive string-based path navigation for JSON documents
- Direct indexing into array elements inside paths (e.g. `elements[0]/from[1]`)
- Flexible API supporting indexing, standard typed getters, and safe `TryGet` patterns

## Example

Given the following JSON, modeled on a Minecraft fence post block definition:

```json
{
  "textures": {
    "texture": "blocks/fence_post",
    "particle": "blocks/fence_post"
  },
  "elements": [
    {
      "from": [6, 0, 6],
      "to": [10, 16, 10],
      "faces": {
        "north": { "uv": [6, 0, 10, 16], "texture": "#texture" },
        "east":  { "uv": [6, 0, 10, 16], "texture": "#texture" },
        "south": { "uv": [6, 0, 10, 16], "texture": "#texture" },
        "west":  { "uv": [6, 0, 10, 16], "texture": "#texture" }
      }
    }
  ]
}
```

You can query it directly with path syntax instead of manually walking the object graph:

```csharp
// Grab a nested object
JsonObject faces = jsonObject.Get<JsonObject>("elements[0]/faces");

// Fetch a specific value from inside an array
int fromX = jsonObject.Get<int>("elements[0]/from[0]");
```

## API Reference

Methods available on the `JsonObject` class.

### Path checking

- `Exists(string path)`
- `Exists(JsonPath path)`

### Retrieval

- `Get<T>(string path)`
- `Get<T>(JsonPath path)`
- `this[string path]`
- `this[JsonPath path]`

### Safe retrieval

- `TryGet(string path, out JsonValue value)`
- `TryGet(JsonPath path, out JsonValue value)`
- `TryGet<TValue>(string path, out TValue value)`
- `TryGet<TValue>(JsonPath path, out TValue value)`

## License

This project is distributed under the MIT License.
