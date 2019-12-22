#region Namespaces

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Application = Autodesk.Revit.ApplicationServices.Application;

#endregion

namespace QApps
{
    public class QuickFilterViewModel : ViewModelBase
    {
        private List<string> allValueParameters01;
        private List<string> allValueParameters02;
        private List<string> allValueParameters03;
        private string _selectedParameter01;
        private string _selectedParameter02;
        private string _selectedParameter03;
        private bool isCurrentView;
       
        protected internal ExternalEvent ExEvent;
        private UIApplication uiapp;
        private UIDocument uidoc;
        private Application app;
        private Document doc;

        #region private property
        // Tất cả Instance Element
        private List<Element> _allInstanceElements = new List<Element>();
        // Tất cả FamilySymbol của các Instance Element
        private List<ElementType> _allFamilySymbol
            = new List<ElementType>();

        private List<ElementId> _sameElements = new List<ElementId>();
        private List<ElementId> currentSelection = new List<ElementId>();

        SKRrYXKUnVmrC_L dlqConstraint = new SKRrYXKUnVmrC_L();
        private string _selectedValueParameter01;
        private string _selectedValueParameter02;
        private List<string> _allParameters02 = new List<string>();
        private List<string> _allParameters03 = new List<string>();
        private bool _isCurrentSelection;
        private bool _isCurrentModel;

        #endregion private

        #region public property
        public List<string> AllParameters01 { get; set; } = new List<string>();

        public List<string> AllParameters02
        {
            get { return _allParameters02; }
            set
            {
                _allParameters02 = value;
                OnPropertyChanged();
            }
        }

        public List<string> AllParameters03
        {
            get { return _allParameters03; }
            set
            {
                _allParameters03 = value;
                OnPropertyChanged();
            }
        }

        public string SelectedParameter01
        {
            get { return _selectedParameter01; }
            set
            {
                _selectedParameter01 = value;
                UpdateAllValueParameters01();
                //UpdateAllParameters02();
            }
        }

        public string SelectedParameter02
        {
            get { return _selectedParameter02; }
            set
            {
                _selectedParameter02 = value;
                UpdateAllValueParameters02();
                //UpdateAllParameters03();

            }
        }
        public string SelectedParameter03
        {
            get { return _selectedParameter03; }
            set
            {
                _selectedParameter03 = value;
                UpdateAllValueParameters03();
            }
        }

        public List<string> AllValueParameters01
        {
            get { return allValueParameters01; }

            set
            {
                allValueParameters01 = value;
                OnPropertyChanged();
            }
        }

        public List<string> AllValueParameters02
        {
            get { return allValueParameters02; }

            set
            {
                allValueParameters02 = value;
                OnPropertyChanged();
            }
        }
        public List<string> AllValueParameters03
        {
            get { return allValueParameters03; }

            set
            {
                allValueParameters03 = value;
                OnPropertyChanged();
            }
        }

        public string SelectedValueParameter01
        {
            get { return _selectedValueParameter01; }
            set
            {
                _selectedValueParameter01 = value;
                //UpdateAllParameters02();
                UpdateAllValueParameters02();
                UpdateAllValueParameters03();

            }
        }

        public string SelectedValueParameter02
        {
            get { return _selectedValueParameter02; }
            set
            {
                _selectedValueParameter02 = value;
                //UpdateAllParameters03();
                UpdateAllValueParameters03();
            }
        }

        public string SelectedValueParameter03 { get; set; }

        public bool IsCurrentView
        {
            get { return isCurrentView; }

            set
            {
                isCurrentView = value;
                UpdateScope();
            }
        }

        public bool IsCurrentSelection
        {
            get { return _isCurrentSelection; }
            set
            {
                _isCurrentSelection = value;
                UpdateScope();
            }
        }

        public bool IsCurrentModel
        {
            get { return _isCurrentModel; }
            set
            {
                _isCurrentModel = value;
                UpdateScope();
            }
        }

        #endregion public property


