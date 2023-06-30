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
using OpenRA.Mods.Example.Games.Kknd.FileFormats;
using OpenRA.Video;

namespace OpenRA.Mods.Example.Games.Kknd.VideoLoaders
{
	public class VbcVideoLoader : IVideoLoader
	{
		public bool TryParseVideo(Stream s, bool useFramePadding, out IVideo video)
		{
			video = null;

			if (!IsVbc(s))
				return false;

			video = new Vbc(s);

			return true;
		}

		static bool IsVbc(Stream s)
		{
			var start = s.Position;

			if (s.ReadASCII(4) != "SIFF")
			{
				s.Position = start;

				return false;
			}

			s.Position = start;

			return true;
		}
	}
}
