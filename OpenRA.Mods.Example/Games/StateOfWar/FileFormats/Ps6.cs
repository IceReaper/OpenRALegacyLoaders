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
using OpenRA.Primitives;

namespace OpenRA.Mods.Example.Games.StateOfWar.FileFormats
{
	public class Ps6
	{
		public readonly Ps6Frame[] Frames;

		public Ps6(Stream stream)
		{
			var frames = new List<Ps6Frame>();

			while (stream.Position < stream.Length)
			{
				var size = stream.ReadInt32();

				if (size == 0)
					break;

				frames.Add(new Ps6Frame(new SegmentStream(stream, stream.Position, size)));
			}

			Frames = frames.ToArray();
		}
	}
}
