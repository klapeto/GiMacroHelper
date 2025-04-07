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

using System.Text.Json;
using GiMacroHelper.Model;

namespace GiMacroHelper
{
	/// <summary>
	///     Handles the Loading/Reloading or the macro configuration.
	/// </summary>
	internal class ModelLoader : IDisposable
	{
		private readonly FileSystemWatcher _watcher;

		private DateTime _lastReloadTime = DateTime.MinValue;

		public ModelLoader()
		{
			_watcher = new FileSystemWatcher(Directory.GetCurrentDirectory(), "*.json");
			_watcher.EnableRaisingEvents = true;
			_watcher.IncludeSubdirectories = true;
			_watcher.Changed += WatcherOnChanged;
			Reload();
		}

		/// <summary>
		///     The current loaded configuration.
		/// </summary>
		public MacrosRoot Model { get; private set; }

		/// <summary>
		///     The root filename to load as model.
		/// </summary>
		/// <example>"Config.json"</example>
		public string FileName { get; } = "Config.json";

		/// <summary>
		///     Notifies when the configuration was reloaded from disk.
		/// </summary>
		public EventHandler Reloaded { get; set; }

		/// <inheritdoc />
		public void Dispose()
		{
			_watcher.Dispose();
		}

		private static void ProcessModel(string filePath, MacrosRoot root)
		{
			var macros = root.Macros;

			foreach (var macro in macros)
			{
				macro.Value.Name = macro.Key;
			}

			if (root.Include != null)
			{
				foreach (var include in root.Include)
				{
					MacrosRoot model;
					try
					{
						var directory = Path.GetDirectoryName(filePath);
						model = Load(Path.Combine(directory, Path.GetRelativePath(".", include.Path)));
					}
					catch (Exception e)
					{
						Console.Error.WriteLine(e.Message);
						continue;
					}

					foreach (var macro in model.Macros)
					{
						if (!macros.ContainsKey(macro.Key) || include.Override == true)
						{
							macros[macro.Key] = macro.Value;
							macro.Value.Name = macro.Key;
						}
					}
				}
			}
		}

		private static MacrosRoot Load(string path)
		{
			if (!File.Exists(path))
			{
				throw new FileNotFoundException($"Failed to load '{path}': File does not exist");
			}

			var model = JsonSerializer.Deserialize<MacrosRoot>(
				File.ReadAllText(path),
				new JsonSerializerOptions
				{
					PropertyNameCaseInsensitive = false,
				});
			if (model != null)
			{
				ProcessModel(path, model);
				return model;
			}

			throw new InvalidDataException($"Failed to load '{path}': File empty");
		}

		private void Reload()
		{
			var path = Path.Combine(Directory.GetCurrentDirectory(), FileName);
			try
			{
				Model = Load(path);
				Reloaded?.Invoke(this, EventArgs.Empty);
				Console.WriteLine("Reloaded config");
			}
			catch (Exception e)
			{
				Console.Error.WriteLine(e.Message);
			}
		}

		private void WatcherOnChanged(object sender, FileSystemEventArgs e)
		{
			if (e.ChangeType is WatcherChangeTypes.Changed or WatcherChangeTypes.Created)
			{
				if (DateTime.UtcNow - _lastReloadTime > TimeSpan.FromSeconds(0.5))
				{
					Reload();
					_lastReloadTime = DateTime.UtcNow;
				}
			}
		}
	}
}