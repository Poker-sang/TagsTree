using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace TagsTree.Models
{
	public class RelationsDataTable : DataTable
	{
		private readonly Dictionary<int, DataRow> RowsDict = new();
		public bool this[FileModel rowKey, string columnKey]
		{
			get => (bool)RowsDict[rowKey.Id][columnKey];
			set => RowsDict[rowKey.Id][columnKey] = value;
		}
		public DataRow RowAt(FileModel rowKey) => RowsDict[rowKey.Id];
		public IEnumerable<FileModel> GetFileModels(string tag) => from DataRow row in Rows where (bool)row[tag] select App.IdToFile[(int)row[0]];

		public IEnumerable<string> GetTags(FileModel file)
		{
			for (var i = 1; i < Columns.Count; i++)
				if ((bool)RowsDict[file.Id][Columns[i]])
					yield return Columns[i].ColumnName;
		}
		public IEnumerable<FileModel> GetFileModels(List<string> tags)
		{
			if (tags.Count == 0)
				return App.IdToFile.Values.ToList();
			var enumerator = tags.GetEnumerator();
			_ = enumerator.MoveNext();
			return GetFileModels(enumerator).Select(row => App.IdToFile[(int)row[0]]).ToList();
		}
		private List<DataRow> GetFileModels(IEnumerator<string> tags)
		{
			var tag = tags.Current;
			List<DataRow> range = tags.MoveNext() ? GetFileModels(tags) : RowsDict.Values.ToList();
			var tempList = range.Where(row => (bool) row[tag]).ToList();
			tempList.AddRange(App.Tags.Values.Where(childTag => App.Tags[tag].HasChildTag(childTag))
				.SelectMany(_ => range, (childTag, row) => new {childTag, row})
				.Where(t => (bool) t.row[t.childTag.Name])
				.Select(t => t.row));
			return tempList;
		}
		public void NewRow(FileModel fileModel)
		{
			var newRow = NewRow();
			newRow[0] = fileModel.Id;
			RowsDict[(int)newRow[0]] = newRow;
			Rows.Add(newRow);
		}
		public void NewColumn(string name)
		{
			var column = new DataColumn //不拎出来会因为"False"无法转化为bool类型而抛异常
			{
				AllowDBNull = false,
				AutoIncrement = false,
				ColumnName = name,
				Caption = name,
				DataType = typeof(bool),
				ReadOnly = false,
				Unique = false,
				DefaultValue = false
			};
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
					temp.RowsDict[(int)row[0]] = row; //row[0]即为row["FileId"]
				return temp;
			}
			catch (Exception)
			{
				var temp = new RelationsDataTable("Relations");
				temp.Columns.Add(new DataColumn
				{
					AllowDBNull = false,
					AutoIncrement = false,
					ColumnName = "FileId",
					Caption = "FileId",
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