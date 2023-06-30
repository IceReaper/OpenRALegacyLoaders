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
using OpenRA.Primitives;

namespace OpenRA.Mods.Example.Games.Generic.FileFormats
{
	public class Pcx
	{
		public readonly ushort X;
		public readonly ushort Y;
		public readonly ushort Width;
		public readonly ushort Height;
		public readonly Color[] Pixels;

		public Pcx(Stream stream)
		{
			if (stream.ReadUInt8() != 0x0a)
				throw new Exception("Broken pcx file!");

			var version = stream.ReadUInt8();
			var encoding = stream.ReadUInt8();
			var bpp = stream.ReadUInt8();

			if (version != 5 || encoding != 1 || bpp != 8)
				throw new Exception("Broken pcx file!");

			X = stream.ReadUInt16();
			Y = stream.ReadUInt16();
			Width = stream.ReadUInt16();
			Height = stream.ReadUInt16();
			Pixels = new Color[Width * Height];

			stream.Position += 52; // dpi, ega palette

			var reserved1 = stream.ReadUInt8();
			var channels = stream.ReadUInt8();
			var lineWidth = stream.ReadUInt16();
			var paletteType = stream.ReadUInt16();

			if (reserved1 != 0 || channels != 1 || (paletteType != 1 && paletteType != 2))
				throw new Exception("Broken pcx file!");

			stream.Position += 4; // resolution

			stream.ReadBytes(54); // Unk data!

			stream.Position = stream.Length - 768 - 1;

			if (stream.ReadUInt8() != 12)
				throw new Exception("Broken pcx file!");

			var palette = new Color[256];

			for (var i = 0; i < palette.Length; i++)
				palette[i] = Color.FromArgb(0xff, stream.ReadUInt8(), stream.ReadUInt8(), stream.ReadUInt8());

			stream.Position = 128;

			for (var y = 0; y < Height; y++)
			{
				for (var x = 0; x < lineWidth;)
				{
					var count = 1;
					var value = stream.ReadUInt8();

					if (value >> 6 == 0x3)
					{
						count = value & 0x3f;
						value = stream.ReadUInt8();
					}

					for (var i = 0; i < count; i++)
					{
						if (x < Width)
							Pixels[y * Width + x] = palette[value];

						x++;
					}
				}
			}
		}
	}
}
