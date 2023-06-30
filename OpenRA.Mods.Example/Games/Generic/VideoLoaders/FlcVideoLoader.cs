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
using OpenRA.Mods.Example.Games.Generic.FileFormats;
using OpenRA.Video;

namespace OpenRA.Mods.Example.Games.Generic.VideoLoaders
{
	public class FlcVideoLoader : IVideoLoader
	{
		public bool TryParseVideo(Stream stream, bool useFramePadding, out IVideo video)
		{
			var start = stream.Position;
			stream.Position = 4;
			var identifier = stream.ReadUInt16();
			stream.Position = start;

			video = null;

			if (identifier != 0xaf12)
				return false;

			video = new Flc(stream);

			return true;
		}
	}
}
