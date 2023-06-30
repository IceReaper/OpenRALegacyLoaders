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

using System.IO;
using System.Linq;
using System.Text;
using OpenRA.Graphics;
using OpenRA.Mods.Example.Games.WarWind.FileFormats;
using OpenRA.Primitives;

namespace OpenRA.Mods.Example.Games.WarWind.SpriteLoaders
{
	public class D3GrSpriteLoader : ISpriteLoader
	{
		sealed class D3GrSpriteFrame : ISpriteFrame
		{
			public SpriteFrameType Type => SpriteFrameType.Rgba32;
			public Size Size { get; }
			public Size FrameSize { get; }
			public float2 Offset { get; }
			public byte[] Data { get; }
			public bool DisableExportPadding => true;

			public D3GrSpriteFrame(Color[] palette, D3GrFrame frame)
			{
				FrameSize = Size = new Size(frame.Width, frame.Height);
				Offset = new int2(frame.Width / 2 + frame.OffsetX, frame.Height / 2 + frame.OffsetY);
				Data = frame.Pixels.SelectMany(pixel =>
				{
					var color = palette[pixel];

					return new[] { color.R, color.G, color.B, color.A };
				}).ToArray();
			}
		}

		public bool TryParseSprite(Stream s, string filename, out ISpriteFrame[] frames, out TypeDictionary metadata)
		{
			var valid = s.Length > 4;

			if (valid)
			{
				valid = Encoding.ASCII.GetString(s.ReadBytes(4)) == "D3GR";
				s.Position -= 4;
			}

			if (!valid || s is not SegmentStream segmentStream || segmentStream.BaseStream is not FileStream fileStream)
			{
				metadata = null;
				frames = null;
				return false;
			}

			metadata = null;

			foreach (var package in Game.ModData.ModFiles.MountedPackages)
			{
				if (package.Name != fileStream.Name[..^3] + "001")
					continue;

				// TODO Find proper palette mappings - not all use palette 0 !
				var sprite = new D3Gr(s);
				var palette = sprite.Palette != null ? sprite : new D3Gr(package.GetStream("0.pal"));
				frames = sprite.Frames.Select(frame => new D3GrSpriteFrame(palette.Palette, frame))
					.ToArray<ISpriteFrame>();

				return true;
			}

			frames = null;
			return false;
		}
	}
}
