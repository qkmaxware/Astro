using System.Text.Json;

namespace Qkmaxware.Astro {

public interface ISerializable {}

public static class Serializer {

    public static string ToJson<T>(this T obj) where T:ISerializable {
        return JsonSerializer.Serialize(obj, new JsonSerializerOptions{
            WriteIndented = true
        });
    }
}

}