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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    public enum SelectionType { Selection, Sheets, Views, Viewport, All };

    /// <summary>
    /// Interaction logic for ParameterSelectDialog.xaml
    /// </summary>
    public partial class ParameterSelectDialog : UserControl
    {
        private SelectionType m_selectionType = SelectionType.Selection;

        public ParameterSelectDialog(IRepository repository)
        {
            InitializeComponent();
            m_repository = repository;
            if (m_repository == null) throw new ArgumentException("Invalid Repository");
            switch (m_selectionType)
            {
                case SelectionType.Selection:
                    selectionRB.IsChecked = true;
                    break;
                case SelectionType.Sheets:
                    sheetRB.IsChecked = true;
                    break;
                case SelectionType.Views:
                    viewRB.IsChecked = true;
                    break;
                case SelectionType.Viewport:
                    viewportRB.IsChecked = true;
                    break;
                case SelectionType.All:
                    allRB.IsChecked = true;
                    break;
            }

            setParamNames();
            selectionChanged();
        }

        private void setParamNames()
        {

            if (m_repository != null)
            {
                switch (m_selectionType)
                {
                    case SelectionType.Selection:
                        m_avaliableParameters = m_repository.SelectedElementsTextParameterNames.ToList<String>();
                        break;
                    case SelectionType.Sheets:
                        m_avaliableParameters = m_repository.SheetTextParameterNames.ToList<String>();
                        break;
                    case SelectionType.Views:
                        m_avaliableParameters = m_repository.ViewTextParameterNames.ToList<String>();
                        break;
                    case SelectionType.Viewport:
                        m_avaliableParameters = m_repository.ViewportTextParameterNames.ToList<String>();
                        break;
                    case SelectionType.All:
                        m_avaliableParameters = m_repository.AllElementTextParameterNames.ToList<String>();
                        break;
                }
                refreshBindings();
            }
        }

        private IRepository m_repository;

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            addSelected();
        }

        /// <summary>
        /// Adds the selected parameters to the selected list
        /// and removes them from the avaliable list
        /// </summary>
        private void addSelected()
        {
            var avaliableSelection = AvaliableLB.SelectedItems;
            foreach (String parameter in avaliableSelection)
            {
                m_selectedParameters.Add(parameter);
                m_avaliableParameters.Remove(parameter);
            }
            refreshBindings();
            selectionChanged();
        }

        /// <summary>
        /// Dirty hack to refresh listboxes
        /// </summary>
        private void refreshBindings()
        {
            m_avaliableParameters.Sort();
            m_selectedParameters.Sort();
            AvaliableLB.ItemsSource = null;
            SelectedLB.ItemsSource = null;
            AvaliableLB.ItemsSource = m_avaliableParameters;
            SelectedLB.ItemsSource = m_selectedParameters;
        }


        /// <summary>
        /// updates the ui controls
        /// </summary>
        private void selectionChanged()
        {
            OkBtn.IsEnabled = m_selectedParameters.Count != 0;
        }

        private void AvaliableLB_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            addSelected();
        }

        private List<String> m_avaliableParameters = new List<string>();
        private List<String> m_selectedParameters = new List<string>();

        private void RemoveBtn_Click(object sender, RoutedEventArgs e)
        {
            removeSelected();
        }

        /// <summary>
        /// remove a paramter from the selected list, and return it to the avaliable list
        /// </summary>
        private void removeSelected()
        {
            var selectedSelection = SelectedLB.SelectedItems;
            foreach (String parameter in selectedSelection)
            {
                m_selectedParameters.Remove(parameter);
                m_avaliableParameters.Add(parameter);
            }
            refreshBindings();
            selectionChanged();
        }

        private void SelectedLB_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            removeSelected();
        }

        private void NoneBtn_Click(object sender, RoutedEventArgs e)
        {
            removeAll();
        }

        /// <summary>
        /// removes all the selected parameters, return them to the avaliable list
        /// </summary>
        private void removeAll()
        {
            m_avaliableParameters.AddRange(m_selectedParameters);
            m_selectedParameters.Clear();
            refreshBindings();
            selectionChanged();
        }

        private void AllBtn_Click(object sender, RoutedEventArgs e)
        {
            addAll();
        }

        /// <summary>
        /// Adds all the parameters to the selected list, 
        /// removing them from the avaiable list
        /// </summary>
        private void addAll()
        {
            m_selectedParameters.AddRange(m_avaliableParameters);
            m_avaliableParameters.Clear();
            refreshBindings();
            selectionChanged();
        }

        private void OkBtn_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).DialogResult = true;
            Window.GetWindow(this).Close();
            m_repository.InitializeSpellCheck(new Collection<String>(m_selectedParameters), m_selectionType);
        }

        /// <summary>
        /// Use the selected elements, false indicates the use of all elements
        /// </summary>


        public SelectionType SelectionType
        {
            get { return m_selectionType; }
        }

        /// <summary>
        /// List of the selected Parameter names
        /// </summary>
        public IList<String> SelectedParameterNames
        {
            get
            {
                return new List<String>(m_selectedParameters);
            }
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).DialogResult = false;
            Window.GetWindow(this).Close();
        }

        private void selectionRB_Checked(object sender, RoutedEventArgs e)
        {
            m_selectionType = SelectionType.Selection;
            setParamNames();
        }

        private void sheetRB_Checked(object sender, RoutedEventArgs e)
        {
            m_selectionType = SelectionType.Sheets;
            setParamNames();
        }

        private void viewRB_Checked(object sender, RoutedEventArgs e)
        {
            m_selectionType = SelectionType.Views;
            setParamNames();
        }

        private void viewportRB_Checked(object sender, RoutedEventArgs e)
        {
            m_selectionType = SelectionType.Viewport;
            setParamNames();
        }

        private void allRB_Checked(object sender, RoutedEventArgs e)
        {
            m_selectionType = SelectionType.All;
            setParamNames();
        }
    }
}
