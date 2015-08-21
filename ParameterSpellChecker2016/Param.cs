using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParameterSpellChecker
{
    class Param
    {
        private int m_id;
        private string m_paramName;
        private string m_familyName;
        private string m_text;
        private bool m_isChanged = false;

        /// <summary>
        /// default constructor for use in collections;
        /// </summary>
        public Param()
        {
            m_id = 0;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id">Id of the revit parameter</param>
        /// <param name="paramName">Parmaneter Name</param>
        /// <param name="familyName">Family Name of associated element family</param>
        /// <param name="text">The Contents</param>
        public Param(int id, string paramName, string familyName, string text)
        {
            m_id = id;
            m_paramName = paramName;
            m_familyName = familyName;
            m_text = text;
            m_isChanged = false;
        }

        /// <summary>
        /// The Id of the Parameter, corelates with Revit.Parameter.IntegerValue
        /// </summary>
        public int Id
        {
            get
            {
                return m_id;
            }
        }

        public string ParamName
        {
            get { return m_paramName; }
        }

        public string FamilyName
        {
            get { return m_familyName; }
        }

        public string Text
        {
            get { return m_text; }
            set
            {
                m_text = value;
                m_isChanged = true;
            }
        }

        public bool IsChanged
        {
            get { return m_isChanged; }
        }

    }
}
