using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TagsTree.Algorithm;

public class TableDictionary<TColumn, TRow> where TColumn : notnull where TRow : notnull
{
    public Dictionary<TColumn, int> Columns = new();
    public Dictionary<TRow, int> Rows = new();
    public List<List<bool>> Table = new();
    private readonly Func<string, TColumn> _columnParse;
    private readonly Func<string, TRow> _rowParse;

    public TableDictionary(Func<string, TColumn> columnParse, Func<string, TRow> rowParse)
    {
        _columnParse = columnParse;
        _rowParse = rowParse;
    }

    public List<bool> this[TColumn column]
    {
        get => Table[Columns[column]];
        set => Table[Columns[column]] = value;
    }
    public bool this[TColumn column, TRow row]
    {
        get => this[column][Rows[row]];
        set => this[column][Rows[row]] = value;
    }

    public void AddColumn(TColumn column)
    {
        Columns[column] = Columns.Count;
        var temp = new List<bool>(Rows.Count);
        for (var i = 0; i < Rows.Count; ++i)
            temp.Add(false);
        Table.Add(temp);
    }
    public void AddRow(TRow row)
    {
        Rows[row] = Rows.Count;
        foreach (var list in Table)
            list.Add(false);
    }
    public void RemoveColumn(TColumn column)
    {
        var index = Columns[column];
        Table.RemoveAt(index);
        _ = Columns.Remove(column);
        foreach (var (key, value) in Columns)
            if (value > index)
                --Columns[key];
    }
    public void RemoveRow(TRow row)
    {
        var index = Rows[row];
        foreach (var list in Table)
            list.RemoveAt(index);
        _ = Rows.Remove(row);
        foreach (var (key, value) in Rows)
            if (value > index)
                --Rows[key];
    }

    public void Deserialize(string path)
    {
        Table.Clear();
        Columns.Clear();
        Rows.Clear();
        var buffer = File.ReadAllText(path);
        var lines = buffer.Split(';');
        var rows = lines[0].Split(',').Select(row => _rowParse(row)).ToArray();
        foreach (var row in rows)
            Rows[row] = Rows.Count;
        foreach (var line in lines.Skip(1))
        {
            var columns = line.Split(',');
            var column = _columnParse(columns[0]);
            Columns[column] = Columns.Count;
            Table.Add(new());
            foreach (var c in columns[1])
                this[column].Add(c is '1');
        }
    }

    public void Serialize(string path)
    {
        if (Table.Count is not 0 && Table[0].Count is not 0)
        {
            var buffer = Rows.Keys.Aggregate("", (current, key) => current + key + ",");
            buffer = buffer.Remove(buffer.Length - 1) + ";";
            buffer = Columns.Aggregate(buffer, (current, pair) => this[pair.Key].Aggregate(current + pair.Key + ",", (currentX, value) => currentX + (value ? 1 : 0)) + ";");
            buffer = buffer.Remove(buffer.Length - 1);
            File.WriteAllText(path, buffer, Encoding.UTF8);
        }
    }
}