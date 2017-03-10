using LoopingMediaGallery.Interfaces;
using LoopingMediaGallery.Objects;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace LoopingMediaGallery.Controls
{
	public class AsyncImage : UserControl
	{
		private static readonly ConcurrentDictionary<Uri, BitmapImage> ImageCache = new ConcurrentDictionary<Uri, BitmapImage>();

		private readonly ILogger _logger;

		public AsyncImage()
		{
			_logger = new Logger();
		}

		public Uri UriSource
		{
			get { return (Uri)GetValue(UriSourceProperty); }
			set { SetValue(UriSourceProperty, value); }
		}
		public static DependencyProperty UriSourceProperty = 
			DependencyProperty.Register("UriSource", typeof(Uri), typeof(AsyncImage), new PropertyMetadata(null, (s,o) => SourceUriChanged(s,o)));

		private static void SourceUriChanged(DependencyObject s, DependencyPropertyChangedEventArgs o)
			=> (s as AsyncImage)?.UpdateSource();

		public ImageSource Source
		{
			get { return (ImageSource)GetValue(SourceProperty); }
			private set { SetValue(SourcePropertyKey, value); }
		}

		public DependencyProperty SourceProperty = SourcePropertyKey.DependencyProperty;

		private static DependencyPropertyKey SourcePropertyKey =
			DependencyProperty.RegisterReadOnly("Source", typeof(ImageSource), typeof(AsyncImage), null);

		private Task UpdateSource()
		{
			var height = (int)Height;
			var width = (int)Width;
			var uriSource = UriSource;
			return Task.Run(() =>
			{ 
				return ImageCache.GetOrAdd(uriSource, (uri) =>
				{
					_logger.Write(string.Format("{0} - Building image with URI {1}", this.GetType().Name, uri));
					var image = new BitmapImage();
					image.BeginInit();
					if (height > 0)
						image.DecodePixelWidth = height;
					if (width > 0)
						image.DecodePixelHeight = width;
					image.UriSource = uri;
					image.CacheOption = BitmapCacheOption.OnLoad;
					image.EndInit();
					if (image.CanFreeze)
						image.Freeze();
					_logger.Write(string.Format("{0} - Finished building image with URI {1}", this.GetType().Name, uri));

					return image;
				});
			}).ContinueWith((task) =>
			{
				if (task.Exception == null && task.Result != null)
				{
					_logger.Write(string.Format("{0} - Assigning asyncimage with uri {1}", this.GetType().Name, uriSource));

					Source = task.Result as BitmapImage;
				}
			}, TaskScheduler.FromCurrentSynchronizationContext());
		}
	}
}
