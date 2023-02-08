using MiniExcelLibs;
using System.Data;
using System.Text;
using System.Xml.Linq;
using Konsole;
using Konsole.Forms;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System;
using Konsole.Internal;
using static Konsole.Menu;

namespace JSQuery;

internal class Program {
    static ScriptManager manager = new();
    static HighSpeedWriter writer = new();
    static bool finished = false;
    static Window window;
    static Style menuStyle;
    static Task writerTask;
    static Menu menu;


    static Program() { 
        window = new Window(writer);

        menuStyle = new Style(
            thickNess: LineThickNess.Single,
            body: Colors.DarkBlueOnWhite,
            title: Colors.DarkBlueOnWhite,
            columnHeaders: Colors.DarkBlueOnWhite,
            line: Colors.DarkBlueOnWhite,
            selectedItem: Colors.GrayOnBlue,
            bold: Colors.DarkBlueOnWhite
        );

        writerTask = Task.Run(() => {
            while (true) {
                if (finished) {
                    writer.Flush();
                }
            }
        });
    }




    static void Main(string[] args) {
        Console.OutputEncoding = Encoding.UTF8;
        Console.CursorVisible = false;

        while (true) {
            UpdateMenu();
            finished = true;
            menu.Run();

            Console.WriteLine("scr");
            Console.ReadKey();
        }
    }






    static void RunScript(int id) {
        var info = manager.list[id];
        var engine = new JsEngine();

        try {
            engine.Open(info.path, info.mainPath);
            engine.Run();
        } catch (Exception ex) {
            //Console.WriteLine(ex.Message);
        } finally {
            engine.Dispose();
        }
    }


    static void UpdateMenu() {
        var scripts = window.SplitLeft("");
        var description = window.SplitRight("Description");

        manager.Update();
        var menuActions = new List<MenuItem>();
        foreach (var script in manager.list) {
            menuActions.Add(new MenuItem(script.Value.name, _ => {
                finished = false;
                window.Clear();
                Console.Clear();

                RunScript(script.Key);
            }));
        }

        menu = new Menu(new MenuSettings {
            Console = scripts,
            Title = "Scripts",
            MenuActions = menuActions.ToArray(),
        });
        menu.Style = menuStyle;
    }

}
