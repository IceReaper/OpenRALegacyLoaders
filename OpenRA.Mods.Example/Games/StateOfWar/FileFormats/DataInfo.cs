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

namespace OpenRA.Mods.Example.Games.StateOfWar.FileFormats
{
	public class DataInfo : IReadOnlyPackage
	{
		sealed class DataEntry
		{
			public readonly uint Offset;
			public readonly uint Length;

			public DataEntry(uint offset, uint length)
			{
				Offset = offset;
				Length = length;
			}
		}

		public string Name { get; }
		public IEnumerable<string> Contents => index.Keys;

		readonly Dictionary<string, DataEntry> index = new();
		readonly Stream stream;

		public DataInfo(Stream infoStream, Stream dataStream, string name)
		{
			stream = dataStream;
			Name = name;

			infoStream.ReadInt32(); // 01 01 01 01
			var entries = infoStream.ReadInt32();

			for (var i = 0; i < entries; i++)
			{
				byte temp;
				var tempName = "";

				while ((temp = infoStream.ReadUInt8()) != 0x00)
					tempName += (char)(temp - 0xa);

				index.Add(tempName, new DataEntry(infoStream.ReadUInt32(), infoStream.ReadUInt32()));
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
