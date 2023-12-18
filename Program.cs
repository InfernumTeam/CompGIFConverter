using System.IO.Compression;
using System.Runtime.Versioning;

namespace CompGIFConverter
{
	public class Program
	{
		private static int ConvertedGIFs;

		private static int FailedGIFS;

		public static void Main()
		{
			// Loop through every file with the .compgif extension in the current folder, and attempt to create a gif from it.
			foreach (var filepath in Directory.EnumerateFiles(Environment.CurrentDirectory, "*.compgif"))
				TryCreateGif(filepath);

			string failedGifText = FailedGIFS > 0 ? $"\n{FailedGIFS} Gifs failed to convert!": string.Empty;
			Console.WriteLine($"{ConvertedGIFs} GIFs converted!{failedGifText}\nPress any key to exit!");
			Console.ReadKey(true);
		}

		public static void TryCreateGif(string inputFilePath)
		{
			if (!File.Exists(inputFilePath))
			{
				FailedGIFS++;
				return;
			}

			try
			{
				// Read the data from the file.
				var compressedData = File.ReadAllBytes(inputFilePath);

				// Create two streams, one from the data, and one to be written to after decompression.
				using MemoryStream compressedStream = new(compressedData);
				using MemoryStream decompressedStream = new();

				// Decompress the data stream, copy it to the memory stream and get the decompressed data back.
				using GZipStream gZipStream = new(compressedStream, CompressionMode.Decompress);
				gZipStream.CopyTo(decompressedStream);
				var decompressedData = decompressedStream.GetBuffer();

				// Create a suitable new file name, and save the data to a new file.
				var outputFilePath = Environment.CurrentDirectory + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(inputFilePath) + "GIF.gif" ;
				File.WriteAllBytes(outputFilePath, decompressedData);
				ConvertedGIFs++;
			}
			catch
			{
				FailedGIFS++;
			}
		}
	}
}