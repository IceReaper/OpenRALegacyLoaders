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
using OpenRA.Mods.Example.Games.JurassicWar.PackageLoaders;
using OpenRA.Mods.Example.Games.JurassicWar.SoundLoaders;
using OpenRA.Mods.Example.Games.JurassicWar.SpriteLoaders;
using OpenRA.Video;

namespace OpenRA.Mods.Example.Games.JurassicWar
{
	public class JurassicWarGameInfo : IGameInfo
	{
		public string Folder => "jurassic_war";

		public IEnumerable<IPackageLoader> PackageLoaders => new IPackageLoader[]
		{
			new TrcPackageLoader(), new TrwPackageLoader()
		};

		public IEnumerable<ISoundLoader> SoundLoaders => new[] { new TrwSoundLoader() };

		public IEnumerable<ISpriteLoader> SpriteLoaders => new ISpriteLoader[]
		{
			new TrsSpriteLoader()
		};

		public IEnumerable<IVideoLoader> VideoLoaders => Array.Empty<IVideoLoader>();

		public IEnumerable<string> Packages => new[] { "*.trc", "*.trw", "." };

		public IEnumerable<string> AudioExtensions => new[] { ".sound" };
		public IEnumerable<string> SpriteExtensions => new[] { ".trs" };
		public IEnumerable<string> VideoExtensions => Array.Empty<string>();
	}
}
