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

namespace OpenRA.Mods.Example.Games.Kknd.FileFormats
{
	public class Pal
	{
		public readonly Color[] Colors;

		public Pal(Stream stream)
		{
			Colors = new Color[stream.Length / 2];

			for (var i = 0; i < Colors.Length; i++)
			{
				var color16 = stream.ReadUInt16();
				var r = ((color16 & 0x7c00) >> 7) & 0xff;
				var g = ((color16 & 0x03e0) >> 2) & 0xff;
				var b = ((color16 & 0x001f) << 3) & 0xff;
				Colors[i] = Color.FromArgb(i == 0 ? 0x00 : 0xff, r, g, b);
			}
		}
	}
}
