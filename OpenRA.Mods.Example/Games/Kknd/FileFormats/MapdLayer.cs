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

namespace OpenRA.Mods.Example.Games.Kknd.FileFormats
{
	public class MapdLayer
	{
		public readonly uint TileWidth;
		public readonly uint TileHeight;
		public readonly uint TilesX;
		public readonly uint TilesY;

		public readonly uint[] Tiles;
		public readonly byte[] TileAttributes;

		public MapdLayer(Stream stream, AssetFormat assetFormat)
		{
			TileWidth = stream.ReadUInt32();
			TileHeight = stream.ReadUInt32();
			TilesX = stream.ReadUInt32();
			TilesY = stream.ReadUInt32();

			if (TileWidth != 32 || TileHeight != 32)
				throw new Exception("MAPD: Tile size broken");

			if (assetFormat == AssetFormat.Kknd2)
			{
				var mapWidth = stream.ReadUInt32();
				var mapHeight = stream.ReadUInt32();

				stream.ReadUInt32(); // TODO Unk

				if (mapWidth != TilesX * TileWidth)
					throw new Exception("MAPD: Map dimension broken");

				if (mapHeight != TilesY * TileHeight)
					throw new Exception("MAPD: Map dimension broken");
			}

			Tiles = new uint[TilesX * TilesY];

			for (var y = 0; y < TilesY; y++)
			for (var x = 0; x < TilesX; x++)
			{
				var tile = OffsetUtils.ReadUInt32Offset(stream);

				if (assetFormat == AssetFormat.Kknd2)
					tile -= tile % 4;

				Tiles[y * TilesX + x] = tile;
			}

			if (assetFormat == AssetFormat.Kknd2)
				TileAttributes = stream.ReadBytes((int)(TilesX * TilesY));
		}
	}
}
