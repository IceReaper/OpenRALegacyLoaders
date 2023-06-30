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
using System.Linq;

namespace OpenRA.Mods.Example.Games.Dominion.FileFormats
{
	public class SprAnim
	{
		public readonly uint[] Frames;

		public SprAnim(Stream stream)
		{
			var unk1 = stream.ReadUInt32(); // 0
			var unk2 = stream.ReadUInt32(); // 0
			var numFrames1 = stream.ReadUInt32();
			var numFrames2 = stream.ReadUInt32();
			var unk5 = stream.ReadUInt32(); // 0
			var unk6 = stream.ReadUInt32(); // 0
			var unk7 = stream.ReadUInt32(); // 0
			var unk8 = stream.ReadUInt32(); // TODO fps?
			var unk9 = stream.ReadBytes(6); // TODO

			if (unk1 != 0 || unk2 != 0 || unk5 != 0 || unk6 != 0 || unk7 != 0)
				throw new Exception("SprAnim: Unknown value!");

			if (numFrames1 != numFrames2)
				throw new Exception("SprAnim: Unknown value!");

			Frames = new uint[numFrames1];

			for (var i = 0; i < numFrames1; i++)
				Frames[i] = stream.ReadUInt32();

			var unk10 = new uint[8];

			for (var i = 0; i < unk10.Length; i++)
				unk10[i] = stream.ReadUInt32();

			if (unk10.Any(e => e != 0))
				throw new Exception("SprAnim: Unknown value!");

			if (stream.Position < stream.Length)
				throw new Exception("SprAnim: Unknown value!");
		}
	}
}
