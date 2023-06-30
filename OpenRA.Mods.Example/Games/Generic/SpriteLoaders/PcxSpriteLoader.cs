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
using OpenRA.Mods.Example.Games.Generic.FileFormats;
using OpenRA.Primitives;

namespace OpenRA.Mods.Example.Games.Generic.SpriteLoaders
{
	public class PcxSpriteFrame : ISpriteFrame
	{
		public SpriteFrameType Type => SpriteFrameType.Rgba32;
		public Size Size { get; }
		public Size FrameSize { get; }
		public float2 Offset { get; }
		public byte[] Data { get; }
		public bool DisableExportPadding => true;

		public PcxSpriteFrame(Size size, byte[] pixels)
		{
			Size = size;
			FrameSize = size;
			Offset = new float2(0, 0);
			Data = pixels;
		}
	}

	public class PcxSpriteLoader : ISpriteLoader
	{
		public bool TryParseSprite(Stream s, string filename, out ISpriteFrame[] frames, out TypeDictionary metadata)
		{
			frames = null;
			metadata = null;

			if (!filename.EndsWith(".pcx", StringComparison.OrdinalIgnoreCase))
				return false;

			var test = s.ReadUInt16();
			s.Position -= 2;

			if (test != 0x050a)
				return false;

			var pcx = new Pcx(s);
			var size = new Size(pcx.Width, pcx.Height);

			frames = new ISpriteFrame[]
			{
				new PcxSpriteFrame(size,
					pcx.Pixels.SelectMany(color => new[] { color.R, color.G, color.B, color.A }).ToArray())
			};

			return true;
		}
	}
}
