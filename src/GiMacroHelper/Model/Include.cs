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
	///     Defines a file inclusion
	/// </summary>
	internal class Include
	{
		public Include(string path, bool? @override)
		{
			Path = path;
			Override = @override;
		}

		/// <summary>
		///     The relative or absolute path of the file to load.
		/// </summary>
		/// <example>"./additional-macros.json"</example>
		public string Path { get; }

		/// <summary>
		///     Whether macros with existing names should be overriden by the ones in this file.
		/// </summary>
		public bool? Override { get; }
	}
}