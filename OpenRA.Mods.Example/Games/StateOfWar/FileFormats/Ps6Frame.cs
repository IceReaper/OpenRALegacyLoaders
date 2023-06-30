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

namespace OpenRA.Mods.Example.Games.StateOfWar.FileFormats
{
	public class Ps6Frame
	{
		public readonly int Width;
		public readonly int Height;
		public readonly int OriginX;
		public readonly int OriginY;
		public readonly byte[] Pixels;

		public Ps6Frame(Stream stream)
		{
			Width = stream.ReadInt16();
			Height = stream.ReadInt16();
			OriginX = stream.ReadInt16();
			OriginY = stream.ReadInt16();
			stream.ReadInt16(); // TODO
			stream.ReadInt16(); // TODO
			stream.ReadInt16(); // TODO
			stream.ReadInt16(); // TODO

			if (Width <= 0 || Height <= 0)
				return;

			var lineOffsets = new int[Height];

			for (var i = 0; i < Height; i++)
				lineOffsets[i] = stream.ReadInt32() * 2;

			Pixels = new byte[Width * Height * 4];

			for (var i = 0; i < Height; i++)
			{
				stream.Position = lineOffsets[i];

				var numCommand = stream.ReadInt16();
				var skipMode = stream.ReadInt16() == 0x00;
				var x = 0;

				for (var j = 0; j < numCommand; j++)
				{
					if (skipMode)
						x += stream.ReadInt16();
					else
					{
						var readPixels = stream.ReadInt16();

						for (var k = 0; k < readPixels; k++)
						{
							var color16 = stream.ReadUInt16(); // RRRRRGGGGGGBBBBB
							Pixels[(x + i * Width) * 4 + 0] = (byte)((color16 & 0xf800) >> 8);
							Pixels[(x + i * Width) * 4 + 1] = (byte)((color16 & 0x07e0) >> 3);
							Pixels[(x + i * Width) * 4 + 2] = (byte)((color16 & 0x001f) << 3);
							Pixels[(x + i * Width) * 4 + 3] = 0xff;
							x++;
						}
					}

					skipMode = !skipMode;
				}
			}
		}
	}
}
