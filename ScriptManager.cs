using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSQuery;


public struct ScriptInfo {
    public string path;
    public string main;
    public string name { get => Path.GetFileName(path); }
    public string mainPath { get => path + @"\" + main; }

    public ScriptInfo(string path, string main) {
        this.path = path;
        this.main = main;
    }
}




public class ScriptManager {
    public string pluginsDirectory { get; private set; }
    public Dictionary<int, ScriptInfo> list { get; private set; }



    public ScriptManager(string? path = null) {
        list = new Dictionary<int, ScriptInfo>();
        pluginsDirectory = path ?? Environment.CurrentDirectory + @"\scripts";
        Update();
    }






    public void Update() {
        list.Clear();

        Directory.CreateDirectory(pluginsDirectory);

        var dirs = GetPluginsDirectories();

        int index = 1;
        foreach (var dir in dirs) {
            var main = GetMain(dir);
            if (main == null) {
                continue;
            }

            list.Add(index++, new(dir, Path.GetFileName(main)));
        }
    }





    private string? GetMain(string path) {
        var dirName = Path.GetFileName(path) ?? "";

        foreach (var file in GetJsFiles(path)) {
            var fileName = Path.GetFileNameWithoutExtension(file);
            switch (fileName) {
                case "index":
                case "main":
                case var value when value == dirName:
                return file;
            }
        }

        return null;
    }





    string[] GetJsFiles(string path) => Directory.GetFiles(path, "*.js", SearchOption.TopDirectoryOnly);
    string[] GetPluginScripts(string pluginName) => GetJsFiles(pluginsDirectory + "\\" + pluginName);
    string[] GetPluginsDirectories() => Directory.GetDirectories(pluginsDirectory, "*", SearchOption.TopDirectoryOnly);
}
