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


using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ParameterSpellChecker
{
    /// <summary>
    /// Interaction logic for SpellDialog.xaml
    /// </summary>
    public partial class SpellDialog : UserControl
    {
        public SpellDialog(IRepository repository)
        {
            InitializeComponent();
            if (repository == null)
            {
                throw new ArgumentException("Null repository");
            }
            m_repository = repository;
            m_params = (IEnumerator<Parameter>)m_repository.NextParameter();
            nextParameter();
        }

        private IRepository m_repository;
        private Parameter m_parameter;
        private IEnumerator<Parameter> m_params;
        private const string END_OF_DOCUMENT = "End Of Document";

        private void nextParameter()
        {
            if (m_params.MoveNext())
            {
                do
                {
                    m_parameter = m_params.Current;
                    FamilyTB.Text = m_parameter.Element as FamilyInstance != null ?
                        ((FamilyInstance)m_parameter.Element).Name :
                        null;
                    TypeTB.Text = m_parameter.Element as FamilyInstance != null ?
                        ((FamilyInstance)m_parameter.Element).GetParameters("Type").FirstOrDefault().AsString() :
                        null;
                    PararmeterTB.Text = m_parameter.Definition.Name;
                    ContextTB.Clear();
                    ContextTB.Text = m_parameter.HasValue ?
                        m_parameter.AsString() :
                        String.Empty;
                } while ( ContextTB.GetNextSpellingErrorCharacterIndex(0, LogicalDirection.Forward) == -1 && m_params.MoveNext());
            }
            else
            {
                nextBtn.IsEnabled = false;
                FamilyTB.Clear();
                TypeTB.Clear();
                ContextTB.Clear();
                ContextTB.Text = END_OF_DOCUMENT;
                ContextTB.IsEnabled = false;
            }
        }

        private void nextBtn_Click(object sender, RoutedEventArgs e)
        {
            nextParameter();
        }

        private void ContextTB_TextChanged(object sender, TextChangedEventArgs e)
        {
           
        }

        private void ContextTB_LostFocus(object sender, RoutedEventArgs e)
        {
            if (m_parameter != null && !m_parameter.AsString().Equals(ContextTB.Text))
            {
                m_parameter.Set(ContextTB.Text);
            }
        }

        private void closeBtn_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).DialogResult = true;
            Window.GetWindow(this).Close();
        }
    }
}
