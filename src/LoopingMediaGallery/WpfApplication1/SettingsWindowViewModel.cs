using Microsoft.WindowsAPICodePack.Dialogs;
using LoopingMediaGallery.Interfaces;
using System;

namespace LoopingMediaGallery
{
	public class SettingsWindowViewModel
	{
		private readonly ISettingsProvider _settingsProvider;
		private readonly ISaveSettings _settingsSaver;

		public event EventHandler SendNeedsClose;

		public SettingsWindowViewModel(ISettingsProvider settingsProvider, ISaveSettings settingsSaver)
		{
			if (settingsProvider == null) throw new ArgumentNullException(nameof(settingsProvider));
			if (settingsSaver == null) throw new ArgumentNullException(nameof(settingsSaver));

			_settingsProvider = settingsProvider;
			_settingsSaver = settingsSaver;
 		}

		public int RefreshRate
		{
			get { return _settingsProvider.FileRefreshRate; }
			set { _settingsSaver.RefreshRate = value; }
		}

		public int Duration
		{
			get { return _settingsProvider.Duration; }
			set { _settingsSaver.Duration = value; }
		}

		public string FolderPath
		{
			get { return _settingsProvider.FileFolderPath; }
			set { _settingsSaver.FolderPath = value; }
		}

		public void Save()
		{
			_settingsSaver.Save();
			SendNeedsClose?.Invoke(this, new EventArgs());
		}

		public void Open()
		{
			var dlg = new CommonOpenFileDialog();
			dlg.Title = "Select a path to monitor";
			dlg.IsFolderPicker = true;

			dlg.AddToMostRecentlyUsedList = false;
			dlg.AllowNonFileSystemItems = false;
			dlg.EnsureFileExists = true;
			dlg.EnsurePathExists = true;
			dlg.EnsureReadOnly = false;
			dlg.EnsureValidNames = true;
			dlg.Multiselect = false;
			dlg.ShowPlacesList = true;

			if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
			{
				FolderPath = dlg.FileName;
			}
		}
	}
}