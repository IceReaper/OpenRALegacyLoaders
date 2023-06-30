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
using OpenRA.Mods.Example.Games.Earth2140.PackageLoaders;
using OpenRA.Mods.Example.Games.Earth2140.SoundLoaders;
using OpenRA.Mods.Example.Games.Earth2140.SpriteLoaders;
using OpenRA.Mods.Example.Games.Generic.SpriteLoaders;
using OpenRA.Mods.Example.Games.Generic.VideoLoaders;
using OpenRA.Video;

namespace OpenRA.Mods.Example.Games.Earth2140
{
	public class Earth2140GameInfo : IGameInfo
	{
		public string Folder => "earth_2140";

		public IEnumerable<IPackageLoader> PackageLoaders => new IPackageLoader[] { new WdPackageLoader() };
		public IEnumerable<ISoundLoader> SoundLoaders => new ISoundLoader[] { new SmpSoundLoader() };

		public IEnumerable<ISpriteLoader> SpriteLoaders => new ISpriteLoader[]
		{
			new DatSpriteLoader(), new MixSpriteLoader(), new PcxSpriteLoader()
		};

		public IEnumerable<IVideoLoader> VideoLoaders => new IVideoLoader[] { new FlcVideoLoader() };

		public IEnumerable<string> Packages => new[] { "*.wd" };

		public IEnumerable<string> AudioExtensions => new[] { ".smp" };
		public IEnumerable<string> SpriteExtensions => new[] { ".dat", ".mix", ".pcx" };
		public IEnumerable<string> VideoExtensions => new[] { ".flc" };
	}
}
