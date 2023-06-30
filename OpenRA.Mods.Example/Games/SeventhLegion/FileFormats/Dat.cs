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

namespace OpenRA.Mods.Example.Games.SeventhLegion.FileFormats
{
	public class Dat
	{
		public readonly uint[][] Colors;

		public Dat(Stream stream)
		{
			Colors = new uint[stream.Length / 256 / 3][];

			for (var i = 0; i < Colors.Length; i++)
			{
				var palette = new uint[256];
				Colors[i] = palette;

				for (var j = 0; j < palette.Length; j++)
					palette[j] = (uint)((0xff << 24) | (((stream.ReadUInt8() << 16) | (stream.ReadUInt8() << 8) | (stream.ReadUInt8() << 0)) << 2));
			}
		}
	}
}
