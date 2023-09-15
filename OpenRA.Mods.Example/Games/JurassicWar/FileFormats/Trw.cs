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
	public class Trw : IReadOnlyPackage
	{
		public class TrwStream : SegmentStream
		{
			public record TrwHeader(ushort Channels, uint SampleRate, ushort SampleBits);

			public readonly TrwHeader Header;

			public TrwStream(TrwHeader header, Stream stream)
				: base(stream, stream.Position, stream.Length)
			{
				Header = header;
			}
		}

		sealed class TrwEntry
		{
			public readonly uint Offset;
			public readonly uint Length;

			public TrwEntry(uint offset, uint length)
			{
				Offset = offset;
				Length = length;
			}
		}

		public string Name { get; }
		public IEnumerable<string> Contents => index.Keys;

		readonly Dictionary<string, TrwEntry> index = new();
		readonly Stream stream;

		readonly TrwStream.TrwHeader[] headers;

		public Trw(Stream stream, string name)
		{
			this.stream = stream;
			Name = name;

			var magic = Encoding.ASCII.GetString(stream.ReadBytes(4));

			if (magic != "TRW\u001a")
				throw new Exception();

			var entries = stream.ReadUInt32();
			var entriesCopy = stream.ReadUInt32();
			var dataOffset = stream.ReadUInt32();

			if (stream.ReadBytes(16).Any(e => e != 0x00))
				throw new Exception();

			if (entriesCopy != entries)
				throw new Exception();

			headers = new TrwStream.TrwHeader[entries];

			for (var i = 0; i < entries; i++)
			{
				var offset = stream.ReadUInt32();
				var length = stream.ReadUInt32();
				var channels = stream.ReadUInt16();
				var samplesRate = stream.ReadUInt16();
				var sampleBits = stream.ReadUInt16();
				var unk = this.stream.ReadUInt16();

				if (unk != 0 && unk != channels)
					throw new Exception();

				headers[i] = new TrwStream.TrwHeader(channels, samplesRate, sampleBits);
				index.Add($"{i}.sound", new TrwEntry(dataOffset + offset, length));
			}
		}

		public Stream GetStream(string filename)
		{
			if (!index.TryGetValue(filename, out var entry))
				return null;

			var id = int.Parse(Path.GetFileNameWithoutExtension(filename));

			return new TrwStream(headers[id],
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
