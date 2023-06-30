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

namespace OpenRA.Mods.Example.Games.SeventhLegion.FileFormats
{
	public class Col
	{
		public readonly Color[] Colors = new Color[256];

		public Col(Stream stream)
		{
			for (var i = 0; i < Colors.Length; i++)
				Colors[i] = Color.FromArgb(i == 0 ? byte.MinValue : byte.MaxValue, stream.ReadUInt8() << 2, stream.ReadUInt8() << 2, stream.ReadUInt8() << 2);
		}
	}
}
