using LoopingMediaGallery.Interfaces;
using System;
using System.Globalization;
using System.Reflection;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace LoopingMediaGallery.Converters
{
	public class UriToUriConverter : IMultiValueConverter
	{
		public static UriToUriConverter Instance => _instance;
		private static UriToUriConverter _instance = new UriToUriConverter();

		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (!(values[0] is Uri)) return null;
			if (!(values[1] is MediaType)) return null;

			var uri = values[0] as Uri;
			var type = (MediaType)values[1];
			int dimensions = (parameter is int) ? (int)parameter : 32;

			if (type == MediaType.Image)
			{
				return uri;
			}
			else
			{
				return new Uri("pack://application:,,,/Resources/videoThumb.png");
			}
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
