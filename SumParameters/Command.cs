using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using SumParameters.Model;
using SumParameters.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Collections.ObjectModel;

namespace SumParameters
{
    [TransactionAttribute(TransactionMode.Manual)]
    internal class Command : IExternalCommand
    {
        private static UIApplication _uiapp;
        private static Autodesk.Revit.ApplicationServices.Application _app;
        private static UIDocument _uidoc;
        private static Document _doc;
        private static MainWindow _view;
        
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            _uiapp = commandData.Application;
            _uidoc = _uiapp.ActiveUIDocument;
            _app = _uiapp.Application;
            _doc = _uidoc.Document;
            if (commandData.Application.ActiveUIDocument.Document.IsFamilyDocument)
            {
                MessageBox.Show("Вы открыли семейство", "Ошибка");
                return Result.Cancelled;
            }
            ObservableCollection<ParamItem> paramSet;
            var predReference = _uidoc.Selection.GetElementIds();
            if (predReference.Count == 0) paramSet = new ObservableCollection<ParamItem>();
            else paramSet = GetParamItems(predReference);

            var viewModel = new ViewModel(paramSet);

            _view = new WPF.MainWindow();

            //что бы немодальное окно было сверху
            System.Diagnostics.Process proc = System.Diagnostics.Process.GetCurrentProcess();
            System.Windows.Interop.WindowInteropHelper helper = new System.Windows.Interop.WindowInteropHelper(_view);
            helper.Owner = proc.MainWindowHandle;

            _view.DataContext = viewModel;
            _view.Show();

            _app.DocumentClosed += new EventHandler<Autodesk.Revit.DB.Events.DocumentClosedEventArgs>(IsOpenedDocuments);
            return Result.Succeeded;
        }

        private void IsOpenedDocuments(object sender, Autodesk.Revit.DB.Events.DocumentClosedEventArgs args)
        {
            if (_app.Documents.Size < 1)
            {
                _view.Close();
            }
        }
        private static ObservableCollection<ParamItem> GetParamItems (ICollection<ElementId> predReference)
        {
            ObservableCollection<ParamItem> paramSet = new ObservableCollection<ParamItem>();
            foreach (var id in predReference)
            {
                //Параметры экземпляра
                Element el = _doc.GetElement(id);
                var ps = el.GetOrderedParameters();
                var psIntDouble = ps.Where(x => x.StorageType == StorageType.Integer || x.StorageType == StorageType.Double);
                foreach (var param in psIntDouble)
                {
                    //доп проверка для bool и workset
                    if (param.Definition.ParameterType.ToString() != "YesNo" &&
                        param.Definition.ParameterType.ToString() != "Invalid")
                    {
                        ParamItem paramItem = new ParamItem(param);
                        paramSet.Add(paramItem);
                    }
                }
                //Параметры типа
                var elType = _doc.GetElement(el.GetTypeId());
                if (elType == null) continue;
                var pst = elType.GetOrderedParameters();
                var pstIntDouble = pst.Where(x => x.StorageType == StorageType.Integer || x.StorageType == StorageType.Double);
                foreach (var param in pstIntDouble)
                {
                    //доп проверка для bool и workset
                    if (param.Definition.ParameterType.ToString() != "YesNo" &&
                        param.Definition.ParameterType.ToString() != "Invalid")
                    {
                        ParamItem paramItem = new ParamItem(param);
                        paramSet.Add(paramItem);

                    }
                }
            }
            
            return GetSortSumParameters(paramSet) ;
        }
        public static ObservableCollection<ParamItem> GetSortSumParameters(ObservableCollection<ParamItem> paramItems)
        {
            ObservableCollection<ParamItem> paramItemsColection = new ObservableCollection<ParamItem>();
            var set = paramItems.OrderBy(x => x.Name).ToList();
            string lastItemName = "";
            for (int i = 0; i < set.Count; i++)
            {
                var itemName = set[i].Name;
                if (lastItemName != itemName)
                {
                    paramItemsColection.Add(set[i]);
                    lastItemName = itemName;
                }
                else set[i - 1].Value += set[i].Value;

            }
            return paramItemsColection ;
        }


        public static ObservableCollection<ParamItem> AddSelectElements()
        {
            try
            {
                List<ParamItem> newsheet = null;
                //var reference = _uidoc.Selection.PickObjects(ObjectType.Element, , "Выберите элементы для подсчета параметров",);
                var predReference = _uidoc.Selection.GetElementIds();
                //List<ElementId> elIdList = reference.ToList().ConvertAll(x => x.ElementId);
                List<ElementId> elIdList = predReference.ToList();
                var addedParamItems = GetParamItems(elIdList);
               
                //elementsId.AddRange(elIdList);
                return GetSortSumParameters(addedParamItems);
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                return null;
            }

        }
    }
}
