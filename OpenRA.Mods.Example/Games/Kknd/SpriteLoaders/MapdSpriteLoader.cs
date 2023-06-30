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
using OpenRA.Mods.Example.Games.Kknd.FileFormats;
using OpenRA.Primitives;

namespace OpenRA.Mods.Example.Games.Kknd.SpriteLoaders
{
	public class MapdSpriteLoader : ISpriteLoader
	{
		sealed class MapdSpriteFrame : ISpriteFrame
		{
			public SpriteFrameType Type => SpriteFrameType.Rgba32;

			public Size Size { get; }

			public Size FrameSize { get; }

			public float2 Offset => float2.Zero;

			public byte[] Data { get; }

			public bool DisableExportPadding => true;

			public MapdSpriteFrame(Mapd map, MapdLayer layer)
			{
				FrameSize = Size = new Size(
					(int)(layer.TilesX * layer.TileWidth),
					(int)(layer.TilesY * layer.TileHeight));
				Data = new byte[FrameSize.Width * FrameSize.Height * 4];

				for (var tileX = 0; tileX < layer.TilesX; tileX++)
				for (var tileY = 0; tileY < layer.TilesY; tileY++)
				{
					var tileId = layer.Tiles[tileY * layer.TilesX + tileX];

					if (tileId == 0)
						continue;

					var tile = map.Tiles[tileId];

					for (var y = 0; y < layer.TileHeight; y++)
					{
						var row = tile
							.Skip((int)(y * layer.TileWidth))
							.Take((int)layer.TileWidth)
							.SelectMany(pixel =>
							{
								return new[]
								{
									map.Palette[pixel].R,
									map.Palette[pixel].G,
									map.Palette[pixel].B,
									map.Palette[pixel].A
								};
							}).ToArray();

						Array.Copy(
							row,
							0,
							Data,
							((tileY * layer.TileHeight + y) * Size.Width + tileX * layer.TileWidth) * 4,
							row.Length);
					}
				}
			}
		}

		public bool TryParseSprite(Stream s, string filename, out ISpriteFrame[] frames, out TypeDictionary metadata)
		{
			metadata = null;

			if (!filename.EndsWith($".{Mapd.Type}") || s is not Lvl.LvlStream lvlStream)
			{
				frames = null;

				return false;
			}

			// This is damn ugly, but MAPD uses offsets from LVL start.
			lvlStream.BaseStream.Position = lvlStream.BaseOffset;
			var map = new Mapd(lvlStream, lvlStream.AssetFormat);
			frames = map.Layers.Select(layer => new MapdSpriteFrame(map, layer) as ISpriteFrame).ToArray();

			return true;
		}
	}
}
