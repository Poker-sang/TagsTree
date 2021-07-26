using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Collections.Generic;

namespace TagsTree.Models
{
	public class RelationsDataTable : DataTable
	{
		private Dictionary<int, DataRow> RowsDict = new();
		public bool this[FileModel rowKey, string columnKey]
		{
			get => (bool)RowsDict[rowKey.GetHashCode()][columnKey];
			set => RowsDict[rowKey.GetHashCode()][columnKey] = value;
		}
		public DataRow GetRowAt(FileModel rowKey) => RowsDict[rowKey.GetHashCode()];
		public IEnumerable<FileModel> GetFileModels(string tag)
		{
			foreach (DataRow row in Rows)
				if ((bool)row[tag])
					yield return App.HashFiles[(int)row[0]];
		}
		public IEnumerable<string> GetTags(FileModel file)
		{
			foreach (DataColumn column in Columns)
				if ((bool)RowsDict[file.GetHashCode()][column])
					yield return column.ColumnName;
		}
		public IEnumerable<FileModel> GetFileModels(IEnumerable<string> tags)
		{
			var a= tags.GetEnumerator();
			a.MoveNext();
			foreach (var row in GetFileModels(a))
				yield return App.HashFiles[(int)row[0]];
		}
		private IEnumerable<DataRow> GetFileModels(IEnumerator<string> tags)
		{
			var tag = tags.Current;
			IEnumerable<DataRow> range;
			if (tags.MoveNext())
				range = GetFileModels(tags);
			else range = RowsDict.Values;
			foreach (var row in range)
				if ((bool)row[tag])
					yield return row;
		}
		public void NewRow(FileModel fileModel)
		{
			var newRow = NewRow();
			newRow["FileHash"] = fileModel.GetHashCode();
			Rows.Add(newRow);
		}
		public void NewColumn(string name)
		{
			var column = new DataColumn()
			{
				AllowDBNull = false,
				AutoIncrement = false,
				ColumnName = name,
				Caption = name,
				DataType = typeof(bool),
				ReadOnly = false,
				Unique = false
			};
			column.DefaultValue = false;    //不拎出来会因为"False"无法转化为bool类型而抛异常
			Columns.Add(column);
		}
		public void RenameColumn(string originalName, string newName)
		{
			foreach (DataColumn column in Columns)
				if (column.ColumnName == originalName)
				{
					column.ColumnName = newName;
					column.Caption = newName;
					return;
				}
		}
		public void DeleteColumn(string name)
		{
			foreach (DataColumn column in Columns)
				if (column.ColumnName == name)
				{
					Columns.Remove(column);
					return;
				}
		}

		private static bool _init;
		private RelationsDataTable() { }

		private RelationsDataTable(string name) => TableName = name;
		public static RelationsDataTable? Load()
		{
			if (_init)
				return null;
			_init = true;
			try
			{
				var temp = new RelationsDataTable("Relations");
				_ = temp.ReadXml(App.RelationsPath);
				foreach (DataRow row in temp.Rows)
					temp.RowsDict[(int)row[0]] = row; //row[0]即为row["FileHash"]
				return temp;
			}
			catch (Exception)
			{
				var temp = new RelationsDataTable("Relations");
				temp.Columns.Add(new DataColumn
				{
					AllowDBNull = false,
					AutoIncrement = false,
					ColumnName = "FileHash",
					Caption = "FileHash",
					DataType = typeof(int),
					ReadOnly = false,
					Unique = true
				});
				return temp;
			}
		}

		public async void Save() => await Task.Run(() => WriteXml(App.RelationsPath, XmlWriteMode.WriteSchema));
	}
}