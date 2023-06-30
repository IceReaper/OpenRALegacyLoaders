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

namespace OpenRA.Mods.Example.Games.WarWind.FileFormats
{
	public class D3Gr
	{
		public readonly D3GrFrame[] Frames;
		public readonly int2[] Hotspots;
		public readonly Color[] Palette;

		public D3Gr(Stream stream)
		{
			if (stream.ReadASCII(4) != "D3GR")
				throw new Exception("D3GR: Broken Format!");

			stream.ReadInt32(); // TODO flags.
			var framesStart = stream.ReadInt32();
			var paletteStart = stream.ReadInt32();
			var unkStart = stream.ReadInt32();

			if (stream.ReadInt32() != 0)
				throw new Exception("D3GR: Broken Format!");

			Frames = new D3GrFrame[stream.ReadInt16()];
			stream.ReadInt16(); // TODO 0 or 320?

			var offsets = new int[Frames.Length];

			for (var i = 0; i < offsets.Length; i++)
				offsets[i] = stream.ReadInt32();

			if (unkStart != 0)
			{
				if (stream.Position != unkStart)
					throw new Exception("D3GR: Broken Format!");

				Hotspots = new int2[Frames.Length];

				for (var i = 0; i < Hotspots.Length; i++)
					Hotspots[i] = new int2(stream.ReadUInt8(), stream.ReadUInt8());
			}

			if (paletteStart != 0)
			{
				if (stream.Position != paletteStart)
					throw new Exception("D3GR: Broken Format!");

				stream.Position = paletteStart;

				if (stream.ReadInt16() != 256 || stream.ReadInt16() != 256)
					throw new Exception("D3GR: Broken Format!");

				Palette = new Color[256];

				for (var i = 0; i < Palette.Length; i++)
				{
					var r = stream.ReadUInt8() << 2;
					var g = stream.ReadUInt8() << 2;
					var b = stream.ReadUInt8() << 2;

					Palette[i] = Color.FromArgb(i == 0 ? 0x00 : 0xff, r, g, b);
				}
			}

			for (var i = 0; i < offsets.Length; i++)
			{
				if (stream.Position != framesStart + offsets[i])
					throw new Exception("D3GR: Broken Format!");

				Frames[i] = new D3GrFrame(stream);
			}

			if (stream.Position != stream.Length)
				throw new Exception("D3GR: Broken Format!");
		}
	}
}
