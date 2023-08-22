using SumParameters.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace SumParameters.WPF
{
    public class ViewModel : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged realise
        public event PropertyChangedEventHandler PropertyChanged;
        public int GetPropertyChangedSubscribledLenght()
        {
            return PropertyChanged?.GetInvocationList()?.Length ?? 0;
        }
        protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value)) return false;

            storage = value;
            RaisePropertyChanged(propertyName);

            return true;
        }
        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            PropertyChanged?.Invoke(this, args);
        }
        #endregion

        private ObservableCollection<ParamItem> items;
        public ObservableCollection<ParamItem> Items
        {
            get { return items; }
            set => SetProperty(ref items, value);
        }

        private int indexSelectionMethod;
        public int IndexSelectionMethod
        {
            get { return indexSelectionMethod; }
            set => SetProperty(ref indexSelectionMethod, value);
        }


        public ViewModel(ObservableCollection<ParamItem> paramItems)
        {
            //прокидка команд
            Select = new RelayCommand(o => SelectCommand(o));

            Items = paramItems;
        }

        public ICommand Select { get; }

        private void SelectCommand(object obj)
        {
            if(IndexSelectionMethod == 0) Items = Command.AddSelectElements();
        }

    }
}
