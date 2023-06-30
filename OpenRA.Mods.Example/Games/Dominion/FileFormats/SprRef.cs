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

namespace OpenRA.Mods.Example.Games.Dominion.FileFormats
{
	public class SprRef
	{
		public readonly uint Frame;
		public readonly int OffsetX;
		public readonly int OffsetY;

		public SprRef(Stream stream)
		{
			Frame = stream.ReadUInt32();
			var unk1 = stream.ReadUInt32();
			OffsetX = stream.ReadInt32();
			OffsetY = stream.ReadInt32();

			if (unk1 != 0)
				throw new Exception("SprReference: Unknown value!");

			if (stream.Position < stream.Length)
				Console.WriteLine("?");
		}
	}
}
