using LoopingMediaGallery.Interfaces;
using System;
using System.Globalization;
using System.Reflection;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace LoopingMediaGallery.Converters
{
    public class UriToBitmapImageConverter : IMultiValueConverter
	{
		public static UriToBitmapImageConverter Instance => _instance;
		private static UriToBitmapImageConverter _instance = new UriToBitmapImageConverter();

		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (!(values[0] is Uri)) return null;
			if (!(values[1] is MediaType)) return null;

			var uri = values[0] as Uri;
			var type = (MediaType)values[1];
			int dimensions = (parameter is int) ? (int)parameter : 32;

			if (type == MediaType.Image)
			{
				var image = new BitmapImage();
				image.BeginInit();
				image.DecodePixelWidth = 32;
				image.DecodePixelHeight = 32;
				image.UriSource = uri;
				image.CacheOption = BitmapCacheOption.OnLoad;
				image.EndInit();
				if (image.CanFreeze)
					image.Freeze();
				return image;
			}
			else
			{
				var assembly = Assembly.GetExecutingAssembly();
				var imageStream = assembly.GetManifestResourceStream("LoopingMediaGallery.Resources.videoThumb.png");

				var image = new BitmapImage();
				image.BeginInit();
				image.DecodePixelWidth = 32;
				image.DecodePixelHeight = 32;
				image.StreamSource = imageStream;
				image.CacheOption = BitmapCacheOption.OnLoad;
				image.EndInit();
				if (image.CanFreeze)
					image.Freeze();
				return image;
			}
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
