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

namespace OpenRA.Mods.Example.Games.JurassicWar.FileFormats
{
	public class Pal
	{
		public readonly Color[] Colors;

		public Pal(Stream stream)
		{
			Colors = new Color[stream.Length / 3];

			for (var i = 0; i < Colors.Length; i++)
			{
				Colors[i] = Color.FromArgb(
					i == 0 ? 0x00 : 0xff,
					stream.ReadUInt8(),
					stream.ReadUInt8(),
					stream.ReadUInt8());
			}
		}
	}
}
