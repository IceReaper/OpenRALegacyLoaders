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
using OpenRA.FileSystem;
using OpenRA.Primitives;

namespace OpenRA.Mods.Example.Games.Dominion.FileFormats
{
	public class ZeroBin : IReadOnlyPackage
	{
		public class ZeroBinStream : SegmentStream
		{
			public readonly ZeroBin Container;
			public readonly string Name;

			public ZeroBinStream(ZeroBin container, string name, Stream stream)
				: base(stream, 0, stream.Length)
			{
				Container = container;
				Name = name;
			}
		}

		public sealed class ZeroBinEntry
		{
			public readonly Stream Stream;
			public readonly uint Offset;
			public readonly uint Length;

			public ZeroBinEntry(Stream stream, uint offset, uint length)
			{
				Offset = offset;
				Length = length;
				Stream = stream;
			}
		}

		public string Name { get; }
		public IEnumerable<string> Contents => index.Keys;
		readonly Dictionary<string, ZeroBinEntry> index = new();

		readonly Stream stream;

		public readonly Dictionary<uint, SprFrame> Frames = new();
		public readonly Dictionary<uint, SprAnim> Animations = new();
		public readonly Dictionary<uint, SprRef> References = new();
		public readonly Dictionary<uint, SprUnk> Unks = new();
		public readonly Dictionary<uint, Pal> Palettes = new();

		public ZeroBin(Stream binStream, Stream stream, string name)
		{
			this.stream = stream;
			Name = name;

			// TODO crazy data here.
			binStream.Position = 356;

			var chunks = new ZeroBinEntry[15];

			for (var i = 0; i < chunks.Length; i++)
				chunks[i] = new ZeroBinEntry(stream, binStream.ReadUInt32(), binStream.ReadUInt32());

			// Chunk 1: Unused
			// Chunk 2: Unused
			// TODO Chunk 3: {uint ascending, int unk}[]
			// Chunk 4: Strings
			// Chunk 7: Unused
			// Chunk 8: Unused
			// Chunk 9: Unused
			// Chunk 10: Modules
			// Chunk 11: Imports
			// Chunk 12: {uint module}[Imports.Length]
			// Chunk 13: Unused
			// Chunk 14: SourcePath

			// Chunk 5: Embedded Data
			var embeddedData = SegmentStream.CreateWithoutOwningStream(binStream, chunks[5].Offset, (int)chunks[5].Length);

			// Chunk 0: FileList
			binStream.Position = chunks[0].Offset;

			for (var i = 0u; binStream.Position < chunks[0].Offset + chunks[0].Length; i++)
			{
				var type = binStream.ReadUInt16();
				var offset = binStream.ReadUInt32();
				var length = binStream.ReadUInt32();

				switch (type)
				{
					case 1:
						Frames.Add(i, new SprFrame(new SegmentStream(stream, offset, length)));
						break;

					case 15:
						References.Add(i, new SprRef(new SegmentStream(embeddedData, offset, length)));
						break;

					case 17:
						Palettes.Add(i, new Pal(new SegmentStream(embeddedData, offset, length)));
						break;

					case 18:
						Unks.Add(i, new SprUnk(new SegmentStream(embeddedData, offset, length)));
						break;

					case 20:
					{
						Animations.Add(i, new SprAnim(new SegmentStream(embeddedData, offset, length)));
						break;
					}

					default:
					{
						var entry = new ZeroBinEntry(type == 11 ? stream : embeddedData, offset, length);

						var extension = type switch
						{
							11 => "11", // TODO what is this?
							14 => "14", // TODO what is this?
							16 => "16", // TODO what is this?
							19 => "19", // TODO what is this?
							_ => throw new Exception("Format not supported.")
						};

						index.Add($"{i:00000}.{extension}", entry);
						break;
					}
				}
			}

			// Chunk 6: Global Palette
			index.Add("global.pal", new ZeroBinEntry(binStream, chunks[6].Offset, chunks[6].Length));

			var unusedFrames = Frames.Keys.ToList();
			var dummyEntry = new ZeroBinEntry(this.stream, 0, 16);

			foreach (var (key, animation) in Animations)
			{
				index.Add($"{key:00000}.spr", dummyEntry);

				foreach (var frame in animation.Frames)
					unusedFrames.Remove(frame);
			}

			foreach (var reference in References.Values)
				unusedFrames.Remove(reference.Frame);

			foreach (var id in unusedFrames)
				index.Add($"{id:00000}.spr", dummyEntry);
		}

		public Stream GetStream(string filename)
		{
			return !index.TryGetValue(filename, out var entry)
				? null
				: new ZeroBinStream(this, filename,
					SegmentStream.CreateWithoutOwningStream(entry.Stream, entry.Offset, (int)entry.Length));
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
