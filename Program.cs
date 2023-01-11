using MiniExcelLibs;
using System.Data;
using System.Text;
using System.Xml.Linq;

namespace JSQuery;

internal class Program {
    static ScriptManager manager = new();




    static void Main(string[] args) {
        Console.OutputEncoding = Encoding.UTF8;

        while (true) {
            manager.Update();

            if (!PrintList()) {
                PrintAndClear("Scripts not found");
                continue;
            }

            ScriptInfo info;
            try {
                var id = Convert.ToInt32(Console.ReadLine());
                info = manager.list[id];
            } catch {
                PrintAndClear("Wrong id");
                continue;
            }

            Console.Clear();

            var engine = new JsEngine();

            try {
                engine.Open(info.path, info.mainPath);
                engine.Run();
            } catch (Exception ex){
                Console.WriteLine(ex.Message);
            } finally {
                engine.Dispose();
                PrintAndClear("\r\n========== Script finished ==========");
            }

                
        }



    }






    static void PrintAndClear(string str) {
        Console.WriteLine(str);
        Console.ReadKey();
        Console.Clear();
    }





    static bool PrintList() {
        if (manager.list.Count == 0) {
            return false;
        }

        Console.WriteLine("Scripts:");

        foreach (var script in manager.list) {
            Console.WriteLine(string.Format("   {0}. {1}", script.Key, script.Value.name));
        }

        Console.WriteLine("");
        Console.Write("Select id: ");

        return true;
    }

}
