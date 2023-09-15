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
using System.Text;
using OpenRA.FileSystem;

namespace OpenRA.Mods.Example.Games.Z.FileFormats
{
	public class ExpContainer : IReadOnlyPackage
	{
		public class ExpStream : MemoryStream
		{
			public readonly ExpContainer Container;

			public ExpStream(ExpContainer container, byte[] value)
				: base(value)
			{
				Container = container;
			}
		}

		public string Name { get; }
		public IEnumerable<string> Contents => index.Keys;

		readonly Dictionary<string, byte[]> index = new();
		readonly Stream stream;

		public ExpContainer(Stream stream, string name)
		{
			this.stream = stream;
			Name = name;

			stream.Position = stream.Length - 8;

			var numFiles = stream.ReadUInt32();
			var magic = Encoding.ASCII.GetString(stream.ReadBytes(4));

			if (magic != "JMP2")
				throw new Exception();

			for (var i = 0; i < numFiles; i++)
			{
				stream.Position = stream.Length - 8 - (numFiles - i) * 24;

				var fileName = Encoding.ASCII.GetString(stream.ReadBytes(16)).Split('\0')[0];
				var offset = stream.ReadUInt32();
				var decompressedLength = stream.ReadUInt32();
				var compressedLength = stream.Length - offset - 8;

				if (stream.Position < stream.Length - 8)
				{
					stream.Position += 16;
					compressedLength = stream.ReadUInt32() - offset;
				}

				stream.Position = offset;
				var compressed = stream.ReadBytes((int)compressedLength);
				var decompressed = Decompress(compressed, decompressedLength);

				index.Add(fileName, decompressed);
			}
		}

		static byte[] Decompress(byte[] compressed, uint decompressedLength)
		{
			var decompressed = new byte[decompressedLength];

			var reader = new MemoryStream(compressed);
			var writer = new BinaryWriter(new MemoryStream(decompressed));

			while (true)
			{
				var compressionBits = reader.ReadUInt8();

				for (var i = 7; i >= 0; i--)
				{
					if (((compressionBits >> i) & 1) == 1)
						writer.Write(reader.ReadUInt8());
					else
					{
						var compression = reader.ReadUInt16();

						if (compression == 0)
							return decompressed;

						var length = (compression & 0x000f) + 2;
						var offset = (compression >> 4) - 1;

						const int BlockSize = 0x1000;
						var blockStart = writer.BaseStream.Position / BlockSize * BlockSize;
						var start = blockStart + offset;

						if (offset >= writer.BaseStream.Position % BlockSize)
							start -= BlockSize;

						for (var j = 0; j < length; j++)
							writer.Write(decompressed[start + j]);
					}
				}
			}
		}

		public Stream GetStream(string filename)
		{
			return !index.TryGetValue(filename, out var entry) ? null : new ExpStream(this, entry);
		}

		public bool Contains(string filename)
		{
			return index.ContainsKey(filename);
		}

		public IReadOnlyPackage OpenPackage(string filename, FileSystem.FileSystem context)
		{
			var childStream = GetStream(filename);

			if (childStream == null)
				return null;

			if (context.TryParsePackage(childStream, filename, out var package))
				return package;

			childStream.Dispose();

			return null;
		}

		public void Dispose()
		{
			GC.SuppressFinalize(this);

			stream.Dispose();
		}
	}
}
