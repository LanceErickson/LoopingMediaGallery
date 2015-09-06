using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace LoopingMediaGallery
{
	class LoopingMediaController : IDisposable
    {
        private bool _running = false;
        public bool Running
        {
            get
            { return _running; }
            set { _running = value; }
        }

        private int _duration; 
        public int Duration
        {
            get
            {
                return _duration;
            }
            internal set
            {
                _duration = value;
                if (Running && _mediaSwitchTimer != null)
                {
                    _mediaSwitchTimer.Dispose();
                    InitializeTimer();
                }
            }
        }
        public List<string> FileList { get; internal set; }
        public string IdleImage { get; internal set; }

        private List<LoopingMediaControl> _views = new List<LoopingMediaControl>();
        private Timer _mediaSwitchTimer;

        private static string[] imageFormatExtensions = { ".png", ".jpg", ".jpeg" };
        private static string[] videoFormatExtensions = { ".mp4", ".avi", ".wmv" };


        private int _mediaIndex = 0;
        protected int MediaIndex
        {
            get
            {
                if (FileList == null || FileList.Count == 0)
                    return 0;
                return _mediaIndex;
            }
            set
            {
                if (FileList == null || FileList.Count == 0)
                    return;
                if (value > FileList.Count)
                {
                    _mediaIndex = 1;
                    return;
                }

                if (value < 0)
                {
                    _mediaIndex = 0;
                    return;
                }
                _mediaIndex = value;
            }
        }

        public LoopingMediaController()
        {
        }

        private void InitializeTimer()
        {
			if(_mediaSwitchTimer != null)
			{
				_mediaSwitchTimer.Dispose();
				_mediaSwitchTimer = null;
			}
            _mediaSwitchTimer = new System.Threading.Timer(GotoNext, new System.Threading.AutoResetEvent(false), (int)TimeSpan.FromSeconds(Duration).TotalMilliseconds, (int)TimeSpan.FromSeconds(Duration).TotalMilliseconds);
        }

        internal void Reset()
        {
			_views.ForEach(x => x.Clear());
            MediaIndex = 0;
            Next();
        }

        private void GotoNext(object sender)
        {
            Next();
        }

        internal void Subscribe(LoopingMediaControl loopingMediaControl)
        {
            _views.Add(loopingMediaControl);
			if(_views.Count == 1)
				loopingMediaControl.OnMediaFailure += MediaFailed;
        }

        internal void Unsubscribe(LoopingMediaControl loopingMediaControl)
        {
            _views.Remove(loopingMediaControl);
			if (_views.Count == 0)
				loopingMediaControl.OnMediaFailure -= MediaFailed;
        }

		private void MediaFailed(object sender, EventArgs e)
		{
			((LoopingMediaControl)sender).OnVideoFinished = null;
			Next();
		}

        internal void Start()
        {
            Running = true;
            Next();
        }

        internal void Stop()
        {
            Running = false;
            if(_mediaSwitchTimer != null)
                _mediaSwitchTimer.Dispose();
        }

		internal void Next()
		{
			if (_mediaSwitchTimer != null)
			{
				_mediaSwitchTimer.Dispose();
				_mediaSwitchTimer = null;
			}

			if (FileList == null || FileList.Count == 0)
			{
				if(Running)
					InitializeTimer();
				return;
			}

            MediaIndex++;

            string source = FileList[MediaIndex-1];

            if(imageFormatExtensions.Contains(Path.GetExtension(source), StringComparer.OrdinalIgnoreCase))
            {
                if (!File.Exists(source))
                {
                    Next();
                    return;
                }

				try
				{
					_views.ForEach(x => x.ShowImage(source));
				}
				catch
				{
					Next();
					return;
				}

				if(Running)
					InitializeTimer();
			}

            if(videoFormatExtensions.Contains(Path.GetExtension(source), StringComparer.OrdinalIgnoreCase))
            {
                if (!File.Exists(source))
                {
                    Next();
                    return;
                }

				try
				{
					var firstPlayer = _views.FirstOrDefault();
					if (firstPlayer != null)
					{
						firstPlayer.OnVideoFinished += VideoFinished;
						firstPlayer.ShowVideo(source);
					}
				}
				catch
				{
					Next();
					return;
				}

               // _views.ForEach(x => x.OnVideoFinished += VideoFinished);
     //          _views.ForEach(x => {
				 //  if(x != firstPlayer)
					//x.ShowVideo(source, true);
     //              });
            }

            return;
        }

        private void VideoFinished(object sender, EventArgs e)
        {
			((LoopingMediaControl)sender).OnVideoFinished -= VideoFinished;
            if (Running)
            {
                Next();
            }
        }

        internal void Previous()
        {
            MediaIndex = MediaIndex - 2;
            Next();
        }

		internal void Blank()
		{
			_views.ForEach(x => x.Clear());
			Stop();
		}

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls


        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~LoopingMediaController() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
