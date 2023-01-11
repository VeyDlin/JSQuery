using MiniExcelLibs;
using System.Data;
using System.Text;
using OfficeOpenXml;


namespace JSQuery;

public class ExcelParser {
    string file;

    public struct ListData {
        public string name;
        public List<List<object>> lines;
    };



    static ExcelParser() { 
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial; 
    }



    public ExcelParser(string filePath) {
        file = filePath;
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }





    public void Write(List<ListData> data) {
        using var exel = new ExcelPackage();

        foreach (var sheet in data) {
            var book = exel.Workbook.Worksheets.Add(sheet.name);

            for (int line = 0; line < sheet.lines.Count; line++) {
                for (int column = 0; column < sheet.lines[line].Count; column++) {
                    book.Cells[line + 1, column + 1].Value = sheet.lines[line][column];
                }
            }
        }

        exel.SaveAs(new FileInfo(file));
    }





    public List<ListData> Read() {
        List<ListData> data = new(); 
        
        foreach (var sheetName in MiniExcel.GetSheetNames(file)) {
            var table = MiniExcel.QueryAsDataTable(file, sheetName: sheetName, useHeaderRow: false);

            var list = new ListData() {
                name = sheetName,
                lines = new List<List<object>>()
            };

            foreach (DataRow row in table.Rows) {

                var line = new List<object>();
                for (int i = 0; i < table.Columns.Count; i++) {
                    line.Add(row[i]);
                }


                list.lines.Add(line);
            }


            data.Add(list);
        }

        return data;
    }
}
