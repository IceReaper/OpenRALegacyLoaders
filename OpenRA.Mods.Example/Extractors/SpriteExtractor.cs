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
using OpenRA.FileFormats;
using OpenRA.Graphics;
using OpenRA.Primitives;

namespace OpenRA.Mods.Example.Extractors
{
	public static class SpriteExtractor
	{
		public static void Extract(IEnumerable<Sprite> sprites, string name)
		{
			var outputFile = Path.Combine("Extracted", $"{name}.png");

			Directory.CreateDirectory(Path.GetDirectoryName(outputFile) ?? string.Empty);

			var sheetBaker = new SheetBaker(4);

			foreach (var sprite in sprites)
			{
				var sheetData = sprite.Sheet.GetData();
				var frame = new byte[sprite.Bounds.Width * sprite.Bounds.Height * 4];

				for (var y = 0; y < sprite.Bounds.Height; y++)
				{
					var sourceIndex = ((sprite.Bounds.Y + y) * sprite.Sheet.Size.Width + sprite.Bounds.X) * 4;
					var targetIndex = y * sprite.Bounds.Width * 4;
					var length = sprite.Bounds.Width * 4;

					Array.Copy(sheetData, sourceIndex, frame, targetIndex, length);
				}

				var offset = new int2((int)sprite.Offset.X, (int)sprite.Offset.Y);
				var size = new Size(sprite.Bounds.Width, sprite.Bounds.Height);

				sheetBaker.Add(new Rectangle(offset, size), frame);
			}

			var data = sheetBaker.Bake(out var width, out var height, out var offsetX, out var offsetY);
			var embeddedData = new Dictionary<string, string> { { "Offset", $"{offsetX},{offsetY}" } };

			new Png(data, SpriteFrameType.Bgra32, width, height, null, embeddedData).Save(outputFile);
		}
	}
}
