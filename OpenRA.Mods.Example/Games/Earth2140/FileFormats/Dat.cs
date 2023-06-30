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

namespace OpenRA.Mods.Example.Games.Earth2140.FileFormats
{
	public class Dat
	{
		public readonly int Width;
		public readonly int Height;
		public readonly byte[] Pixels;

		public Dat(Stream stream)
		{
			Width = stream.ReadUInt16();
			Height = stream.ReadUInt16();
			stream.ReadUInt8(); // TODO always 1
			stream.ReadUInt8(); // TODO id?
			Pixels = stream.ReadBytes(Width * Height);
		}
	}
}
