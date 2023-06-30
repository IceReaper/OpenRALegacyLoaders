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
using OpenRA.Mods.Common.AudioLoaders;
using OpenRA.Mods.Example.Games.Kknd.PackageLoaders;
using OpenRA.Mods.Example.Games.Kknd.SpriteLoaders;
using OpenRA.Mods.Example.Games.Kknd.VideoLoaders;
using OpenRA.Video;

namespace OpenRA.Mods.Example.Games.Kknd
{
	public class Kknd2KrossfireGameInfo : IGameInfo
	{
		public string Folder => "kknd2_krossfire";

		public IEnumerable<IPackageLoader> PackageLoaders => new IPackageLoader[] { new LvlPackageLoader() };
		public IEnumerable<ISoundLoader> SoundLoaders => new ISoundLoader[] { new WavLoader() };

		public IEnumerable<ISpriteLoader> SpriteLoaders => new ISpriteLoader[]
		{
			new BlitSpriteLoader(), new MapdSpriteLoader(), new MobdSpriteLoader()
		};

		public IEnumerable<IVideoLoader> VideoLoaders => new IVideoLoader[] { new VbcVideoLoader() };

		public IEnumerable<string> Packages => new[] { "fmv", "levels", "*.bpk", "*.lpk", "*.lpm", "*.lps", "*.mpk", "*.spk" };

		public IEnumerable<string> AudioExtensions => new[] { ".soun" };
		public IEnumerable<string> SpriteExtensions => new[] { ".blit", ".mapd", ".mobd" };
		public IEnumerable<string> VideoExtensions => new[] { ".vbc" };
	}
}
