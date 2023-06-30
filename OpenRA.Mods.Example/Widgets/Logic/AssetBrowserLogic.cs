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
using System.Reflection;
using OpenRA.FileSystem;
using OpenRA.Graphics;
using OpenRA.Mods.Common.Widgets;
using OpenRA.Mods.Example.Extractors;
using OpenRA.Support;
using OpenRA.Widgets;

namespace OpenRA.Mods.Example.Widgets.Logic
{
	public class AssetBrowserLogic : Common.Widgets.Logic.AssetBrowserLogic
	{
		const BindingFlags Flags = BindingFlags.Instance | BindingFlags.NonPublic;

		[ObjectCreator.UseCtor]
		public AssetBrowserLogic(Widget widget, Action onExit, ModData modData, WorldRenderer worldRenderer)
			: base(widget, onExit, modData, worldRenderer)
		{
			var closeButton = widget.GetOrNull<ButtonWidget>("CLOSE_BUTTON");

			var extractButton = (ButtonWidget)closeButton.Clone();

			extractButton.Id = "EXTRACT_BUTTON";
			extractButton.X = new IntegerExpression($"{closeButton.X.Expression} - {closeButton.Width.Expression} - 20");
			extractButton.Text = "Extract";
			extractButton.GetText = () => extractButton.Text;
			extractButton.OnClick = Extract;

			closeButton.Parent.AddChild(extractButton);

			extractButton.Initialize(new WidgetArgs());

			var sourceDropdown = widget.GetOrNull<DropDownButtonWidget>("SOURCE_SELECTOR");
			sourceDropdown.Width = new IntegerExpression($"{sourceDropdown.Width.Expression} + 150");
			sourceDropdown.Initialize(new WidgetArgs());
		}

		void Extract()
		{
			if (GetType().BaseType?.GetField("currentFilename", Flags)?.GetValue(this) is not string currentFilename)
				return;

			if (GetType().BaseType?.GetField("currentSprites", Flags)?.GetValue(this) is Sprite[] sprites)
				SpriteExtractor.Extract(sprites, currentFilename);
			else if (GetType().BaseType?.GetField("currentSoundFormat", Flags)?.GetValue(this) is ISoundFormat audio)
				AudioExtractor.Extract(audio, currentFilename);
			else if (GetType().BaseType?.GetField("currentSprites", Flags)?.GetValue(this) is VideoPlayerWidget[])
			{
				// TODO not extracting videos for now!
			}
			else if (GetType().BaseType?.GetField("currentPackage", Flags)?.GetValue(this) is IReadOnlyPackage package)
				RawExtractor.Extract(package, currentFilename);
		}
	}
}
