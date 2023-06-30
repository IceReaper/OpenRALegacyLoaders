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
using System.Text;

namespace OpenRA.Mods.Example.Games.Dominion.FileFormats
{
	public class Tl2
	{
		public readonly Tl2Tile[] Tiles;

		public Tl2(Stream stream)
		{
			var magic = Encoding.ASCII.GetString(stream.ReadBytes(6)).TrimEnd('\0');

			if (magic != "TL2")
				throw new Exception("TL2: Invalid header!");

			Tiles = new Tl2Tile[stream.ReadUInt32()];

			if (stream.Length != stream.ReadUInt32() + 18)
				throw new Exception("TL2: Invalid file size!");

			if (stream.Position != stream.ReadUInt32() - 4)
				throw new Exception("TL2: Invalid tile offset!");

			for (var i = 0; i < Tiles.Length; i++)
				Tiles[i] = new Tl2Tile(stream);

			if (stream.Position != stream.Length)
				throw new Exception("TL2: Missing data!");
		}
	}
}
