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

using System.IO;
using OpenRA.Primitives;

namespace OpenRA.Mods.Example.Games.Kknd
{
	public static class OffsetUtils
	{
		public static uint ReadUInt32Offset(Stream stream)
		{
			var offset = stream.ReadUInt32();

			if (offset != 0 && offset != uint.MaxValue && stream is SegmentStream segmentStream)
				offset -= (uint)segmentStream.BaseOffset;

			return offset;
		}
	}
}
