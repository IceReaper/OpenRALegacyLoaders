using System;
using System.IO;
using System.Linq;
using System.Text;
using OpenRA.Graphics;
using OpenRA.Mods.Example.Games.Dominion.FileFormats;
using OpenRA.Primitives;

namespace OpenRA.Mods.Example.Games.Dominion.SpriteLoaders
{
	public class Tl2SpriteFrame : ISpriteFrame
	{
		public SpriteFrameType Type { get; }
		public Size Size { get; }
		public Size FrameSize { get; }
		public float2 Offset { get; }
		public byte[] Data { get; }
		public bool DisableExportPadding => true;

		public Tl2SpriteFrame(byte[] pixels)
		{
			Type = SpriteFrameType.Rgba32;
			FrameSize = Size = new Size(Tl2Tile.Width, Tl2Tile.Height);
			Offset = new float2(0, 0);
			Data = pixels;
		}
	}

	public class Tl2SpriteLoader : ISpriteLoader
	{
		public bool TryParseSprite(Stream s, string filename, out ISpriteFrame[] frames, out TypeDictionary metadata)
		{
			frames = null;
			metadata = null;

			if (!filename.EndsWith(".tl2", StringComparison.OrdinalIgnoreCase))
				return false;

			var test = Encoding.ASCII.GetString(s.ReadBytes(3));
			s.Position -= 3;

			if (test != "TL2")
				return false;

			var tl2 = new Tl2(s);

			frames = new ISpriteFrame[tl2.Tiles.Length];
			var palette = new Color[256];



			foreach (var package in Game.ModData.ModFiles.MountedPackages)
			{
				if (!package.Name.EndsWith("DOMINION.000"))
					continue;

				palette = new Pal(package.GetStream("global.pal")).Colors;
				break;
			}

			for (var i = 0; i < tl2.Tiles.Length; i++)
			{
				frames[i] = new Tl2SpriteFrame(tl2.Tiles[i].Pixels.SelectMany(i =>
				{
					var color = palette[i];

					return new[] { color.R, color.G, color.B, color.A };
				}).ToArray());
			}

			return true;
		}
	}
}
