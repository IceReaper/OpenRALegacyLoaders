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
using System.IO;

namespace OpenRA.Mods.Example.Games.WarWind.FileFormats
{
	public class D3GrFrame
	{
		public readonly int OffsetX;
		public readonly int OffsetY;
		public readonly int Width;
		public readonly int Height;
		public readonly byte[] Pixels;

		public D3GrFrame(Stream stream)
		{
			var pixelsLength = stream.ReadInt32();
			stream.ReadInt32(); // TODO this is usually 2. When this is 1, the frame is 0x0, hence empty.
			OffsetX = stream.ReadInt16();
			OffsetY = stream.ReadInt16();
			Height = stream.ReadInt16();
			Width = stream.ReadInt16();
			Pixels = stream.ReadBytes(Width * Height);

			if (pixelsLength - 16 != Width * Height)
				throw new Exception("D3GR: Broken frame!");
		}
	}
}
