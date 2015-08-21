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
