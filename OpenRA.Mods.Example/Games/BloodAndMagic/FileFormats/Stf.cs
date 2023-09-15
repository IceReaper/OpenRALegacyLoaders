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
using OpenRA.FileSystem;
using OpenRA.Primitives;

namespace OpenRA.Mods.Example.Games.BloodAndMagic.FileFormats
{
	public class Stf : IReadOnlyPackage
	{
		public class StfStream : SegmentStream
		{
			public readonly Stf Container;
			public readonly Stream Header;
			public readonly ushort Entries;

			public StfStream(Stf container, Stream stream, Stream header, ushort entries)
				: base(stream, 0, stream.Length)
			{
				Container = container;
				Header = header;
				Entries = entries;
			}
		}

		class StfEntry
		{
			public readonly long HeaderPosition;
			public readonly uint HeaderSize;
			public readonly ushort HeaderEntries;
			public readonly long Position;
			public readonly uint Size;

			public StfEntry(long headerPosition, uint headerSize, ushort headerEntries, long position, uint size)
			{
				HeaderPosition = headerPosition;
				HeaderSize = headerSize;
				HeaderEntries = headerEntries;
				Position = position;
				Size = size;
			}
		}

		public string Name { get; }
		public IEnumerable<string> Contents => index.Keys;

		readonly Dictionary<string, StfEntry> index = new();
		readonly Stream stream;

		public Stf(Stream stream, string name)
		{
			this.stream = stream;
			Name = name;

			while (stream.Position < stream.Length)
			{
				var fileId = stream.ReadUInt16();
				stream.ReadUInt32(); // compressionType?
				var compressedSize = stream.ReadUInt32();
				stream.ReadUInt32(); // uncompressedSize?
				var fileType = stream.ReadUInt16();
				var headerEntries = stream.ReadUInt16();
				var headerSize = stream.ReadUInt32();
				var unk1 = stream.ReadUInt32();
				var unk2 = stream.ReadUInt16();

				if (unk1 != 0 || unk2 != 0xfefe)
					throw new Exception();

				// We should not have all file types!
				var extension = fileType switch
				{
					1 => "spr",
					3 => "hmp",
					4 => "wav",
					5 => "pal",
					7 => "fon",
					8 => "sqb",
					18 => "tlb",
					19 => "mif",
					_ => "unk"
				};

				var headerPosition = this.stream.Position;
				stream.Position += headerSize;

				var filePosition = stream.Position;
				stream.Position += compressedSize;

				index.Add($"{fileId}.{extension}",
					new StfEntry(headerPosition, headerSize, headerEntries, filePosition, compressedSize));
			}
		}

		public Stream GetStream(string filename)
		{
			return !index.TryGetValue(filename, out var entry)
				? null
				: new StfStream(this, SegmentStream.CreateWithoutOwningStream(stream, entry.Position, (int)entry.Size),
					SegmentStream.CreateWithoutOwningStream(stream, entry.HeaderPosition, (int)entry.HeaderSize),
					entry.HeaderEntries);
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
