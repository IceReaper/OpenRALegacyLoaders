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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using OpenRA.Primitives;

namespace OpenRA.Mods.Example.Games.Kknd.FileFormats
{
	public class Mapd
	{
		public const string Type = "MAPD";

		public readonly Color[] Palette;
		public readonly MapdLayer[] Layers;
		public readonly Dictionary<uint, byte[]> Tiles = new();

		public Mapd(Stream stream, AssetFormat assetFormat)
		{
			if (assetFormat == AssetFormat.Kknd2)
			{
				var unk = stream.ReadUInt32();

				if (unk != 1)
					throw new Exception("MAPD: Unknown value");
			}

			var layers = stream.ReadUInt32();
			var layerOffsets = new uint[layers];

			for (var i = 0; i < layers; i++)
				layerOffsets[i] = OffsetUtils.ReadUInt32Offset(stream);

			var numColors = stream.ReadUInt32();
			Palette = new Color[numColors];

			for (var i = 0; i < numColors; i++)
			{
				if (assetFormat == AssetFormat.Kknd1)
				{
					var r = stream.ReadUInt8();
					var g = stream.ReadUInt8();
					var b = stream.ReadUInt8();
					stream.ReadUInt8(); // unused alpha
					Palette[i] = Color.FromArgb(i == 0 ? 0x00 : 0xff, r, g, b);
				}
				else
				{
					var color16 = stream.ReadUInt16();
					var r = ((color16 & 0x7c00) >> 7) & 0xff;
					var g = ((color16 & 0x03e0) >> 2) & 0xff;
					var b = ((color16 & 0x001f) << 3) & 0xff;
					Palette[i] = Color.FromArgb(i == 0 ? 0x00 : 0xff, r, g, b);
				}
			}

			Layers = new MapdLayer[layerOffsets.Length];

			for (var i = 0; i < layerOffsets.Length; i++)
			{
				if (stream.Position != layerOffsets[i])
					throw new Exception("MAPD: Wrong offset!");

				var type = Encoding.ASCII.GetString(stream.ReadBytes(4).Reverse().ToArray());

				if (type != "SCRL")
					throw new Exception("MAPD: Unknown type");

				Layers[i] = new MapdLayer(stream, assetFormat);
			}

			while (stream.Position < stream.Length)
			{
				var offset = (uint)stream.Position;

				if (assetFormat == AssetFormat.Kknd1)
					stream.ReadUInt32(); // TODO Unk 0 or 1 - this could mean color 0x00 must be transparent?

				Tiles[offset] = stream.ReadBytes(32 * 32);
			}

			if (stream.Position < stream.Length)
				throw new Exception("MAPD: Missing data!");
		}
	}
}
