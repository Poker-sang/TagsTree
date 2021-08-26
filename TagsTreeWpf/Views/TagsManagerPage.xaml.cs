using ModernWpf.Controls;
using System;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml;
using TagsTreeWpf.Commands;
using TagsTreeWpf.Models;
using TagsTreeWpf.Services;
using TagsTreeWpf.Services.ExtensionMethods;
using TagsTreeWpf.ViewModels;

namespace TagsTreeWpf.Views
{
	public partial class TagsManagerPage : System.Windows.Controls.Page
	{
		public TagsManagerPage()
		{
			DataContext = _vm = new TagsManagerViewModel();
			PPasteCmClick = new RelayCommand(_ => _clipBoard is not null, PasteCmClick);
			PPasteXCmClick = new RelayCommand(_ => _clipBoard is not null, PasteXCmClick);
			InitializeComponent();
			_ = TvTags.SetBinding(ItemsControl.ItemsSourceProperty, new Binding(".") { Mode = BindingMode.TwoWay, Source = _vm.Xdp });

			TbName.TextChanged += NameChanged;
			TvTags.SelectedItemChanged += TvSelectItemChanged;
		}

		private readonly TagsManagerViewModel _vm;
		public RelayCommand PPasteCmClick { get; } //public才能被BindingProxy找到
		public RelayCommand PPasteXCmClick { get; }
		private static XmlElement TvItemGetHeader(object? sender) => (XmlElement)((TreeViewItem)sender!).Header;
		private static XmlElement MItemGetHeader(object? sender) => TvItemGetHeader(((MenuItem)sender!).Tag!);

		private XmlElement? _clipBoard;
		private XmlElement? ClipBoard
		{
			get => _clipBoard;
			set
			{
				if (Equals(value, _clipBoard)) return;
				_clipBoard = value;
				PPasteCmClick.OnCanExecuteChanged();
				PPasteXCmClick.OnCanExecuteChanged();
			}
		}
		#region 拖拽

