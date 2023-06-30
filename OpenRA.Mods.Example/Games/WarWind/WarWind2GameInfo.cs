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
using OpenRA.Mods.Example.Games.WarWind.PackageLoaders;
using OpenRA.Mods.Example.Games.WarWind.SpriteLoaders;
using OpenRA.Video;

namespace OpenRA.Mods.Example.Games.WarWind
{
	public class WarWind2GameInfo : IGameInfo
	{
		public string Folder => "war_wind2";

		public IEnumerable<IPackageLoader> PackageLoaders => new IPackageLoader[] { new ResPackageLoader() };
		public IEnumerable<ISoundLoader> SoundLoaders => new[] { new WavLoader() };
		public IEnumerable<ISpriteLoader> SpriteLoaders => new[] { new D3GrSpriteLoader() };
		public IEnumerable<IVideoLoader> VideoLoaders => Array.Empty<IVideoLoader>();

		public IEnumerable<string> Packages => new[] { "res.*" };

		public IEnumerable<string> AudioExtensions => new[] { ".wav" };
		public IEnumerable<string> SpriteExtensions => new[] { ".d3gr" };
		public IEnumerable<string> VideoExtensions => Array.Empty<string>();
	}
}
