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
using OpenRA.Mods.Example.Games.Z.FileFormats;
using OpenRA.Primitives;

namespace OpenRA.Mods.Example.Games.Z.SpriteLoaders
{
	public class ExpSpriteLoader : ISpriteLoader
	{
		sealed class ExpSpriteFrame : ISpriteFrame
		{
			public SpriteFrameType Type => SpriteFrameType.Rgba32;
			public Size Size { get; }
			public Size FrameSize { get; }
			public float2 Offset { get; }
			public byte[] Data { get; }

			public bool DisableExportPadding => true;

			public ExpSpriteFrame(FileFormats.ExpSpriteFrame frame, Pal palette)
			{
				var width = frame.Width;
				var height = frame.Height;

				Size = new Size(width, height);
				FrameSize = new Size(width, height);
				Offset = new float2(width / 2f, height / 2f);

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

			try
			{
				var sprite = new ExpSprite(stream);

				frames = sprite.Frames.Select(frame => new ExpSpriteFrame(frame, sprite.Palette))
					.ToArray<ISpriteFrame>();

				metadata = null;

				return true;
			}
			catch
			{
				stream.Position = position;

				frames = null;
				metadata = null;

				return false;
			}
		}
	}
}
