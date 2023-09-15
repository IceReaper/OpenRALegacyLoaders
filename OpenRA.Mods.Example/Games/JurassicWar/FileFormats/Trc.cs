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
using OpenRA.FileSystem;
using OpenRA.Primitives;

namespace OpenRA.Mods.Example.Games.JurassicWar.FileFormats
{
	public class Trc : IReadOnlyPackage
	{
		sealed class TrcEntry
		{
			public readonly uint Offset;
			public readonly uint Length;

			public TrcEntry(uint offset, uint length)
			{
				Offset = offset;
				Length = length;
			}
		}

		public string Name { get; }
		public IEnumerable<string> Contents => index.Keys;

		readonly Dictionary<string, TrcEntry> index = new();
		readonly Stream stream;

		public Trc(Stream stream, string name)
		{
			this.stream = stream;
			Name = name;

			var magic = Encoding.ASCII.GetString(stream.ReadBytes(4));

			if (magic != "TRC\u001a")
				throw new Exception();

			stream.ReadUInt32(); // TODO Mostly == entries, not on file 13.
			var entries = stream.ReadUInt32();
			var dataOffset = stream.ReadUInt32();

			if (stream.ReadBytes(16).Any(e => e != 0x00))
				throw new Exception();

			var duplicates = new List<string>();

			for (var i = 0; i < entries; i++)
			{
				var fileName = Encoding.ASCII.GetString(stream.ReadBytes(12)).Split('\0')[0];
				var offset = stream.ReadUInt32();
				var compressedLength = stream.ReadUInt32();
				var uncompressedLength = stream.ReadUInt32();
				this.stream.ReadUInt32(); // TODO

				if (this.stream.ReadBytes(4).Any(e => e != 0x00))
					throw new Exception();

				// TODO implement
				if (compressedLength != uncompressedLength)
					continue;

				// TODO what the hell?
				if (index.ContainsKey(fileName))
					duplicates.Add(fileName);

				if (duplicates.Contains(fileName))
					fileName = Path.GetFileNameWithoutExtension(fileName) + $".{i}" + Path.GetExtension(fileName);

				index.Add(fileName, new TrcEntry(dataOffset + offset, compressedLength));
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
