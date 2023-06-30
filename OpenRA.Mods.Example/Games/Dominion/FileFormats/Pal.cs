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

namespace OpenRA.Mods.Example.Games.Dominion.FileFormats
{
	public class Pal
	{
		public readonly Color[] Colors;

		public Pal(Stream stream)
		{
			Colors = new Color[256];

			var i = 0;

			Colors[i++] = Color.FromArgb(0x00, 0, 0, 0);
			Colors[i++] = Color.FromArgb(0x80, 0, 0, 0);
			Colors[i++] = Color.FromArgb(0xff, 0x00, 0xff);
			Colors[i++] = Color.FromArgb(0xff, 0x00, 0xff);
			Colors[i++] = Color.FromArgb(0xff, 0x00, 0xff);
			Colors[i++] = Color.FromArgb(0xff, 0x00, 0xff);
			Colors[i++] = Color.FromArgb(0xff, 0x00, 0xff);
			Colors[i++] = Color.FromArgb(0xff, 0x00, 0xff);
			Colors[i++] = Color.FromArgb(0xff, 0x00, 0xff);
			Colors[i++] = Color.FromArgb(0xff, 0x00, 0xff);

			while (stream.Position < stream.Length)
			{
				Colors[i++] = Color.FromArgb(
					byte.MaxValue,
					stream.ReadUInt8(),
					stream.ReadUInt8(),
					stream.ReadUInt8());

				stream.ReadUInt8(); // Unused alpha
			}

			while (i < Colors.Length)
				Colors[i++] = Color.FromArgb(0xff, 0x00, 0xff);
		}
	}
}
