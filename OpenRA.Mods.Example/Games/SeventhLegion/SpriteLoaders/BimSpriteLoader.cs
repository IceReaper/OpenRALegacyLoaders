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
using OpenRA.Mods.Example.Games.SeventhLegion.FileFormats;
using OpenRA.Primitives;

namespace OpenRA.Mods.Example.Games.SeventhLegion.SpriteLoaders
{
	public class BimSpriteLoader : ISpriteLoader
	{
		sealed class BimSpriteFrame : ISpriteFrame
		{
			public SpriteFrameType Type => SpriteFrameType.Rgba32;
			public Size Size { get; }
			public Size FrameSize { get; }
			public float2 Offset { get; }
			public byte[] Data { get; }

			public bool DisableExportPadding => true;

			public BimSpriteFrame(Col col, BimFrame bimFrame)
			{
				var width = bimFrame.Width;
				var height = bimFrame.Height;

				Size = new Size(width, height);
				FrameSize = new Size(width, height);
				Offset = new float2(width / 2f, height / 2f);

				Data = bimFrame.Pixels.SelectMany(pixel =>
				{
					var color = col.Colors[pixel];

					return new[] { color.R, color.G, color.B, color.A };
				}).ToArray();
			}
		}

		public bool TryParseSprite(Stream stream, string filename, out ISpriteFrame[] frames, out TypeDictionary metadata)
		{
			if (!filename.EndsWith(".bim", StringComparison.OrdinalIgnoreCase) || stream is not FileStream fileStream)
			{
				metadata = null;
				frames = null;

				return false;
			}

			// TODO verify and fix palette mappings!
			var col = new Col(File.OpenRead(Path.Combine(Path.GetDirectoryName(fileStream.Name) ?? "", "pal1.col")));

			frames = new Bim(stream).Frames.Select(frame => new BimSpriteFrame(col, frame)).ToArray<ISpriteFrame>();
			metadata = null;

			return true;
		}
	}
}
