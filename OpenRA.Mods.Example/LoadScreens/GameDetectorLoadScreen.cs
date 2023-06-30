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
using System.IO;
using System.Linq;
using System.Reflection;
using OpenRA.Graphics;
using OpenRA.Mods.Common.LoadScreens;
using OpenRA.Mods.Example.Games;
using OpenRA.Video;

namespace OpenRA.Mods.Example.LoadScreens
{
	public class GameDetectorLoadScreen : BlankLoadScreen
	{
		public override void StartGame(Arguments args)
		{
			Game.Settings.Graphics.SheetSize = 8192;

			var root = ModData.ModFiles.MountedPackages.Skip(1).First().Name;
			var assetBrowserModData = ModData.Manifest.Get<AssetBrowser>();

			foreach (var gameType in typeof(IGameInfo).Assembly.GetTypes())
			{
				if (!gameType.IsClass || !gameType.IsAssignableTo(typeof(IGameInfo)))
					continue;

				var game = (IGameInfo)Activator.CreateInstance(gameType);
				var gamePath = Path.Combine(root, "data", game.Folder);

				if (!Directory.Exists(gamePath))
					continue;

				// Packages
				foreach (var packageLoader in game.PackageLoaders)
				{
					if (ModData.PackageLoaders.Any(e => e.GetType() == packageLoader.GetType()))
						continue;

					ModData.GetType()
						.GetField(nameof(ModData.PackageLoaders))?
						.SetValue(ModData, ModData.PackageLoaders
							.Append(packageLoader).ToArray());

					ModData.ModFiles.GetType()
						.GetField("packageLoaders", BindingFlags.Instance | BindingFlags.NonPublic)?
						.SetValue(ModData.ModFiles, ModData.PackageLoaders);
				}

				foreach (var filter in game.Packages)
				{
					if (filter.Contains('*'))
					{
						foreach (var file in Directory.GetFiles(gamePath, filter, SearchOption.AllDirectories))
							ModData.ModFiles.Mount(file, $"{game.Folder} {Path.GetRelativePath(gamePath, file)}");
					}
					else
					{
						var path = Path.Combine(gamePath, filter);

						if (File.Exists(path) || Directory.Exists(path))
							ModData.ModFiles.Mount(path, $"{game.Folder} {Path.GetRelativePath(gamePath, path)}");
					}
				}

				// Sounds
				foreach (var soundLoader in game.SoundLoaders)
				{
					if (ModData.SoundLoaders != null &&
					    ModData.SoundLoaders.Any(e => e.GetType() == soundLoader.GetType()))
						continue;

					ModData.GetType()
						.GetField(nameof(ModData.SoundLoaders))?
						.SetValue(ModData, (ModData.SoundLoaders ?? Array.Empty<ISoundLoader>())
							.Append(soundLoader).ToArray());
				}

				foreach (var audioExtension in game.AudioExtensions)
				{
					if (assetBrowserModData.AudioExtensions.Contains(audioExtension))
						continue;

					assetBrowserModData.GetType()
						.GetField(nameof(assetBrowserModData.AudioExtensions))?
						.SetValue(assetBrowserModData, assetBrowserModData.AudioExtensions
							.Append(audioExtension).ToArray());
				}

				// Sprites
				foreach (var spriteLoader in game.SpriteLoaders)
				{
					if (ModData.SpriteLoaders != null &&
					    ModData.SpriteLoaders.Any(e => e.GetType() == spriteLoader.GetType()))
						continue;

					ModData.GetType()
						.GetField(nameof(ModData.SpriteLoaders))?
						.SetValue(ModData, (ModData.SpriteLoaders ?? Array.Empty<ISpriteLoader>())
							.Append(spriteLoader).ToArray());
				}

				foreach (var spriteExtension in game.SpriteExtensions)
				{
					if (assetBrowserModData.SpriteExtensions.Contains(spriteExtension))
						continue;

					assetBrowserModData.GetType()
						.GetField(nameof(assetBrowserModData.SpriteExtensions))?
						.SetValue(assetBrowserModData, assetBrowserModData.SpriteExtensions
							.Append(spriteExtension).ToArray());
				}

				// Videos
				foreach (var videoLoader in game.VideoLoaders)
				{
					if (ModData.VideoLoaders != null &&
					    ModData.VideoLoaders.Any(e => e.GetType() == videoLoader.GetType()))
						continue;

					ModData.GetType()
						.GetField(nameof(ModData.VideoLoaders))?
						.SetValue(ModData, (ModData.VideoLoaders ?? Array.Empty<IVideoLoader>())
							.Append(videoLoader).ToArray());
				}

				foreach (var videoExtension in game.VideoExtensions)
				{
					if (assetBrowserModData.VideoExtensions.Contains(videoExtension))
						continue;

					assetBrowserModData.GetType()
						.GetField(nameof(assetBrowserModData.VideoExtensions))?
						.SetValue(assetBrowserModData, assetBrowserModData.VideoExtensions
							.Append(videoExtension).ToArray());
				}
			}

			base.StartGame(args);
		}
	}
}
