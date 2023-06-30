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

namespace OpenRA.Mods.Example.Games.SeventhLegion.FileFormats
{
	public class BimFrame
	{
		public readonly int Width;
		public readonly int Height;
		public readonly byte[] Pixels;

		public BimFrame(Stream stream)
		{
			var pixelsOffset = stream.ReadInt16();
			Height = stream.ReadInt16();

			if (stream.Length == stream.Position + pixelsOffset * Height)
			{
				Width = pixelsOffset;
				Pixels = stream.ReadBytes(Width * Height);
				return;
			}

			stream.Position = pixelsOffset;
			var compressed = stream.ReadBytes((int)(stream.Length - stream.Position));
			var readOffset = 0;
			stream.Position = 4;

			var rows = new byte[Height][];

			for (var i = 0; i < Height; i++)
			{
				var numChunks = stream.ReadInt16();
				var row = Array.Empty<byte>();

				for (var j = 0; j < numChunks; j++)
				{
					var offset = stream.ReadInt16();
					var copy = stream.ReadInt16();

					Array.Resize(ref row, Math.Max(row.Length, offset + copy));
					Array.Copy(compressed, readOffset, row, offset, copy);

					readOffset += copy;
				}

				rows[i] = row;
			}

			Width = rows.Max(row => row.Length);
			Pixels = new byte[Width * Height];

			for (var i = 0; i < rows.Length; i++)
				Array.Copy(rows[i], 0, Pixels, i * Width, rows[i].Length);
		}
	}
}
