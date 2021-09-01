using System;
using System.Collections.ObjectModel;
using System.Text.Json.Serialization;
using TagsTreeWinUI3.Services;

namespace TagsTreeWinUI3.Models
{
	public class FilesChanged
	{
		public static int Num { get; set; } = 1;

		public int Id { get; }
		public string Name { get; }
		public string Path { get; }
		public string Type { get; }
		public string Other { get; }

		public static ObservableCollection<FilesChanged> Deserialize(string path) => Serialization.Deserialize<ObservableCollection<FilesChanged>>(path);
		public static void Serialize(string path, ObservableCollection<FilesChanged> collection) => Serialization.Serialize(path, collection);

		public FilesChanged(string fullName, string type, string other = "")
		{
			Id = Num;
			Num++;
			Name = fullName[(fullName.LastIndexOf('\\') + 1)..];
			Path = fullName[..fullName.LastIndexOf('\\')];
			Type = type;
			Other = other;
		}
		/// <summary>
		/// 反序列化专用，不要调用该构造器
		/// </summary>
		[JsonConstructor]
		public FilesChanged(int id, string name, string path, string type, string other = "")
		{
			Num = Math.Max(Num, id + 1);
			Id = id;
			Name = name;
			Path = path;
			Type = type;
			Other = other;
		}
	}
}