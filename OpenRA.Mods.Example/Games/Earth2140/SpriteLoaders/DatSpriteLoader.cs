#region Copyright & License Information

/*
 * Copyright (c) The OpenRA Developers and Contributors
 * This file is part of OpenRA, which is free software. It is made
 * available to you under the terms of the GNU General Public License
 * as published by the Free Software Foundation, either version 3 of
 * the License, or (at your option) any later version. For more
 * information, see COPYING.
 */

#endregion

using System;
using System.IO;
using System.Linq;
using OpenRA.Graphics;
using OpenRA.Mods.Example.Games.Earth2140.FileFormats;
using OpenRA.Primitives;

namespace OpenRA.Mods.Example.Games.Earth2140.SpriteLoaders
{
	public class DatSpriteFrame : ISpriteFrame
	{
		public SpriteFrameType Type { get; }
		public Size Size { get; }
		public Size FrameSize { get; }
		public float2 Offset { get; }
		public byte[] Data { get; }
		public bool DisableExportPadding => true;

		public DatSpriteFrame(SpriteFrameType type, Size size, byte[] pixels)
		{
			Type = type;
			Size = size;
			FrameSize = size;
			Offset = new float2(0, 0);
			Data = pixels;
		}
	}

	public class DatSpriteLoader : ISpriteLoader
	{
		public bool TryParseSprite(Stream s, string filename, out ISpriteFrame[] frames, out TypeDictionary metadata)
		{
			frames = null;
			metadata = null;

			if (s is not Wd.WdStream stream || !filename.EndsWith(".dat", StringComparison.OrdinalIgnoreCase))
				return false;

			var palPalette = new Pal(stream.Wd.GetStream(filename[..^4].Split('|').Last() + ".PAL"));
			var datImage = new Dat(s);

			var pixels = datImage.Pixels.SelectMany(i =>
			{
				var color = palPalette.Colors[i];

				return new[] { color.R, color.G, color.B, color.A };
			}).ToArray();

			frames = new ISpriteFrame[]
			{
				new DatSpriteFrame(SpriteFrameType.Rgba32, new Size(datImage.Width, datImage.Height), pixels)
			};

			return true;
		}
	}
}
