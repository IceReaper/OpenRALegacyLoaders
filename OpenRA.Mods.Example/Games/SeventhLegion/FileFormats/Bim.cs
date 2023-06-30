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

using System.Collections.Generic;
using System.IO;
using System.Linq;
using OpenRA.Primitives;

namespace OpenRA.Mods.Example.Games.SeventhLegion.FileFormats
{
	public class Bim
	{
		public readonly BimFrame[] Frames;

		public Bim(Stream s)
		{
			var firstValue = s.ReadInt32();

			if (firstValue == 0x5a4c4356)
			{
				s = Decompressor.Decompress(s);
				firstValue = s.ReadInt32();
			}

			var firstFrame = firstValue;
			s.Position -= 4;

			var frames = new List<int[]>();

			while (s.Position < firstFrame)
			{
				var offset = s.ReadInt32();
				var end = offset;
				var position = s.Position;

				while (end == offset)
				{
					if (s.Position == firstFrame)
					{
						end = (int)s.Length;
						break;
					}

					end = s.ReadInt32();
				}

				s.Position = position;

				// Skip last entry if its 4 bytes (numFrames) (this entry does not always exist!)
				if (offset != s.Length - 4)
					frames.Add(new[] { offset, end - offset });
			}

			Frames = frames.Select(e => new BimFrame(new SegmentStream(s, e[0], e[1]))).ToArray();
		}
	}
}
