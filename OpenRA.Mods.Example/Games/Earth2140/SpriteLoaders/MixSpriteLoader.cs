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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OpenRA.Graphics;
using OpenRA.Mods.Example.Games.Earth2140.FileFormats;
using OpenRA.Primitives;

namespace OpenRA.Mods.Example.Games.Earth2140.SpriteLoaders
{
	public class MixSpriteFrame : ISpriteFrame
	{
		public SpriteFrameType Type => SpriteFrameType.Rgba32;
		public Size Size { get; }
		public Size FrameSize { get; }
		public float2 Offset { get; }
		public byte[] Data { get; }
		public bool DisableExportPadding => true;

		public MixSpriteFrame(Size size, byte[] pixels)
		{
			Size = size;
			FrameSize = size;
			Offset = new float2(0, 0);
			Data = pixels;
		}
	}

	public class MixSpriteLoader : ISpriteLoader
	{
		public bool TryParseSprite(Stream s, string filename, out ISpriteFrame[] frames, out TypeDictionary metadata)
		{
			var start = s.Position;
			var identifier = s.ReadASCII(10);
			s.Position = start;

			frames = null;
			metadata = null;

			var framesList = new List<ISpriteFrame>();

			if (identifier != "MIX FILE  ")
			{
				if (s is Wd.WdStream && filename.Contains("mixmax", StringComparison.OrdinalIgnoreCase))
					framesList.AddRange(new MixMax(s).Frames.Select(f => new MixSpriteFrame(f.Size, f.Pixels)));
				else
					return false;
			}
			else
			{
				var mix = new Mix(s);

				if (mix.Frames.Length == 0)
					return false;

				foreach (var frame in mix.Frames)
				{
					var size = new Size(frame.Width, frame.Height);

					if (frame.Is32Bpp)
						framesList.Add(new MixSpriteFrame(size, frame.Pixels));
					else
					{
						var palette = mix.Palettes[frame.Palette].Colors;

						var argbImage = frame.Pixels.SelectMany(pixel =>
						{
							var color = palette[pixel];

							return new[] { color.R, color.G, color.B, color.A };
						}).ToArray();

						framesList.Add(new MixSpriteFrame(size, argbImage));
					}
				}
			}

			frames = framesList.ToArray();

			return true;
		}
	}
}