        //public QuickFilterViewModel(UIDocument uidoc)
        //{
        //    Doc = uidoc.Document;
        //    UiDoc = uidoc;
        //    Initialize();
        //}
        public QuickFilterViewModel(ExternalCommandData commandData)
        {
            QuickFilterHandler handler = new QuickFilterHandler();
            ExEvent = ExternalEvent.Create(handler);
            handler.ViewModel = this;

            uiapp = commandData.Application;
            uidoc = uiapp.ActiveUIDocument;
            app = uiapp.Application;
            doc = uidoc.Document;

            Initialize();
            ShowWindow(commandData);
        }

        private void Initialize()
        {
            AllParameters01 = doc.GetAllParameters();
            AllParameters01 = AllParameters01.Where(p => !p.Contains("Analytical")).ToList();
            AllParameters01.Insert(0, "");
            AllParameters02 = AllParameters03 = AllParameters01;
        }

        public void ShowWindow(ExternalCommandData commandData)
        {
            Process[] processlist = Process.GetProcesses();
            foreach (var p in processlist)
            {
                if (p.MainWindowTitle.Equals(mpsDXTeybJAfSnv.QuickSelect))
                {
                    WindowHelper.BringProcessToFront(p);
                    return;
                }
            }

            var window = new QuickFilterWindow(this);
            window.Show();
        }

        public void UpdateAllValueParameters01()
        {
            if (string.IsNullOrEmpty(SelectedParameter01))
                return;

            List<string> list = _allInstanceElements.Select(e =>
                e.LookupParameter(SelectedParameter01)?.GetValue(true))
                .ToList();

            //if (list.Count > 0)
            //    list = list.Distinct().ToList();

            List<string> list1 = _allFamilySymbol.Select(e =>
                      e?.LookupParameter(SelectedParameter01)?.GetValue(true))
                 .ToList();

            //if (list1.Count > 0)
            //    list1 = list1.Distinct().ToList();

            list.AddRange(list1);
            if (list.Count > 0)
                list = list.Distinct().ToList();

            AllValueParameters01 = list;
            AllValueParameters01.Sort();
        }
        public void UpdateAllValueParameters02()
        {
            if (string.IsNullOrEmpty(SelectedParameter01) ||
                string.IsNullOrEmpty(SelectedParameter02))
            {
                AllValueParameters02 = new List<string>();
                return;
            }

            List<Element> elements = _allInstanceElements
                .Where(e =>
                {
                    Parameter p = e?.GetParameter(SelectedParameter01);
                    if (p == null) return false;
                    return !string.IsNullOrEmpty(p.GetValue(true))
                           && p.GetValue(true).Equals(SelectedValueParameter01);
                }).ToList();

            List<ElementType> elementTypes = _allFamilySymbol
                .Where(e =>
                {
                    Parameter p = e?.GetParameter(SelectedParameter01);
                    if (p == null) return false;
                    return !string.IsNullOrEmpty(p.GetValue(true))
                           && p.GetValue(true).Equals(SelectedValueParameter01);
                }).ToList();


            List<string> list
               = elements
                .Select(e => e?.LookupParameter(SelectedParameter02)?.GetValue(true))
                .Where(v => !string.IsNullOrEmpty(v))
                .ToList();

            //if (list.Count > 0)
            //    list = list.Distinct().ToList();

            List<string> list1 = elementTypes.Select(e =>
                e?.LookupParameter(SelectedParameter02)?.GetValue(true)).ToList();
            //if (list1.Count > 0)
            //    list1 = list1.Distinct().ToList();

            list.AddRange(list1);
            if (list.Count > 0)
                list = list.Distinct().ToList();

            AllValueParameters02 = list;
            AllValueParameters02.Sort();
        }
        public void UpdateAllValueParameters03()
        {
            if (string.IsNullOrEmpty(SelectedParameter01) ||
                string.IsNullOrEmpty(SelectedParameter02) ||
                string.IsNullOrEmpty(SelectedParameter03))
            {
                AllValueParameters03 = new List<string>();
                return;
            }

            List<Element> elements1 = _allInstanceElements
                .Where(e =>
                {
                    Parameter p = e?.GetParameter(SelectedParameter01);
                    if (p == null) return false;
                    return !string.IsNullOrEmpty(p.GetValue(true)) && p.GetValue(true).Equals(SelectedValueParameter01);
                }).ToList();

            List<Element> elements = elements1
                .Where(e =>
                {
                    Parameter p = e?.GetParameter(SelectedParameter02);
                    if (p == null) return false;
                    return !string.IsNullOrEmpty(p.GetValue(true)) && p.GetValue(true).Equals(SelectedValueParameter02);
                }).ToList();

            List<ElementType> elementTypes1 = _allFamilySymbol
                .Where(e =>
                {
                    Parameter p = e?.GetParameter(SelectedParameter01);
                    if (p == null) return false;
                    return !string.IsNullOrEmpty(p.GetValue(true)) && p.GetValue(true).Equals(SelectedValueParameter01);
                }).ToList();

            List<ElementType> elementTypes = elementTypes1
                         .Where(e =>
                         {
                             Parameter p = e?.GetParameter(SelectedParameter02);
                             if (p == null) return false;
                             return !string.IsNullOrEmpty(p.GetValue(true)) && p.GetValue(true).Equals(SelectedValueParameter02);
                         }).ToList();



            List<string> list
               = elements
                .Select(e => e?.LookupParameter(SelectedParameter03)?.GetValue(true))
                /*.Distinct()*/.ToList();

            list.AddRange(elementTypes.Select(e =>
               e?.LookupParameter(SelectedParameter03)?.GetValue(true)));

            if (list.Count > 0)
                list = list.Distinct().ToList();

            AllValueParameters03 = list;
            AllValueParameters03.Sort();
        }
        /// <summary>
        /// Update số lượng elements được chọn để lấy value parameter
        /// </summary>
        public void UpdateScope()
        {
            currentSelection = uidoc.Selection.GetElementIds().ToList();
            //if (expr)
            //{

            //}


            _allInstanceElements =
                KDXQqeontLCGWXe.GetModelElements(doc, IsCurrentView,
                    IsCurrentSelection, currentSelection);

            _allFamilySymbol =
                _allInstanceElements.Select(e => e?.GetElementType())
                    .Distinct(new EAArFxukdmItCtf()).ToList();

            UpdateAllValueParameters01();
            //UpdateAllParameters02();
            UpdateAllValueParameters02();
            //UpdateAllParameters03();
            UpdateAllValueParameters03();

        }

