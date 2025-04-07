// =========================================================================
// 
// GiMacroHelper
// 
// Copyright (C) 2025 Ioannis Panagiotopoulos
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>
// =========================================================================

using GiMacroHelper.Model;
using SharpHook;
using SharpHook.Native;

namespace GiMacroHelper
{
	/// <summary>
	///     Handles the shortcuts and raises events when a shortcut is triggered.
	/// </summary>
	internal class ShortcutHandler
	{
		private readonly IGlobalHook _globalHook;
		private readonly object _locker = new();
		private readonly ModelLoader _modelLoader;
		private readonly List<ShortcutInstance> _shortcuts = new();

		public ShortcutHandler(ModelLoader modelLoader, IGlobalHook globalHook)
		{
			_modelLoader = modelLoader;
			_globalHook = globalHook;

			_globalHook.KeyTyped += GlobalHookOnKeyReleased;
			_modelLoader.Reloaded += Reloaded;
			ReRegisterShortcuts();
		}

		/// <summary>
		///     Raised when a macro needs to run due to shortcut trigger.
		/// </summary>
		public EventHandler<(MacrosRoot root, Macro macro)> MacroTriggered { get; set; }

		private void Reloaded(object? sender, EventArgs e)
		{
			ReRegisterShortcuts();
		}

		private void ReRegisterShortcuts()
		{
			lock (_locker)
			{
				_shortcuts.Clear();
				foreach (var macro in _modelLoader.Model.Macros)
				{
					RegisterShortCut(macro.Key, macro.Value.ShortCut, macro.Value);
				}

				foreach (var shortCut in _modelLoader.Model.ShortCuts)
				{
					if (!_modelLoader.Model.Macros.TryGetValue(shortCut.Value, out var macroValue))
					{
						Console.WriteLine(
							$"General shortcut '{shortCut.Key}' points to no existing macro '{shortCut.Value}'");
						continue;
					}

					RegisterShortCut(shortCut.Value, shortCut.Key, macroValue);
				}
			}
		}

		private void GlobalHookOnKeyReleased(object? sender, KeyboardHookEventArgs e)
		{
			var ch = (char)e.Data.RawCode;
			ShortcutInstance? shortcut;
			lock (_locker)
			{
				shortcut = _shortcuts.Find(s => s.Character == ch && (s.Mask?.HasAny(e.RawEvent.Mask) ?? true));
			}

			if (shortcut != null)
			{
				MacroTriggered?.Invoke(this, (_modelLoader.Model, shortcut.Macro));
			}
		}

		private void RegisterShortCut(string name, string? shortCut, Macro macro)
		{
			if (string.IsNullOrWhiteSpace(shortCut))
			{
				return;
			}

			var tokens = shortCut.Split('+').Select(t => t.Trim()).ToArray();
			if (tokens.Length == 0)
			{
				Console.Error.Write($"Failed to register shortcut for '{name}': empty shortcut");
			}
			else if (tokens.Length == 1)
			{
				_shortcuts.Add(new ShortcutInstance(macro, tokens[0].First(), null));
			}
			else
			{
				var value = 0;
				var valid = true;
				var invalidTokens = new List<string>();
				for (var i = 0; i < tokens.Length; i++)
				{
					var token = tokens[i];
					if (!Enum.TryParse(typeof(ModifierMask), token, true, out var mask))
					{
						if (i != tokens.Length - 1)
						{
							valid = false;
							invalidTokens.Add(token);
						}
					}
					else
					{
						value |= (int)(ModifierMask)mask;
					}
				}

				if (!valid)
				{
					Console.Error.Write(
						$"Failed to register shortcut for '{name}': invalid tokens: '{string.Join(',', invalidTokens)}'");
				}
				else
				{
					_shortcuts.Add(new ShortcutInstance(macro, tokens.Last().First(), (ModifierMask)value));
				}
			}
		}

		private class ShortcutInstance
		{
			public ShortcutInstance(Macro macro, char character, ModifierMask? mask)
			{
				Character = character;
				Mask = mask;
				Macro = macro;
			}

			public ModifierMask? Mask { get; }

			public char Character { get; }

			public Macro Macro { get; }
		}
	}
}