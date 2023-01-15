using System.Collections.Generic;
using System.Dynamic;
using System.IO.Compression;
using System.Text;
using Microsoft.ClearScript;
using Microsoft.ClearScript.JavaScript;
using Microsoft.ClearScript.V8;

namespace JSQuery;

class JsEngine {
    private static V8Runtime Runtime = new();

    public V8ScriptEngine engine;
    public V8Script? script;





    static JsEngine() {
        HostSettings.CustomAttributeLoader = new LowCamelCasweAttributeLoader();
    }





    public JsEngine() {
        engine = Runtime.CreateScriptEngine(V8ScriptEngineFlags.DisableGlobalMembers);

        AddType(typeof(Console));
        AddType(typeof(File));
        AddType(typeof(ExcelParser));
        AddType(typeof(WebParser));
        AddType(typeof(ZipFile));
    }





    public void Open(string directory, string mainJs) {
        if (!File.GetAttributes(directory).HasFlag(FileAttributes.Directory)) {
            throw new Exception("Path is not directory");
        }

        var raw = File.ReadAllText(mainJs);
        engine.DocumentSettings.SearchPath = directory;
        engine.DocumentSettings.AccessFlags = DocumentAccessFlags.EnableFileLoading;

        script = engine.Compile(new DocumentInfo() {
            Category = ModuleCategory.Standard
        }, raw);


        AddObject("Environment", new {
            currentDirectory = directory
        });
    }





    public void Run() {
        if (script == null) {
            throw new Exception("Main JS file is not open");
        }

        engine.Execute(script);
    }





    public void AddType(Type type) => engine.AddHostType(type.Name, type);

    public void AddTypeLowCase(Type type) => engine.AddHostType(char.ToLowerInvariant(type.Name[0]) + type.Name.Substring(1), type);

    public void AddObject<T>(string name, T value) => engine.AddHostObject(name, value);





    public void Dispose() {
        var data = engine.Script as DynamicObject;
        if (data != null) {
            var cleanup = new StringBuilder();

            foreach (var item in data.GetDynamicMemberNames()) {
                if (item != "EngineInternal") {
                    cleanup.Append("delete " + item + ";");
                }
            }

            engine.Execute(cleanup.ToString());
        }


        engine.CollectGarbage(true);
        engine.Dispose();
    }


}
