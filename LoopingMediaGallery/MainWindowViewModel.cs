using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Windows.Input;
using System.ComponentModel;

namespace LoopingMediaGallery
{
	public class MainWindowViewModel : IDisposable, INotifyPropertyChanged
    {
		private List<GalleryView> _galleries = new List<GalleryView>();		

        private System.Threading.Timer _fileRefreshTimer;

        private LoopingMediaController _loopingMediaController;

		public event PropertyChangedEventHandler PropertyChanged;
		public void SendPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

		#region Properties
		private string _folderPath;
        public string FolderPath
		{
			get { return _folderPath; }
			set
			{
				_folderPath = value;

				SendPropertyChanged("FolderPath");

				if (_loopingMediaController != null && _loopingMediaController.Running)
				{
					_loopingMediaController.Stop();
					ScanFolderPath(this);
					_loopingMediaController.Reset();
					_loopingMediaController.Start();
				}
				else
				{
					ScanFolderPath(this);
				}
			}
		}

		private LoopingMediaControl _previewPane;
		public LoopingMediaControl PreviewPane
        {
            get
            {
                return _previewPane;
            }
            set
            {
                if(_previewPane != null)
                {
					_loopingMediaController.Unsubscribe(_previewPane);
                }

                _previewPane = value;

                if(_previewPane != null)
                    _loopingMediaController.Subscribe(_previewPane);
            }
        }

        public int Duration
        {
            get
            {
                return _loopingMediaController.Duration;
            }
            set
            {
				if(_loopingMediaController != null)
					_loopingMediaController.Duration = value;
				SendPropertyChanged("Duration");
            }
        }

        public string IdleImage
        {
            get
            {
				if (_loopingMediaController != null)
					return _loopingMediaController.IdleImage;
				else
					return null;
            }
            set
            {
				if (_loopingMediaController != null)
					_loopingMediaController.IdleImage = value;
            }
        }

        public List<string> FileList
        {
            get
            {
				if (_loopingMediaController != null)
					return _loopingMediaController.FileList;
				else
					return null;
            }
            set
            {
				if (_loopingMediaController != null)
					_loopingMediaController.FileList = value;
            }
        }

		private int _refreshRate;
        public int RefreshRate {
			get
			{
				return _refreshRate;
			}
			set
			{
				_refreshRate = value;
				SendPropertyChanged("RefreshRate");
				InitializeRefreshTimer();
			}
		}
#endregion
		
		#region Commands 
		public ICommand SaveCommand
		{
			get;
			internal set;
		}

		public void CreateSaveCommand()
		{
			SaveCommand = new RelayCommand(SaveExecute, CanExecuteSaveCommand);
		}

		private bool CanExecuteSaveCommand(object obj)
		{
			return true;
		}

		private void SaveExecute(object obj)
		{
			Properties.Settings.Default["FolderPath"] = FolderPath;
			Properties.Settings.Default["RefreshRate"] = RefreshRate;
			Properties.Settings.Default["Duration"] = Duration;
			Properties.Settings.Default.Save();
		}

		public ICommand StartStopCommand
        {
            get;
            internal set;
        } 

        public void CreateStartStopCommand()
        {
            StartStopCommand = new RelayCommand(StartStopExecute, CanExecuteStartStopCommand);
        }

        private bool CanExecuteStartStopCommand(object obj)
        {
            return true;
        }

        private void StartStopExecute(object obj)
        {
            if (_loopingMediaController.Running)
                _loopingMediaController.Stop();
            else
                _loopingMediaController.Start();
        }

        public ICommand ResetCommand
        {
            get;
            internal set;
        }

        public void CreateResetCommand()
        {
            ResetCommand = new RelayCommand(ResetExecute, CanExecuteResetCommand);
        }

        private bool CanExecuteResetCommand(object obj)
        {
            return true;
        }

        private void ResetExecute(object obj)
        {
            _loopingMediaController.Reset();
        }

        public ICommand NextCommand
        {
            get;
            internal set;
        }

