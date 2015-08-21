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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System.Collections.ObjectModel;

namespace ParameterSpellChecker
{
    /// <summary>
    /// Wrapper Class that is responsiable for all communication with revit
    /// </summary>
    class Repository : ParameterSpellChecker.IRepository
    {
        public Repository(ExternalCommandData commandData)
        {
            m_commandData = commandData;
            m_uiapp = m_commandData.Application;
            m_uidoc = m_uiapp.ActiveUIDocument;
            m_doc = m_uidoc.Document;
        }

        /// <summary>
        /// Gets all unique parameter names, that contain text, in the current document
        /// </summary>
        /// <returns></returns>
        public Collection<String> AllElementTextParameterNames
        {
            get
            {
                Collection<String> paramNames = new Collection<String>();

                using (FilteredElementCollector collector
                  = new FilteredElementCollector(m_doc)
                    .WhereElementIsNotElementType()
                    .OfCategory(BuiltInCategory.INVALID))
                {

                    addParameterNamesToList(paramNames, collector);
                }
                return paramNames;
            }
        }

        /// <summary>
        /// Gets all unique parameter names, that contain text, in the document sheets
        /// </summary>
        /// <returns></returns>
        public Collection<String> SheetTextParameterNames
        {
            get
            {
                Collection<String> paramNames = new Collection<String>();

                using (FilteredElementCollector collector
                  = new FilteredElementCollector(m_doc)
                    .WhereElementIsNotElementType()
                    .OfCategory(BuiltInCategory.OST_Sheets))
                {

                    addParameterNamesToList(paramNames, collector);
                }
                return paramNames;
            }
        }

        /// <summary>
        /// Gets all unique parameter names, that contain text, in the views
        /// </summary>
        /// <returns></returns>
        public Collection<String> ViewTextParameterNames
        {
            get
            {
                Collection<String> paramNames = new Collection<String>();

                using (FilteredElementCollector collector
                  = new FilteredElementCollector(m_doc)
                    .WhereElementIsNotElementType()
                    .OfCategory(BuiltInCategory.OST_Views))
                {

                    addParameterNamesToList(paramNames, collector);
                }
                return paramNames;
            }
        }

        /// <summary>
        /// Gets all unique parameter names, that contain text, in the viewports
        /// </summary>
        /// <returns></returns>
        public Collection<String> ViewportTextParameterNames
        {
            get
            {
                Collection<String> paramNames = new Collection<String>();

                using (FilteredElementCollector collector
                  = new FilteredElementCollector(m_doc)
                    .WhereElementIsNotElementType()
                    .OfCategory(BuiltInCategory.OST_Viewports))
                {

                    addParameterNamesToList(paramNames, collector);
                }
                return paramNames;
            }
        }

        private static void addParameterNamesToList(Collection<String> paramNames, FilteredElementCollector collector)
        {
            foreach (Element e in collector)
            {
                foreach (Parameter p in e.Parameters)
                {
                    if (!paramNames.Contains(p.Definition.Name)
                        && p.Definition.ParameterType == ParameterType.Text && !p.IsReadOnly)
                    {
                        paramNames.Add(p.Definition.Name);
                    }
                }
            }
        }

        /// <summary>
        /// Gets all the unique parameter names, that contain text, in the curent selection
        /// </summary>
        /// <returns></returns>
        public Collection<string> SelectedElementsTextParameterNames
        {
            get
            {
                Collection<String> paramNames = new Collection<string>();
                Selection selection = m_uidoc.Selection;
                ICollection<ElementId> elementIds = selection.GetElementIds();
                foreach (ElementId eid in elementIds)
                {
                    Element e = m_doc.GetElement(eid);
                    if (e != null)
                    {
                        foreach (Parameter p in e.Parameters)
                        {
                            if (!paramNames.Contains(p.Definition.Name) && p.Definition.ParameterType == ParameterType.Text && !p.IsReadOnly)
                            {
                                paramNames.Add(p.Definition.Name);
                            }
                        }
                    }
                }
                return paramNames;
            }
        }

        public int SelectionCount
        {
            get
            {
                return m_uidoc.Selection.GetElementIds().Count();
            }
        }

        private ExternalCommandData m_commandData;
        private UIApplication m_uiapp;
        private UIDocument m_uidoc;
        private Document m_doc;
        private List<string> m_selectedParameterNames = new List<string>();
        private SelectionType m_selectionType;


        public void InitializeSpellCheck(Collection<string> parameterNames, SelectionType selectionType)
        {
            if (parameterNames != null)
            {
                m_selectedParameterNames = parameterNames.ToList<string>();
            }
            else
            {
                throw new ArgumentException("Null parameter names list");
            }
            m_selectionType = selectionType;
        }



        public IEnumerator<Parameter> NextParameter()
        {
            FilteredElementCollector collector;
            switch (m_selectionType)
            {
                case SelectionType.Selection:
                    Selection selection = m_uidoc.Selection;
                    ICollection<ElementId> elementIds = selection.GetElementIds();
                    foreach (ElementId eid in elementIds)
                    {
                        Element e = m_doc.GetElement(eid);
                        if (e != null)
                        {
                            foreach (Parameter p in e.Parameters)
                            {
                                if (m_selectedParameterNames.Contains(p.Definition.Name) && !p.IsReadOnly)
                                {
                                    yield return p;
                                }
                            }
                        }
                    }
                    break;
                case SelectionType.Sheets:
                    using (collector = new FilteredElementCollector(m_doc)
                                    .WhereElementIsNotElementType()
                                    .OfCategory(BuiltInCategory.OST_Sheets))
                    {

                        foreach (Element e in collector)
                        {
                            foreach (Parameter p in e.Parameters)
                            {
                                if (m_selectedParameterNames.Contains(p.Definition.Name) && !p.IsReadOnly)
                                {
                                    yield return p;
                                }
                            }
                        }
                    }
                    break;

                case SelectionType.Viewport:
                    using (collector = new FilteredElementCollector(m_doc)
                                   .WhereElementIsNotElementType()
                                   .OfCategory(BuiltInCategory.OST_Viewports))
                    {

                        foreach (Element e in collector)
                        {
                            foreach (Parameter p in e.Parameters)
                            {
                                if (m_selectedParameterNames.Contains(p.Definition.Name) && !p.IsReadOnly)
                                {
                                    yield return p;
                                }
                            }
                        }
                    }
                    break;

                case SelectionType.Views:
                    using (collector = new FilteredElementCollector(m_doc)
                                    .WhereElementIsNotElementType()
                                    .OfCategory(BuiltInCategory.OST_Views))
                    {

                        foreach (Element e in collector)
                        {
                            foreach (Parameter p in e.Parameters)
                            {
                                if (m_selectedParameterNames.Contains(p.Definition.Name) && !p.IsReadOnly)
                                {
                                    yield return p;
                                }
                            }
                        }
                    }
                    break;

                case SelectionType.All:
                    using (collector = new FilteredElementCollector(m_doc)
                                    .WhereElementIsNotElementType()
                                    .OfCategory(BuiltInCategory.INVALID))
                    {

                        foreach (Element e in collector)
                        {
                            foreach (Parameter p in e.Parameters)
                            {
                                if (m_selectedParameterNames.Contains(p.Definition.Name) && !p.IsReadOnly)
                                {
                                    yield return p;
                                }
                            }
                        }
                    }
                    break;
                    
            }
            
        }
    }
}
