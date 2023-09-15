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
using System.Text;
using OpenRA.Primitives;

namespace OpenRA.Mods.Example.Games.Z.FileFormats
{
	public class Lbm
	{
		public readonly ushort Width;
		public readonly ushort Height;
		public readonly Color[] Palette;
		public readonly byte[] Pixels;

		public Lbm(Stream stream)
		{
			var magic = Encoding.ASCII.GetString(stream.ReadBytes(4));
			var size = BitConverter.ToUInt32(stream.ReadBytes(4).Reverse().ToArray());

			if (magic != "FORM" || size != stream.Length - stream.Position)
				throw new Exception();

			var type = Encoding.ASCII.GetString(stream.ReadBytes(4));

			if (type != "PBM " && type != "ILBM")
				throw new Exception();

			var headerMagic = Encoding.ASCII.GetString(stream.ReadBytes(4));
			var headerSize = BitConverter.ToUInt32(stream.ReadBytes(4).Reverse().ToArray());

			if (headerMagic != "BMHD" || headerSize != 20)
				throw new Exception();

			Width = BitConverter.ToUInt16(stream.ReadBytes(2).Reverse().ToArray());
			Height = BitConverter.ToUInt16(stream.ReadBytes(2).Reverse().ToArray());
			var offsetX = BitConverter.ToInt16(stream.ReadBytes(2).Reverse().ToArray());
			var offsetY = BitConverter.ToInt16(stream.ReadBytes(2).Reverse().ToArray());
			var numPlanes = stream.ReadUInt8();
			var mask = stream.ReadUInt8();
			var compression = stream.ReadUInt8();
			var pad1 = stream.ReadUInt8();
			BitConverter.ToUInt16(stream.ReadBytes(2).Reverse().ToArray()); // transClr
			stream.ReadUInt8(); // aspectX
			stream.ReadUInt8(); // aspectY
			var pageWidth = BitConverter.ToUInt16(stream.ReadBytes(2).Reverse().ToArray());
			var pageHeight = BitConverter.ToUInt16(stream.ReadBytes(2).Reverse().ToArray());

			if (offsetX != 0 || offsetY != 0 || mask != 0 || pad1 != 0 || pageWidth != 320 || pageHeight != 200)
				throw new Exception();

			var paletteMagic = Encoding.ASCII.GetString(stream.ReadBytes(4));
			var paletteSize = BitConverter.ToUInt32(stream.ReadBytes(4).Reverse().ToArray());

			if (paletteMagic != "CMAP")
				throw new Exception();

			Palette = new Color[paletteSize / 3];

			for (var i = 0; i < Palette.Length; i++)
				Palette[i] = Color.FromArgb(stream.ReadUInt8(), stream.ReadUInt8(), stream.ReadUInt8());

			while (true)
			{
				var chunkMagic = Encoding.ASCII.GetString(stream.ReadBytes(4));
				var chunkSize = BitConverter.ToUInt32(stream.ReadBytes(4).Reverse().ToArray());

				if (chunkMagic == "CRNG")
				{
					// The colour range chunk is 'nonstandard'.
					// It is used by Electronic Arts' Deluxe Paint program to
					// identify a contiguous range of colour registers or a
					// "shade range" and colour cycling.
				}
				else if (chunkMagic == "TINY")
				{
					// The TINY chunk contains a small preview image for various
					// graphics programs, including Deluxe Paint.
				}
				else if (chunkMagic == "DPPS")
				{
					// TODO Undocumented chunk!
				}
				else if (chunkMagic == "ANNO")
				{
					// Annotations.
				}
				else if (chunkMagic == "BODY")
				{
					stream.Position -= 8;

					break;
				}
				else
					throw new Exception(chunkMagic);

				stream.Position += chunkSize + chunkSize % 2;
			}

			var bodyMagic = Encoding.ASCII.GetString(stream.ReadBytes(4));
			var bodySize = BitConverter.ToUInt32(stream.ReadBytes(4).Reverse().ToArray());

			if (bodyMagic != "BODY")
				throw new Exception();

			Pixels = new byte[Width * Height];

			if (compression == 1)
			{
				for (var i = 0; i < Pixels.Length;)
				{
					var count = (sbyte)stream.ReadUInt8();

					if (count < 0)
					{
						var value = stream.ReadUInt8();

						for (var j = 0; j < -count + 1; j++)
							Pixels[i++] = value;
					}
					else
					{
						for (var j = 0; j <= count; j++)
							Pixels[i++] = stream.ReadUInt8();
					}
				}
			}
			else
			{
				var bytesPerRow = 2 * ((Width + 15) / 16);
				var bytes = stream.ReadBytes((int)bodySize);

				for (var y = 0; y < Height; y++)
				for (var plane = 0; plane < numPlanes; plane++)
				for (var x = 0; x < Width; x++)
				{
					var bits = bytes[(plane + numPlanes * y) * bytesPerRow + x / 8];
					var bit = (bits >> (7 - x % 8)) & 0x01;
					Pixels[y * Width + x] |= (byte)(bit << plane);
				}
			}
		}
	}
}
