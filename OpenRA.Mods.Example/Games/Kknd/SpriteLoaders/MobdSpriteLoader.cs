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
using System.IO;
using System.Linq;
using OpenRA.Graphics;
using OpenRA.Mods.Example.Games.Kknd.FileFormats;
using OpenRA.Primitives;

namespace OpenRA.Mods.Example.Games.Kknd.SpriteLoaders
{
	public class MobdSpriteLoader : ISpriteLoader
	{
		sealed class MobdSpriteFrame : ISpriteFrame
		{
			public SpriteFrameType Type => SpriteFrameType.Rgba32;
			public Size Size { get; }
			public Size FrameSize { get; }
			public float2 Offset { get; }
			public byte[] Data { get; }

			public bool DisableExportPadding => true;

			public MobdSpriteFrame(MobdImageVariation imageVariation, MobdImage image, MobdFrame frame, Color[] palette)
			{
				var pixels = image.Pixels;

				if (imageVariation.Mirrored)
				{
					var mirrored = new byte[pixels.Length];

					for (var y = 0; y < image.Height; y++)
					{
						Array.Copy(
							pixels.Skip((int)(y * image.Width)).Take((int)image.Width).Reverse().ToArray(),
							0,
							mirrored,
							y * image.Width,
							image.Width);
					}

					pixels = mirrored;
				}

				FrameSize = Size = new Size((int)image.Width, (int)image.Height);
				Offset = new int2(Size.Width / 2 - frame.OffsetX, Size.Height / 2 - frame.OffsetY);
				Data = pixels.SelectMany(pixel =>
				{
					return new[]
					{
						palette[pixel].R,
						palette[pixel].G,
						palette[pixel].B,
						palette[pixel].A
					};
				}).ToArray();
			}
		}

		public bool TryParseSprite(Stream s, string filename, out ISpriteFrame[] frames, out TypeDictionary metadata)
		{
			if (!filename.EndsWith($".{Mobd.Type}") || s is not Lvl.LvlStream lvlStream)
			{
				metadata = null;
				frames = null;

				return false;
			}

			// This is damn ugly, but MOBD uses offsets from LVL start.
			lvlStream.BaseStream.Position = lvlStream.BaseOffset;
			var mobd = new Mobd(lvlStream, lvlStream.AssetFormat);

			Color[] palette = null;
			var id = int.Parse(filename.Split("|").Last()[..^5]);

			if (lvlStream.AssetFormat == AssetFormat.Kknd1)
			{
				// Hack: SUPER frames using different palette...
				palette = (lvlStream.Container.Contains("supspr", StringComparison.OrdinalIgnoreCase) &&
				           id is 45 or 46 or 79 or 80)
					? GetSuperPalette()
					: GetPlayerPalette();
			}
			else
			{
				// Hack: Gamesprt frames using different palettes...
				if (lvlStream.Container.Contains("gamesprt", StringComparison.OrdinalIgnoreCase))
				{
					palette = id switch
					{
						23 or (>= 31 and <= 40) or 95 => GetMuteBPalette(),
						(>= 110 and <= 119) or 176 => GetRoboBPalette(),
						(>= 182 and <= 191) or 237 or 249 => GetSurvBPalette(),
						>= 54 and <= 64 => GetMuteTPalette(),
						>= 133 and <= 142 => GetRoboTPalette(),
						>= 207 and <= 216 => GetSurvTPalette(),
						_ => null
					};
				}
			}

			var tmp = new List<ISpriteFrame>();

			var animations = new List<MobdAnimation>();
			animations.AddRange(mobd.OrderedAnimations.Select(e => mobd.Animations[e]));
			animations.AddRange(mobd.Animations.Values.Where(e => !animations.Contains(e)));

			foreach (var animationFrames in animations
				         .Select(animation => (animation.Frames.Distinct().Count() == 1
						         ? animation.Frames.Take(1)
						         : animation.Frames)
					         .Select(f => mobd.Frames[f])))
			{
				tmp.AddRange(animationFrames.Select(frame =>
				{
					var imageVariation = mobd.ImageVariations[frame.ImageVariation];
					var image = mobd.Images[imageVariation.Image];

					// HACK: Holdingpen is hardcoded against player index 2...
					if (lvlStream.AssetFormat == AssetFormat.Kknd1 &&
					    lvlStream.Container.Contains("sprites", StringComparison.OrdinalIgnoreCase) && id == 32)
					{
						for (var k = 0; k < image.Pixels.Length; k++)
						{
							if (image.Pixels[k] >= 16 && image.Pixels[k] < 32)
								image.Pixels[k] -= 16;
						}
					}

					if (palette != null)
						return new MobdSpriteFrame(imageVariation, image, frame, palette);

					palette = imageVariation.Palette != null
						? mobd.Palettes[imageVariation.Palette.Value].Palette
						: throw new Exception("Unknown palette");

					return new MobdSpriteFrame(imageVariation, image, frame, palette);
				}));
			}

			frames = tmp.Select(e => e).ToArray();
			metadata = new TypeDictionary();

			return true;
		}

		static Color[] GetSuperPalette()
		{
			foreach (var package in Game.ModData.ModFiles.MountedPackages)
			{
				if (!package.Name.EndsWith("super.lvl", StringComparison.OrdinalIgnoreCase))
					continue;

				var map = new Mapd(package.GetStream("0.MAPD"), AssetFormat.Kknd1);

				var superPalette = map.Palette;
				superPalette[0] = Color.Transparent;

				return superPalette;
			}

			return null;
		}

		static Color[] GetPlayerPalette()
		{
			foreach (var package in Game.ModData.ModFiles.MountedPackages)
			{
				if (!package.Name.EndsWith("mute_01.lvl", StringComparison.OrdinalIgnoreCase))
					continue;

				var map = new Mapd(package.GetStream("0.MAPD"), AssetFormat.Kknd1);

				var playerPalette = map.Palette;

				for (var i = 0; i < 8; i++)
					playerPalette[i * 16] = Color.Transparent;

				return playerPalette;
			}

			return null;
		}

		static IEnumerable<Color> GetBPalette()
		{
			var palette = new Pal(Game.ModData.ModFiles.Open("spriteb.pal"));
			var bPalette = palette.Colors;

			for (var i = 0; i < 24; i++)
				bPalette[i * 256] = Color.Transparent;

			return bPalette;
		}

		static Color[] GetMuteBPalette()
		{
			return GetBPalette().Skip(8 * 256).Take(256).ToArray();
		}

		static Color[] GetRoboBPalette()
		{
			return GetBPalette().Skip(16 * 256).Take(256).ToArray();
		}

		static Color[] GetSurvBPalette()
		{
			return GetBPalette().Skip(0 * 256).Take(256).ToArray();
		}

		static IEnumerable<Color> GetTPalette()
		{
			var palette = new Pal(Game.ModData.ModFiles.Open("spritet.pal"));

			var tPalette = palette.Colors;

			for (var i = 0; i < 24; i++)
				tPalette[i * 64] = Color.Transparent;

			return tPalette;
		}

		static Color[] GetMuteTPalette()
		{
			return GetTPalette().Skip(8 * 64).Take(64).ToArray();
		}

		static Color[] GetRoboTPalette()
		{
			return GetTPalette().Skip(16 * 64).Take(64).ToArray();
		}

		static Color[] GetSurvTPalette()
		{
			return GetTPalette().Skip(0 * 64).Take(64).ToArray();
		}
	}
}