        public void CreateNextCommand()
        {
            NextCommand = new RelayCommand(NextExecute, CanExecuteNextCommand);
        }

        private bool CanExecuteNextCommand(object obj)
        {
            return true;
        }

        private void NextExecute(object obj)
        {
            _loopingMediaController.Next();
        }

        public ICommand PreviousCommand
        {
            get;
            internal set;
        }

        public void CreatePreviousCommand()
        {
            PreviousCommand = new RelayCommand(PreviousExecute, CanExecutePreviousCommand);
        }

        private bool CanExecutePreviousCommand(object obj)
        {
            return true;
        }

        private void PreviousExecute(object obj)
        {
            _loopingMediaController.Previous();
        }

        public ICommand BlankCommand
        {
            get;
            internal set;
        }

        public void CreateBlankCommand()
        {
            BlankCommand = new RelayCommand(BlankExecute, CanExecuteBlankCommand);
        }

        private bool CanExecuteBlankCommand(object obj)
        {
            return true;
        }

        private void BlankExecute(object obj)
        {
            _loopingMediaController.Blank();
        }
		#endregion

		public MainWindowViewModel()
        {
            CreateCommands();
            _loopingMediaController = new LoopingMediaController();
			LoadSettings();

			ScanFolderPath(this);

            _loopingMediaController.IdleImage = this.IdleImage;
            _loopingMediaController.FileList = this.FileList;
        }

		private void LoadSettings()
		{
			FolderPath = (string)Properties.Settings.Default["FolderPath"] ?? string.Empty;
			Duration = (int?)Properties.Settings.Default["Duration"] ?? 10;
			RefreshRate = (int?)Properties.Settings.Default["RefreshRate"] ?? 1;
		}

		private void InitializeRefreshTimer()
		{
			if (_fileRefreshTimer != null)
			{
				_fileRefreshTimer.Dispose();
				_fileRefreshTimer = null;
			}
			_fileRefreshTimer = new System.Threading.Timer(ScanFolderPath, new System.Threading.AutoResetEvent(false), (int)TimeSpan.FromMinutes(RefreshRate).TotalMilliseconds, (int)TimeSpan.FromMinutes(RefreshRate).TotalMilliseconds);
		}

		private void CreateCommands()
        {
            CreateStartStopCommand();
            CreateResetCommand();
            CreateNextCommand();
            CreatePreviousCommand();
			CreateSaveCommand();
			CreateBlankCommand();
        }

        public void Next(object sender, EventArgs e)
        {
            _loopingMediaController.Next();
        }

        public void Previous(object sender, EventArgs e)
        {
            _loopingMediaController.Previous();
        }

        private void ScanFolderPath(object sender)
        {
            if (string.IsNullOrEmpty(FolderPath) || string.IsNullOrWhiteSpace(FolderPath) || !Directory.Exists(FolderPath))
                return;

            string[] files = Directory.GetFiles(FolderPath);

            var fileList = files.ToList();

            foreach (var file in files)
            {
                var ext = Path.GetExtension(file);

                if (fileList != null && !(ext.ToLower() == ".jpg" || ext.ToLower() == ".mp4" || ext.ToLower() == ".png" || ext.ToLower() == ".wmv"))
                    fileList.Remove(file);
            }

            FileList = fileList;
        }

		public void AddGallery(GalleryView gallery)
		{
			_galleries.Add(gallery);
			_loopingMediaController.Subscribe(gallery.loopingMediaControl);
		}

		public void RemoveGallery(GalleryView gallery)
		{
			if (_galleries.Contains(gallery))
			{
				_galleries.Remove(gallery);
				_loopingMediaController.Unsubscribe(gallery.loopingMediaControl);
			}
		}

        public void Dispose()
        {
            foreach(var gallery in _galleries)
			{
				gallery.Close();
				_loopingMediaController.Unsubscribe(gallery.loopingMediaControl);
			}
			_galleries.Clear();
            _loopingMediaController.Stop();
            _loopingMediaController.Dispose();
        }
    }
}
