using System;
using System.IO;
using System.Linq;
using OpenRA.Graphics;
using OpenRA.Mods.Example.Games.Dominion.FileFormats;
using OpenRA.Primitives;

namespace OpenRA.Mods.Example.Games.Dominion.SpriteLoaders
{
	public class SprSpriteFrame : ISpriteFrame
	{
		public SpriteFrameType Type { get; }
		public Size Size { get; }
		public Size FrameSize { get; }
		public float2 Offset { get; }
		public byte[] Data { get; }
		public bool DisableExportPadding => true;

		public SprSpriteFrame(SprFrame frame, Pal palette, int2 offset)
		{
			var pixels = frame.Pixels.SelectMany(i =>
			{
				var color = palette.Colors[i];

				return new[] { color.R, color.G, color.B, color.A };
			}).ToArray();

			Type = SpriteFrameType.Rgba32;
			FrameSize = Size = new Size((int)frame.Width, (int)frame.Height);
			Offset = new float2(offset.X, offset.Y);
			Data = pixels;
		}
	}

	public class SprSpriteLoader : ISpriteLoader
	{
		public bool TryParseSprite(Stream s, string filename, out ISpriteFrame[] frames, out TypeDictionary metadata)
		{
			frames = null;
			metadata = null;

			if (s is not ZeroBin.ZeroBinStream stream || !filename.EndsWith(".spr", StringComparison.OrdinalIgnoreCase))
				return false;

			var spr = new Spr(stream, uint.Parse(filename.Split("|").Last()[..^4]));

			frames = new ISpriteFrame[spr.Frames.Length];

			for (var i = 0; i < spr.Frames.Length; i++)
				frames[i] = new SprSpriteFrame(spr.Frames[i], spr.Palette, spr.Offsets[i]);

			return true;
		}
	}
}
