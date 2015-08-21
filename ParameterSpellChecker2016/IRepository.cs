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
