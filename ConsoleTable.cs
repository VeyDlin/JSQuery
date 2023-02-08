using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static JSQuery.ConsoleTable;

namespace JSQuery;
public class ConsoleTable {
    public enum TableAlignment { Left, Right }
    public TableAlignment NumberAlignment { get; set; } = TableAlignment.Left;
    public IList<object> Columns { get; set; }
    public IList<object[]> Rows { get; protected set; }

    public Type[] ColumnTypes { get; private set; }

    public static HashSet<Type> NumericTypes = new HashSet<Type> {
        typeof(int),  typeof(double),  typeof(decimal),
        typeof(long), typeof(short),   typeof(sbyte),
        typeof(byte), typeof(ulong),   typeof(ushort),
        typeof(uint), typeof(float)
    };





    public ConsoleTable(params string[] columns) {
        Rows = new List<object[]>();
        Columns = new List<object>(columns);
    }





    public ConsoleTable AddColumn(IEnumerable<string> names) {
        foreach (var name in names) {
            Columns.Add(name);
        }

        return this;
    }





    public ConsoleTable AddRow(params object[] values) {
        if (values == null) {
            throw new ArgumentNullException(nameof(values));
        }

        if (!Columns.Any()) {
            throw new Exception("Please set the columns first");
        }

        if (Columns.Count != values.Length) {
            throw new Exception(
                $"The number columns in the row ({Columns.Count}) does not match the values ({values.Length})"
            );
        }

        Rows.Add(values);

        return this;
    }





    public void Write() {
        var builder = new StringBuilder();
        var columnLengths = ColumnLengths();
        var format = Format(columnLengths);
        var columnHeaders = string.Format(format, Columns.ToArray());
        var results = Rows.Select(row => string.Format(format, row)).ToList();
        var divider = Regex.Replace(columnHeaders, @"[^|]", "-");
        var dividerPlus = divider.Replace("|", "+");

        builder.AppendLine(dividerPlus);
        builder.AppendLine(columnHeaders);

        foreach (var row in results) {
            builder.AppendLine(dividerPlus);
            builder.AppendLine(row);
        }
        builder.AppendLine(dividerPlus);

        Console.WriteLine(builder.ToString());
    }





    private string Format(List<int> columnLengths, char delimiter = '|') {
        var columnAlignment = 
            Enumerable
            .Range(0, Columns.Count)
            .Select(GetNumberAlignment)
            .ToList();

        var delimiterStr = delimiter == char.MinValue ? string.Empty : delimiter.ToString();

        var format =
            Enumerable
            .Range(0, Columns.Count)
            .Select(i => " " + delimiterStr + " {" + i + "," + columnAlignment[i] + columnLengths[i] + "}")
            .Aggregate((s, a) => s + a) + " " + delimiterStr;

        return format.Trim();
    }





    private string GetNumberAlignment(int i) {
        if (
            NumberAlignment == TableAlignment.Right && 
            ColumnTypes != null && 
            NumericTypes.Contains(ColumnTypes[i])
        ) {
            return "";
        }

        return "-";
    }





    private List<int> ColumnLengths() {
        var columnLengths = Columns
            .Select((t, i) => Rows.Select(x => x[i])
                .Union(new[] { Columns[i] })
                .Where(x => x != null)
                .Select(x => x.ToString().Length).Max())
            .ToList();
        return columnLengths;
    }
}