        public void GetElements()
        {
            if (_allInstanceElements.Count == 0||
                string.IsNullOrEmpty(SelectedParameter01)||
                string.IsNullOrEmpty(SelectedValueParameter01)
                ) return;

            List<Element> sameValue = _allInstanceElements.Where(e =>
            {
                Parameter p = e.GetParameter(SelectedParameter01);
                string value = p?.GetValue(true);
                if (string.IsNullOrEmpty(value)) return false;
                return value.Equals(SelectedValueParameter01);
            }).ToList();

            if (!string.IsNullOrEmpty(SelectedParameter02)
                && !string.IsNullOrEmpty(SelectedValueParameter02))
            {
                sameValue = sameValue.Where(e =>
                 {
                     Parameter p = e.GetParameter(SelectedParameter02);
                     string value = p?.GetValue(true);
                     if (string.IsNullOrEmpty(value)) return false;
                     return value.Equals(SelectedValueParameter02);
                 }).ToList();
            }

            if (!string.IsNullOrEmpty(SelectedParameter03)
                && !string.IsNullOrEmpty(SelectedValueParameter03))
            {
                sameValue = sameValue.Where(e =>
                 {
                     Parameter p = e.GetParameter(SelectedParameter03);
                     string value = p?.GetValue(true);
                     if (string.IsNullOrEmpty(value)) return false;
                     return value.Equals(SelectedValueParameter03);
                 }).ToList();
            }

            _sameElements = sameValue.Select(e => e.Id).ToList();
            _sameElements = _sameElements
                .Where(id => id != ElementId.InvalidElementId).ToList();
            if (_sameElements.Any())
            {
                uidoc.Selection.SetElementIds(_sameElements);
                //uidoc.ShowElements(_sameElements);
            }
        }
    }
}
