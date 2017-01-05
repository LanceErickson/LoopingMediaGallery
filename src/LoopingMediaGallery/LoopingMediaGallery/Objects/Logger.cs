using LoopingMediaGallery.Interfaces;
using System;

namespace LoopingMediaGallery.Objects
{
	public class Logger : ILogger
	{
		public void Write(string message) => Console.WriteLine(message);
	}
}
