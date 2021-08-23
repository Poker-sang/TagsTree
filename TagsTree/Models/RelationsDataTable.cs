using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TagsTree.Services.ExtensionMethods;
using TagsTree.ViewModels;
using static TagsTree.Properties.Settings;

namespace TagsTree.Models
{
	public class RelationsDataTable : DataTable
	{
		private readonly Dictionary<int, DataRow> _rowsDict = new();
		public bool this[FileModel fileModel, TagModel tag]
		{
			get => (bool)_rowsDict[fileModel.Id][tag.Id];
			set => _rowsDict[fileModel.Id][tag.Id.ToString()] = value;
		}
		public DataRow RowAt(FileModel rowKey) => _rowsDict[rowKey.Id];


		public IEnumerable<string> GetTags(FileModel file)
		{
			for (var i = 1; i < Columns.Count; i++)
				if ((bool)_rowsDict[file.Id][Columns[i]])
					yield return App.Tags[Convert.ToInt32(Columns[i].ColumnName)].Name;
		}
		private readonly struct Part
		{
			public readonly int Num;
			public readonly FileViewModel File;

			public Part(int num, FileViewModel file)
			{
				Num = num;
				File = file;
			}
		}

		public static ObservableCollection<FileViewModel> FuzzySearchName(string name, IEnumerable<FileViewModel> range)
		{
			var precise = new List<FileViewModel>();
			var fuzzy = new List<FileViewModel>();
			var part = new List<Part>();
			var fuzzyRegex = new Regex(Regex.Replace(name, "(.)", ".+$1", RegexOptions.IgnoreCase));
			var partRegex = new Regex($"[{name}]", RegexOptions.IgnoreCase);
			foreach (var fileViewModel in range)
			{
				if (fileViewModel.Name.Contains(name, StringComparison.OrdinalIgnoreCase))
					precise.Add(fileViewModel);
				else if (fuzzyRegex.IsMatch(fileViewModel.Name))
					fuzzy.Add(fileViewModel);
				else
				{
					var matches = partRegex.Matches(fileViewModel.Name);
					if (matches.Count != 0)
						part.Add(new Part(matches.Count, fileViewModel));
				}
			}
			precise.AddRange(fuzzy);
			part.Sort((x, y) => x.Num.CompareTo(y.Num));
			precise.AddRange(part.Select(item => item.File));
			var temp = new ObservableCollection<FileViewModel>();
			foreach (var fileModel in precise)
				temp.Add(fileModel);
			return temp;
		}
		public IEnumerable<FileModel> GetFileModels(List<PathTagModel>? tags = null)
		{
			if (tags is null || tags.Count == 0)
				return App.IdFile.Values.ToList();
			var validTags = new Dictionary<PathTagModel, bool>();
			foreach (var tag in tags.Where(tag => !validTags.ContainsKey(tag)))
				validTags[tag] = true;
			var enumerator = tags.GetEnumerator();
			_ = enumerator.MoveNext();
			return GetFileModels(enumerator).Select(row => App.IdFile[(int)row[0]]).ToList();
		}
		private List<DataRow> GetFileModels(IEnumerator<PathTagModel> tags)
		{
			var tag = tags.Current;
			var lastRange = tags.MoveNext() ? GetFileModels(tags) : _rowsDict.Values.ToList();
			if (App.Tags.ContainsKey(tag.Name))
			{
				var dataRows = lastRange.Where(row => (bool)row[tag.Name.GetTagModel()!.Id.ToString()]).ToList();
				dataRows.AddRange(App.Tags.Values.Where(childTag => App.Tags[tag.Name].HasChildTag(childTag))
					.SelectMany(_ => lastRange, (childTag, row) => new { childTag, row })
					.Where(t => (bool)t.row[t.childTag.Id.ToString()])
					.Select(t => t.row));
				return dataRows;
			}
			if (Default.PathTags) //唯一需要判断是否能使用路径作为标签的地方
			{
				return lastRange
					.SelectMany(dataRow => App.IdFile[(int)dataRow[0]].PartialPath[4..].Split('\\', StringSplitOptions.RemoveEmptyEntries), (dataRow, pathTag) => new { dataRow, pathTag })
					.Where(t => t.pathTag == tag.Name)
					.Select(t => t.dataRow).ToList();
			}
			return new List<DataRow>();
		}
		public void NewRow(FileModel fileModel)
		{
			var newRow = NewRow();
			newRow[0] = fileModel.Id;
			_rowsDict[(int)newRow[0]] = newRow;
			Rows.Add(newRow);
		}
		public void NewColumn(int id)
		{
			var column = new DataColumn //不拎出来会因为"False"无法转化为bool类型而抛异常
			{
				AllowDBNull = false,
				AutoIncrement = false,
				ColumnName = id.ToString(),
				Caption = id.ToString(),
				DataType = typeof(bool),
				ReadOnly = false,
				Unique = false,
				DefaultValue = false
			};
			Columns.Add(column);
		}
		public void DeleteColumn(int id)
		{
			foreach (DataColumn column in Columns)
				if (column.ColumnName == id.ToString())
				{
					Columns.Remove(column);
					return;
				}
		}

		public RelationsDataTable() => TableName = "Relations";


		public void RefreshRowsDict()
		{
			_rowsDict.Clear();
			foreach (DataRow row in Rows)
				_rowsDict[(int)row[0]] = row; //row[0]即为row["FileId"]
		}

		public void Load()
		{
			try
			{
				ReadXml(App.RelationsPath);
				RefreshRowsDict();
			}
			catch (Exception)
			{
				Clear();
				Columns.Add(new DataColumn
				{
					AllowDBNull = false,
					AutoIncrement = false,
					ColumnName = "FileId",
					Caption = "FileId",
					DataType = typeof(int),
					ReadOnly = false,
					Unique = true
				});
				foreach (var tag in App.Tags.Values)
					NewColumn(tag.Id);
				foreach (var fileModel in App.IdFile.Values)
					NewRow(fileModel);
			}
		}

		public async void Save(string path) => await Task.Run(() => WriteXml(path, XmlWriteMode.WriteSchema));
	}
}