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

namespace GiMacroHelper.Model
{
	/// <summary>
	///     Defines a macro.
	/// </summary>
	internal class Macro
	{
		/// <summary>
		///     The name of the macro.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		///     The collection of actions to perform.
		/// </summary>
		public MacroAction[] Actions { get; set; } = [];

		/// <summary>
		///     The string of the shortcut to trigger the macro.
		/// </summary>
		/// <example>"Alt+/"</example>
		public string? ShortCut { get; set; }
	}
}