		private void TbTag_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) => MouseX.LastMousePos = e.GetPosition(TvTags);
		private void TbTag_MouseMove(object sender, MouseEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed && MouseX.MouseDisplace(4, e.GetPosition(TvTags)))
				_ = DragDrop.DoDragDrop((TreeViewItem)sender, TvItemGetHeader(sender), DragDropEffects.Move);
		}
		private void TbTag_DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetData(typeof(XmlElement)) is XmlElement origin)
				if (origin != TvItemGetHeader(sender))
					((TreeViewItem)sender).Foreground = new SolidColorBrush(Colors.LightGray);
			e.Handled = true;
		}
		private void TbTag_DragLeave(object sender, DragEventArgs dragEventArgs)
		{
			((TreeViewItem)sender).Foreground = new SolidColorBrush(Colors.Black);
		}
		private void TbTag_Drop(object sender, DragEventArgs e)
		{
			((TreeViewItem)sender).Foreground = new SolidColorBrush(Colors.Black);
			if (e.Data.GetData(typeof(XmlElement)) is not XmlElement origin)
				return;
			e.Effects = DragDropEffects.None;
			e.Handled = true;
			if (origin != TvItemGetHeader(sender))
				MoveTag(origin, TvItemGetHeader(sender));
		}
		private void TvTags_Drop(object sender, DragEventArgs e)
		{
			if (e.Data.GetData(typeof(XmlElement)) is not XmlElement origin)
				return;
			e.Effects = DragDropEffects.None;
			e.Handled = true;
			MoveTag(origin, null);
		}

		#endregion


		#region 事件处理

		private void NameChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs e)
		{
			_vm.Name = Regex.Replace(_vm.Name, $@"[{FileX.GetInvalidNameChars}]+", "");
			var textBox = (TextBox)typeof(AutoSuggestBox).GetField("m_textBox", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(sender)!;
			textBox.SelectionStart = textBox.Text.Length;
		}

		private void TvSelectItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e) => TbPath.Path = TagMethods.TvSelectedItemChanged((XmlElement?)e.NewValue) ?? TbPath.Path;
		
		#endregion

		#region 命令

		private void NewBClick(object parameter, RoutedEventArgs e)
		{
			if (TbPath.Path.GetTagModel() is not { } pathTagModel)
			{
				MessageBoxX.Error("「标签路径」不存在！");
				return;
			}
			if (!NewTagCheck(_vm.Name)) return;
			NewTag(_vm.Name, pathTagModel.XmlElement);
			_vm.Name = "";
		}
		private void MoveBClick(object parameter, RoutedEventArgs e)
		{
			if (TbPath.Path.GetTagModel() is not { } pathTagModel)
			{
				MessageBoxX.Error("「标签路径」不存在！");
				return;
			}
			if (_vm.Name.GetTagModel() is not { } nameTagModel)
			{
				MessageBoxX.Error("「标签名称」不存在！");
				return;
			}
			MoveTag(nameTagModel.XmlElement, pathTagModel.XmlElement);
			_vm.Name = "";
		}
		private void RenameBClick(object parameter, RoutedEventArgs e)
		{
			if (TbPath.Path is "")
			{
				MessageBoxX.Error("未输入希望重命名的标签");
				return;
			}
			if (TbPath.Path.GetTagModel() is not { } pathTagModel)
			{
				MessageBoxX.Error("「标签路径」不存在！");
				return;
			}
			if (!NewTagCheck(_vm.Name)) return;
			RenameTag(_vm.Name, pathTagModel.XmlElement);
			_vm.Name = "";
			TbPath.Path = "";
		}
		private void DeleteBClick(object parameter, RoutedEventArgs e)
		{
			if (TbPath.Path is "")
			{
				MessageBoxX.Error("未输入希望删除的标签");
				return;
			}
			if (TbPath.Path.GetTagModel() is not { } pathTagModel)
			{
				MessageBoxX.Error("「标签路径」不存在！");
				return;
			}
			DeleteTag(pathTagModel.XmlElement);
			_vm.Name = "";
		}
		private void SaveBClick(object parameter, RoutedEventArgs e)
		{
			App.SaveXdTags();
			App.SaveRelations();
			BSave.IsEnabled = false;
		}

		private void NewCmClick(object sender, RoutedEventArgs e)
		{
			var dialog = new InputName(FileX.InvalidMode.Name);
			if (dialog.ShowDialog() != false && NewTagCheck(dialog.Message))
				NewTag(dialog.Message, MItemGetHeader(sender));
		}
		private void NewXCmClick(object sender, RoutedEventArgs e)
		{
			var dialog = new InputName(FileX.InvalidMode.Name);
			if (dialog.ShowDialog() != false && NewTagCheck(dialog.Message))
				NewTag(dialog.Message, App.XdpRoot!);
		}
		private void CutCmClick(object sender, RoutedEventArgs e) => ClipBoard = MItemGetHeader(sender);
		private void PasteCmClick(object? parameter)
		{
			MoveTag(ClipBoard!, (XmlElement)((TreeViewItem)parameter!).Header);
			ClipBoard = null;
		}
		private void PasteXCmClick(object? parameter)
		{
			MoveTag(ClipBoard!, App.XdpRoot!);
			ClipBoard = null;
		}
		private void RenameCmClick(object sender, RoutedEventArgs e)
		{
			var dialog = new InputName(FileX.InvalidMode.Name);
			if (dialog.ShowDialog() != false && NewTagCheck(dialog.Message))
				RenameTag(dialog.Message, MItemGetHeader(sender));
		}
		private void DeleteCmClick(object sender, RoutedEventArgs e) => DeleteTag(MItemGetHeader(sender));

		#endregion

		#region 操作

		private void NewTag(string name, XmlElement path)
		{
			var element = _vm.Xdp.Document.CreateElement("Tag");
			var temp = new TagModel(name, path.GetAttribute("name"), element);
			element.SetAttribute("name", name);
			element.SetAttribute("id", temp.Id.ToString());
			_ = path.AppendChild(element);
			App.Tags[temp.Id, name] = temp;
			App.Relations.NewColumn(temp.Id);
			BSave.IsEnabled = true;
		}
		private void MoveTag(XmlElement name, XmlElement? path)
		{
			try
			{
				path ??= (XmlElement?)_vm.Xdp.Document.LastChild;
				_ = path!.AppendChild(name); //原位置自动被删除
				App.Tags[name.GetAttribute("name")].Path = path.GetAttribute("name").GetTagModel()!.FullName;
				BSave.IsEnabled = true;
			}
			catch (ArgumentException)
			{
				MessageBoxX.Error("禁止将标签移动到自己目录下");
			}
		}
		private void RenameTag(string name, XmlElement path)
		{
			var temp = App.Tags[path.GetAttribute("name")];
			path.SetAttribute("name", name);
			App.Tags.ChangeKey2(temp.Name, name);
			temp.Name = name;
			BSave.IsEnabled = true;
		}
		private void DeleteTag(XmlElement path)
		{
			_ = path.ParentNode!.RemoveChild(path);
			var temp = Convert.ToInt32(path.GetAttribute("id"));
			App.Relations.DeleteColumn(temp);
			App.Tags.Remove(temp);
			BSave.IsEnabled = true;
		}

		private static bool NewTagCheck(string name)
		{
			if (name is "")
			{
				MessageBoxX.Error("标签名称不能为空！");
				return false;
			}
			if (name.GetTagModel() is not null)
			{
				MessageBoxX.Error("与现有标签重名！");
				return false;
			}
			return true;
		}

		#endregion
	}
}
