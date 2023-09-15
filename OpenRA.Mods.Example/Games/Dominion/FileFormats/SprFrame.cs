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

namespace OpenRA.Mods.Example.Games.Dominion.FileFormats
{
	public class SprFrame
	{
		public readonly uint Width;
		public readonly uint Height;
		public readonly int OffsetX;
		public readonly int OffsetY;
		public readonly byte[] Pixels;

		public SprFrame(Stream stream)
		{
			Width = stream.ReadUInt32();
			Height = stream.ReadUInt32();
			var flag = stream.ReadUInt32();
			OffsetX = stream.ReadInt32();
			OffsetY = stream.ReadInt32();
			stream.ReadUInt32();

			Pixels = new byte[Width * Height];

			if (flag == 0)
			{
				try
				{
					while (stream.Position < stream.Length)
					{
						var bitmask = stream.ReadUInt16();
						var chunkLength = bitmask & 0x1fff;
						var chunkType = bitmask >> 13;

						if (chunkType == 0)
						{
							var unk = stream.ReadUInt16();

							if (unk == 0)
							{
								stream.Position -= 2;
								for (var y = 0; y < Height; y++)
								{
									var row = stream.ReadBytes((int)((Width + 1) / 2 * 2));
									Array.Copy(row, 0, Pixels, y * Width, Width);
								}
							}
							else if (unk == 257)
							{
								// TODO what the hell? skip amount? Fill color? idk?
							}
							else
								throw new Exception("SPR: Unknown value!");
						}
						else if (chunkType == 3)
						{
							var segmentLength = stream.ReadUInt16();
							var data = stream.ReadBytes(segmentLength); // TODO this is compressed somehow!

							if (segmentLength + 2 != chunkLength)
								throw new Exception("SPR: Broken length!");
						}
						else if (chunkType == 7)
						{
							if (stream.Position < stream.Length)
								throw new Exception("SPR: End of file expected!");
						}
						else
							throw new Exception("SPR: Unsupported chunk type!");
					}
				}
				catch (Exception e)
				{
					Console.WriteLine(e);
				}

				return;
			}

			for (var y = 0; y < Height; y++)
			{
				var numChunks = stream.ReadUInt8();
				var unk = stream.ReadUInt8();
				var x = 0;

				if (unk != 2)
					throw new Exception("SPR: Unknown value!");

				for (var i = 0; i < numChunks; i++)
				{
					var bitmask = stream.ReadUInt16();
					var length = bitmask & 0x7ff;
					var alpha = (byte)(bitmask >> 11);

					if (alpha == 3)
					{
						Array.Copy(stream.ReadBytes(length), 0, Pixels, (Height - y - 1) * Width + x, length);

						if (length % 2 != 0)
							stream.Position++;
					}
					else
						Array.Fill(Pixels, alpha, (int)((Height - y - 1) * Width + x), length);

					x += length;
				}
			}

			if (stream.ReadUInt8() != 0 || stream.Position < stream.Length)
				throw new Exception("SPR: Unknown data!");
		}
	}
}
