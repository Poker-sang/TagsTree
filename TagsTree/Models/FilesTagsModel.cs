using System;
using System.Data;
using System.Threading.Tasks;

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

		private FilesTagsDataTable() { }
		private FilesTagsDataTable(string name) 
		{
			TableName = name;
		}
		public static async ValueTask<FilesTagsDataTable> Load() => await Task.Run(() =>
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
		public async void Save() => await Task.Run(() => WriteXml(App.RelationsPath, XmlWriteMode.WriteSchema));
	}
}