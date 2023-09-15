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
using OpenRA.Graphics;
using OpenRA.Mods.Example.Games.BloodAndMagic.FileFormats;
using OpenRA.Primitives;

namespace OpenRA.Mods.Example.Games.BloodAndMagic.SpriteLoaders
{
	public class AniSpriteLoader : ISpriteLoader
	{
		sealed class AniSpriteFrame : ISpriteFrame
		{
			public SpriteFrameType Type => SpriteFrameType.Rgba32;
			public Size Size { get; }
			public Size FrameSize { get; }
			public float2 Offset { get; }
			public byte[] Data { get; }

			public bool DisableExportPadding => true;

			public AniSpriteFrame(AniFrame frame, Pal palette)
			{
				var width = frame.Width;
				var height = frame.Height;

				Size = new Size(width, height);
				FrameSize = new Size(width, height);
				Offset = new float2(width / 2f - frame.X, height / 2f - frame.Y);

				Data = frame.Pixels.SelectMany(pixel =>
				{
					var color = palette.Colors[pixel];

					return new[] { color.R, color.G, color.B, color.A };
				}).ToArray();
			}
		}

		public bool TryParseSprite(Stream stream, string filename, out ISpriteFrame[] frames,
			out TypeDictionary metadata)
		{
			var position = stream.Position;
			var valid = stream.Length > 12;

			if (stream is not Stf.StfStream stfStream)
			{
				frames = null;
				metadata = null;

				return false;
			}

			if (valid)
			{
				stream.Position += 10;
				var magic = stream.ReadUInt16();
				stream.Position = position;

				valid = magic == 0xfefe;
			}

			if (!valid)
			{
				frames = null;
				metadata = null;

				return false;
			}

			var aniAnimation = new Ani(stfStream);

			/*
			 * 32 33 100 190 195 666 3535 3560 3602 3620 3660 3678 3710 3730 3770 3795 3820 8000 8050 8150 8200 9020
			 * 9100 9101 9120 9140 9160 9180 9200 9220 9240 9260 9280 9300 9320 9340 9360 9380 9400 9420 9998 9999 11195
			 */

			// TODO map the right palette!
			var palette = new Pal(stfStream.Container.GetStream("32.pal"));

			frames = aniAnimation.Frames.Select(frame => new AniSpriteFrame(frame, palette)).ToArray<ISpriteFrame>();
			metadata = null;

			return true;
		}
	}
}
