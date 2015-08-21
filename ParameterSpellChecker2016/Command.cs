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
