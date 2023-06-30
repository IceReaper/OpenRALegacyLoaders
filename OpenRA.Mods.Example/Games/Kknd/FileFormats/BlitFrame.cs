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

namespace OpenRA.Mods.Example.Games.Kknd.FileFormats
{
	public class BlitFrame
	{
		public readonly uint Width;
		public readonly uint Height;
		public readonly int OffsetX;
		public readonly int OffsetY;
		public readonly byte[] Pixels;

		public BlitFrame(Stream stream)
		{
			Width = stream.ReadUInt32();
			Height = stream.ReadUInt32();
			OffsetX = stream.ReadInt32();
			OffsetY = stream.ReadInt32();
			Pixels = stream.ReadBytes((int)(Width * Height));
		}
	}
}
