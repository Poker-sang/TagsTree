using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TagsTree.Models
{
	public class FileModel
	{
		public FileModel(string name, string path, bool isFolder)
		{
			Name = name;
			Path = path;
			IsFolder = isFolder;
		}

		public string Name { get; set; }
		public string Path { get; set; }
		public bool IsFolder { get; set; }
	}
}
