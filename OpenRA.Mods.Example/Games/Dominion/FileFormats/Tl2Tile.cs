using System.IO;

namespace OpenRA.Mods.Example.Games.Dominion.FileFormats
{
	public class Tl2Tile
	{
		public const int Width = 64;
		public const int Height = 32;

		public readonly byte[] Pixels = new byte[Width * Height];

		public Tl2Tile(Stream stream)
		{
			stream.ReadInt32(); // 0 or -1, could be block information

			for (var y = 0; y < Height; y++)
			{
				var stride = (y < Height / 2 ? y + 1 : Height - y) * 4;
				var offset = (Width - stride) / 2;

				for (var i = 0; i < stride; i++)
					Pixels[y * Width + offset + i] = stream.ReadUInt8();
			}
		}
	}
}
