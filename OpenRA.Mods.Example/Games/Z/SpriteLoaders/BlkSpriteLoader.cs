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
using OpenRA.Mods.Example.Games.Z.FileFormats;
using OpenRA.Primitives;

namespace OpenRA.Mods.Example.Games.Z.SpriteLoaders
{
	public class BlkSpriteLoader : ISpriteLoader
	{
		sealed class BlkSpriteFrame : ISpriteFrame
		{
			public SpriteFrameType Type => SpriteFrameType.Rgba32;
			public Size Size { get; }
			public Size FrameSize { get; }
			public float2 Offset { get; }
			public byte[] Data { get; }

			public bool DisableExportPadding => true;

			public BlkSpriteFrame(Blk blk, int id, Pal palette)
			{
				var width = blk.Width;
				var height = blk.Height;

				Size = new Size(width, height);
				FrameSize = new Size(width, height);
				Offset = new float2(width / 2f, height / 2f);

				Data = blk.Frames[id].SelectMany(pixel =>
				{
					var color = palette.Colors[pixel];

					return new[] { color.R, color.G, color.B, color.A };
				}).ToArray();
			}
		}

		public bool TryParseSprite(Stream stream, string filename, out ISpriteFrame[] frames,
			out TypeDictionary metadata)
		{
			if (stream is not ExpContainer.ExpStream expStream ||
			    !filename.EndsWith(".blk", StringComparison.OrdinalIgnoreCase))
			{
				metadata = null;
				frames = null;

				return false;
			}

			var palette = new Pal(
				expStream.Container.GetStream($"{Path.GetFileNameWithoutExtension(filename).Split('|')[1]}.PAL"));

			var sprite = new Blk(stream);
			frames = Enumerable.Range(0, sprite.Frames.Length)
				.Select(id => new BlkSpriteFrame(sprite, id, palette)).ToArray<ISpriteFrame>();
			metadata = null;

			return true;
		}
	}
}
