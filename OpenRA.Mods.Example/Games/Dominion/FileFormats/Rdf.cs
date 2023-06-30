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
using OpenRA.Primitives;

namespace OpenRA.Mods.Example.Games.Dominion.FileFormats
{
	public class Rdf : IReadOnlyPackage
	{
		sealed class RdfEntry
		{
			public readonly uint Offset;
			public readonly uint Length;

			public RdfEntry(uint length, uint offset)
			{
				Offset = offset;
				Length = length;
			}
		}

		public string Name { get; }
		public IEnumerable<string> Contents => index.Keys;

		readonly Dictionary<string, RdfEntry> index = new();
		readonly Stream stream;

		public Rdf(Stream stream, string name)
		{
			this.stream = stream;
			Name = name;

			// Resource Data File þ Copyright (c) 1997 Ion Storm, Ltd.
			// Dominion Wave Audio Files
			// <empty>
			stream.ReadBytes(64 + 64 + 64);
			stream.ReadUInt16(); // 538
			stream.ReadUInt16(); // 256
			stream.ReadUInt16(); // 1
			var numFiles = stream.ReadUInt16();
			var filesOffset = stream.ReadUInt32();

			stream.Position = filesOffset;

			for (var i = 0; i < numFiles; i++)
			{
				index.Add(
					Encoding.ASCII.GetString(stream.ReadBytes(120)).Split('\0')[0],
					new RdfEntry(stream.ReadUInt32(), stream.ReadUInt32()));
			}
		}

		public Stream GetStream(string filename)
		{
			return !index.TryGetValue(filename, out var entry)
				? null
				: SegmentStream.CreateWithoutOwningStream(stream, entry.Offset, (int)entry.Length);
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
