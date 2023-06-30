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
using System.Collections.Generic;
using OpenRA.FileSystem;
using OpenRA.Graphics;
using OpenRA.Mods.Example.Games.StateOfWar.PackageLoaders;
using OpenRA.Mods.Example.Games.StateOfWar.SpriteLoaders;
using OpenRA.Video;

namespace OpenRA.Mods.Example.Games.StateOfWar
{
	public class StateOfWarGameInfo : IGameInfo
	{
		public string Folder => "state_of_war";

		public IEnumerable<IPackageLoader> PackageLoaders => new IPackageLoader[] { new DataInfoPackageLoader() };
		public IEnumerable<ISoundLoader> SoundLoaders => Array.Empty<ISoundLoader>();
		public IEnumerable<ISpriteLoader> SpriteLoaders => new[] { new Ps6SpriteLoader() };
		public IEnumerable<IVideoLoader> VideoLoaders => Array.Empty<IVideoLoader>();

		public IEnumerable<string> Packages => new[] { "*.data" };

		public IEnumerable<string> AudioExtensions => Array.Empty<string>();
		public IEnumerable<string> SpriteExtensions => new[] { ".ps6" };
		public IEnumerable<string> VideoExtensions => Array.Empty<string>();
	}
}
