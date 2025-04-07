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

using SharpHook;

namespace GiMacroHelper
{
	internal static class Program
	{
		public static int Main(string[] args)
		{
			using var modelLoader = new ModelLoader();

			using var hook = new TaskPoolGlobalHook();

			using var macroExecutor = new MacroExecutor(modelLoader);

			var shortCutHandler = new ShortcutHandler(modelLoader, hook);
			shortCutHandler.MacroTriggered += (_, tuple) =>
			{
				Console.WriteLine($"Macro triggered: {tuple.macro.Name}");
				macroExecutor.Execute(tuple.macro);
			};

			hook.Run();

			return 0;
		}
	}
}
