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

namespace OpenRA.Mods.Example.Games.Z.FileFormats
{
	public class ExpSpriteFrame
	{
		public readonly byte Width;
		public readonly byte Height;
		public readonly byte[] Pixels;

		public ExpSpriteFrame(Stream reader, long end)
		{
			Height = reader.ReadUInt8();
			var stride = reader.ReadUInt8();
			Width = (byte)(stride * 8);

			// TODO there is some data here!
			// TODO this is a hack! Properly parse data, so we dont need this anymore!
			reader.Position = end - Width * Height;

			Pixels = new byte[Width * Height];

			for (var tileY = 0; tileY < 4; tileY++)
			for (var y = 0; y < Height; y++)
			for (var tileX = 0; tileX < 2; tileX++)
			for (var x = 0; x < stride; x++)
				Pixels[tileY + y * Width + tileX * stride * 4 + x * 4] = reader.ReadUInt8();
		}
	}
}
