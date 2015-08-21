/*
 * Copyright (c) 2015 Dave French <contact/dot/dave/dot/french3/at/googlemail/dot/com>
 *
 * This file is part of RevitParameterSpellCheck https://github.com/curlymorphic/RevitParameterSpellcheck
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public
 * License as published by the Free Software Foundation; either
 * version 2 of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * General Public License for more details.
 *
 * You should have received a copy of the GNU General Public
 * License along with this program (see COPYING); if not, write to the
 * Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
 * Boston, MA 02110-1301 USA.
 *
 */


using System;
namespace ParameterSpellChecker
{
    public interface IRepository
    {
        System.Collections.ObjectModel.Collection<string> AllElementTextParameterNames { get;  }
        System.Collections.ObjectModel.Collection<string> SelectedElementsTextParameterNames { get; }
        System.Collections.ObjectModel.Collection<String> SheetTextParameterNames { get; }
        System.Collections.ObjectModel.Collection<String> ViewTextParameterNames { get; }
        System.Collections.ObjectModel.Collection<String> ViewportTextParameterNames { get; }
        int SelectionCount { get; }
        void InitializeSpellCheck(System.Collections.ObjectModel.Collection<string> parameterNames, SelectionType selectionType);
        System.Collections.Generic.IEnumerator<Autodesk.Revit.DB.Parameter> NextParameter();
    }
}
