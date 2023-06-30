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

namespace OpenRA.Mods.Example.Games.Kknd.FileFormats
{
	public class Blit
	{
		public const string Type = "BLIT";

		public readonly BlitFrame[] Images;
		public readonly Color[] Palette;

		public Blit(Stream stream)
		{
			var numImages = stream.ReadUInt32();
			var frameOffsets = new uint[numImages];

			if (stream.ReadUInt32() != uint.MaxValue)
				throw new Exception("BLIT: Unexpected value");

			var paletteOffset = OffsetUtils.ReadUInt32Offset(stream);
			var type = Encoding.ASCII.GetString(stream.ReadBytes(4).Reverse().ToArray());

			if (type != "BLT8")
				throw new Exception("BLIT: Unknown type");

			for (var i = 0; i < numImages; i++)
				frameOffsets[i] = OffsetUtils.ReadUInt32Offset(stream);

			Images = new BlitFrame[frameOffsets.Length];

			for (var i = 0; i < Images.Length; i++)
			{
				if (stream.Position != frameOffsets[i])
					throw new Exception("BLIT: Broken image offset");

				Images[i] = new BlitFrame(stream);
			}

			if (stream.Position != paletteOffset)
				throw new Exception("BLIT: Broken palette offset");

			Palette = new Color[256];

			for (var i = 0; i < Palette.Length; i++)
			{
				var color16 = stream.ReadUInt16();
				var r = ((color16 & 0x7c00) >> 7) & 0xff;
				var g = ((color16 & 0x03e0) >> 2) & 0xff;
				var b = ((color16 & 0x001f) << 3) & 0xff;
				Palette[i] = Color.FromArgb(i == 0 ? 0x00 : 0xff, r, g, b);
			}

			var unk1 = stream.ReadInt32();
			var unk2 = stream.ReadInt32();
			var unk3 = stream.ReadInt32();

			if (unk1 != int.MinValue || unk2 != int.MinValue || unk3 != int.MinValue)
				throw new Exception("BLIT: Unknown data!");

			if (stream.Position < stream.Length)
				throw new Exception("BLIT: Missing data!");
		}
	}
}
