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

#region Namespaces
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System.Windows;
#endregion


namespace ParameterSpellChecker
{
    [Transaction(TransactionMode.Automatic)]
    public class Command : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            if (commandData == null || commandData.Application == null || commandData.Application.ActiveUIDocument == null)
            {
                TaskDialog.Show("Revit", "Parameter Spell Checker requires an open document.");
                return Result.Failed;
            }
            IRepository repository = new Repository(commandData);
            initParameterSelectDialog(repository);
            if (m_parameterSelectWindow.DialogResult.Value == false)
            {
                return Result.Cancelled;
            }
            initSpellDialog(repository);


            return Result.Succeeded;
        }

        private void initSpellDialog(IRepository repository)
        {
            m_spellDialog = new SpellDialog(repository);
            m_spellDialogWindow = new Window();
            m_spellDialogWindow.Content = m_spellDialog;
            m_spellDialogWindow.Height = m_spellDialog.Height;
            m_spellDialogWindow.Width = m_spellDialog.Width;
            m_spellDialogWindow.ShowDialog();
        }

        private void initParameterSelectDialog(IRepository repository)
        {
            m_parameterSelectDialog = new ParameterSelectDialog(repository);
            m_parameterSelectWindow = new Window();
            m_parameterSelectWindow.Content = m_parameterSelectDialog;
            m_parameterSelectWindow.Title = "SpellCheck Parameter Selection";
            m_parameterSelectWindow.Width = m_parameterSelectDialog.grid.Width;
            m_parameterSelectWindow.Height = m_parameterSelectDialog.grid.Height;
            m_parameterSelectWindow.ShowDialog();
        }

        private ParameterSelectDialog m_parameterSelectDialog;
        private SpellDialog m_spellDialog;
        private Window m_parameterSelectWindow;
        private Window m_spellDialogWindow;
    }
}
