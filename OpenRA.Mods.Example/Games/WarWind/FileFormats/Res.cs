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

namespace OpenRA.Mods.Example.Games.WarWind.FileFormats
{
	public class Res : IReadOnlyPackage
	{
		sealed class ResEntry
		{
			public readonly int Offset;
			public readonly int Length;

			public ResEntry(int offset, int length)
			{
				Offset = offset;
				Length = length;
			}
		}

		public string Name { get; }
		public IEnumerable<string> Contents => index.Keys;

		readonly Dictionary<string, ResEntry> index = new();
		readonly Stream stream;

		public Res(Stream stream, string name)
		{
			this.stream = stream;
			Name = name;

			var entries = stream.ReadInt32();

			for (var i = 0; i < entries; i++)
			{
				stream.Position = 4 + i * 4;

				var offset = stream.ReadInt32();
				var length = (i + 1 == entries ? (int)stream.Length : stream.ReadInt32()) - offset;
				var entry = new ResEntry(offset, length);

				stream.Position = offset;

				var test = stream.ReadUInt32();

				if (test == 0x52473344)
				{
					stream.Position += 4;

					// Pixel data is optional in D3GR.
					if (stream.ReadUInt32() == 0)
						index.Add($"{i}.pal", entry);
					else
						index.Add($"{i}.d3gr", entry);
				}
				else if (test == 0x46464952)
					index.Add($"{i}.wav", entry);
				else
					index.Add($"{i}.data", entry);
			}
		}

		public Stream GetStream(string filename)
		{
			return !index.TryGetValue(filename, out var entry)
				? null
				: SegmentStream.CreateWithoutOwningStream(stream, entry.Offset, entry.Length);
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
