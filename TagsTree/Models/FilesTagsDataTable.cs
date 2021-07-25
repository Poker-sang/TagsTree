using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace TagsTree.Models
{
	public class FilesTagsDataTable : DataTable
	{
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

		private FilesTagsDataTable() { }
		private FilesTagsDataTable(string name)
		{
			TableName = name;
		}
		public static async ValueTask<FilesTagsDataTable> Load()
		{
			return await Task.Run(() =>
			{
				try
				{
					var temp = new FilesTagsDataTable("Relations");
					_ = temp.ReadXml(App.RelationsPath);
					return temp;
				}
				catch (Exception)
				{
					var temp = new FilesTagsDataTable("Relations");
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
			});
		}

		public async void Save() => await Task.Run(() => WriteXml(App.RelationsPath, XmlWriteMode.WriteSchema));
	}
}