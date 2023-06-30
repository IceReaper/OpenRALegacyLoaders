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

namespace OpenRA.Mods.Example.Games.SeventhLegion
{
	public static class Decompressor
	{
		public static Stream Decompress(Stream compressed)
		{
			var uncompressedStream = new MemoryStream(new byte[compressed.ReadInt32()]);
			Decompress(compressed, uncompressedStream);
			uncompressedStream.Position = 0;

			return uncompressedStream;
		}

		static void Decompress(Stream compressed, Stream uncompressed)
		{
			var dictionary = new byte[0x1000];
			var index = 0;

			while (uncompressed.Position < uncompressed.Length)
			{
				var flags = compressed.ReadUInt8();

				for (var i = 0; i < 8; i++)
				{
					if (((flags >> i) & 0b1) == 1)
					{
						uncompressed.WriteByte(dictionary[index] = compressed.ReadUInt8());
						index = (index + 1) % dictionary.Length;
					}
					else
					{
						var metaBytes = compressed.ReadBytes(2);
						var offset = metaBytes[0] + ((metaBytes[1] & 0xF0) << 4) + 18;
						var length = (metaBytes[1] & 0x0F) + 3;

						for (var j = 0; j < length; j++)
						{
							uncompressed.WriteByte(dictionary[index] = dictionary[(offset + j) % dictionary.Length]);
							index = (index + 1) % dictionary.Length;
						}
					}

					if (uncompressed.Position == uncompressed.Length)
						break;
				}
			}
		}
	}
}
