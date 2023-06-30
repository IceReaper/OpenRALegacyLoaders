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
using OpenRA.Mods.Common.AudioLoaders;
using OpenRA.Mods.Example.Games.Dominion.PackageLoaders;
using OpenRA.Mods.Example.Games.Dominion.SpriteLoaders;
using OpenRA.Video;

namespace OpenRA.Mods.Example.Games.Dominion
{
	public class DominionGameInfo : IGameInfo
	{
		public string Folder => "dominion";

		public IEnumerable<IPackageLoader> PackageLoaders => new IPackageLoader[]
		{
			new ZeroBinPackageLoader(), new RdfPackageLoader()
		};

		public IEnumerable<ISoundLoader> SoundLoaders => new[] { new WavLoader() };

		public IEnumerable<ISpriteLoader> SpriteLoaders => new ISpriteLoader[]
		{
			new Tl2SpriteLoader(), new SprSpriteLoader()
		};

		public IEnumerable<IVideoLoader> VideoLoaders => Array.Empty<IVideoLoader>();

		public IEnumerable<string> Packages => new[] { "*.000", "*.rdf", "." };

		public IEnumerable<string> AudioExtensions => new[] { ".wav" };
		public IEnumerable<string> SpriteExtensions => new[] { ".tl2", ".spr" };
		public IEnumerable<string> VideoExtensions => Array.Empty<string>();
	}
}
