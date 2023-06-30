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

namespace OpenRA.Mods.Example.Games.Earth2140.FileFormats
{
	public class Wd : IReadOnlyPackage
	{
		public class WdStream : SegmentStream
		{
			public readonly Wd Wd;
			public readonly string Name;

			public WdStream(Wd wd, string name, Stream stream)
				: base(stream, 0, stream.Length)
			{
				Wd = wd;
				Name = name;
			}
		}

		sealed class Entry
		{
			public readonly uint Offset;
			public readonly uint Length;

			public Entry(uint offset, uint length)
			{
				Offset = offset;
				Length = length;
			}
		}

		public string Name { get; }
		public IEnumerable<string> Contents => index.Keys;
		readonly Dictionary<string, Entry> index = new();

		readonly Stream stream;

		public Wd(Stream stream, string filename)
		{
			this.stream = stream;
			Name = filename;

			var numFiles = stream.ReadUInt32();

			if (numFiles == 0)
			{
				var lastOffset = 0u;

				for (var i = 0; i < 255; i++)
				{
					var offset = stream.ReadUInt32();

					if (offset > lastOffset)
						index.Add($"{i}.smp", new Entry(lastOffset + 0x400, offset - lastOffset));

					lastOffset = offset;
				}
			}
			else
			{
				for (var i = 0; i < numFiles; i++)
				{
					var entry = new Entry(stream.ReadUInt32(), stream.ReadUInt32());

					stream.ReadUInt32(); // 0x00
					stream.ReadUInt32(); // 0x00
					stream.ReadBytes(4); // TODO FLC, GRAPH, MENU, MIX, PIRO

					var filePathOffset = stream.ReadUInt32();

					var originalPosition = stream.Position;
					stream.Position = numFiles * 24 + 8 + filePathOffset;
					var name = stream.ReadASCIIZ();
					stream.Position = originalPosition;

					index.Add(name, entry);
				}
			}
		}

		public Stream GetStream(string filename)
		{
			return !index.TryGetValue(filename, out var entry)
				? null
				: new WdStream(this, filename,
					SegmentStream.CreateWithoutOwningStream(stream, entry.Offset, (int)entry.Length));
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
