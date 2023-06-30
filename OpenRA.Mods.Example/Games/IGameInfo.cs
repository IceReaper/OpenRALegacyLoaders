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

using System.Collections.Generic;
using OpenRA.FileSystem;
using OpenRA.Graphics;
using OpenRA.Video;

namespace OpenRA.Mods.Example.Games
{
	public interface IGameInfo
	{
		string Folder { get; }

		IEnumerable<IPackageLoader> PackageLoaders { get; }
		IEnumerable<ISoundLoader> SoundLoaders { get; }
		IEnumerable<ISpriteLoader> SpriteLoaders { get; }
		IEnumerable<IVideoLoader> VideoLoaders { get; }

		IEnumerable<string> Packages { get; }

		IEnumerable<string> AudioExtensions { get; }
		IEnumerable<string> SpriteExtensions { get; }
		IEnumerable<string> VideoExtensions { get; }
	}
}
