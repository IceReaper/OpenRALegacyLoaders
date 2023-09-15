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

namespace OpenRA.Mods.Example.Games.BloodAndMagic.FileFormats
{
	public class Ani
	{
		public readonly AniFrame[] Frames;

		public Ani(Stf.StfStream stream)
		{
			Frames = new AniFrame[stream.Entries];

			for (var i = 0; i < stream.Entries; i++)
				Frames[i] = new AniFrame(stream);
		}
	}
}
