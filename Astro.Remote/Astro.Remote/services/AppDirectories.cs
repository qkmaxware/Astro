using System;
using System.IO;

namespace Astro.Remote.Services {

public static class AppDirectories {
    public static string RootPath {
        get {
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Astro.Remote");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return path;
        }
    }
    
    public static TextWriter CreateFile(string name) {
        var writer = new StreamWriter (Path.Combine(RootPath, name));
        return writer;
    }

    public static string LoadFile(string name) {
        var path = Path.Combine(RootPath, name);
        if (File.Exists(path)) {
            return File.ReadAllText(path);
        } else {
            return null;
        }
    }
}

}