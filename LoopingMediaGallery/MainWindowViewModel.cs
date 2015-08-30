using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

		private string _folderPath;
        public string FolderPath
		{
			get { return _folderPath; }
			set
			{
				_folderPath = value;

				SendPropertyChanged("FolderPath");

				if (_loopingMediaController.Running)
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

		public event PropertyChangedEventHandler PropertyChanged;
		public void SendPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

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
                _loopingMediaController.Duration = value;
            }
        }

        public string IdleImage
        {
            get
            {
                return _loopingMediaController.IdleImage;
            }
            set
            {
                _loopingMediaController.IdleImage = value;
            }
        }

        public List<string> FileList
        {
            get
            {
                return _loopingMediaController.FileList;
            }
            set
            {
                _loopingMediaController.FileList = value;
            }
        }

        public int RefreshRate { get; set; }

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

        public MainWindowViewModel()
        {
            CreateCommands();
            _loopingMediaController = new LoopingMediaController();
            Duration = 5;

            RefreshRate = (int)TimeSpan.FromMinutes(1).TotalMilliseconds;
            FolderPath = "C:\\Users\\Lance\\Desktop\\SlideshowTest";

            // Initialize folder monitoring timer
            _fileRefreshTimer = new System.Threading.Timer(ScanFolderPath, new System.Threading.AutoResetEvent(false), RefreshRate, RefreshRate);

            ScanFolderPath(this);

            _loopingMediaController.Duration = this.Duration;
            _loopingMediaController.IdleImage = this.IdleImage;
            _loopingMediaController.FileList = this.FileList;
        }

        private void CreateCommands()
        {
            CreateStartStopCommand();
            CreateResetCommand();
            CreateNextCommand();
            CreatePreviousCommand();
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

                if (FileList != null && !(ext == "jpg" || ext == "mp4" || ext == "png" || ext == "wmv"))
                    FileList.Remove(file);
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
