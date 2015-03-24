using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Xamarin.Forms;


namespace ConnectionLogger.ViewModels
{
    public abstract class ParentViewModel :BaseViewModel
    {
        #region Constructor
        public ParentViewModel()
        {
            Children = new ObservableCollection<BaseViewModel>();
        }
        #endregion

        #region Fields
        ObservableCollection<BaseViewModel> _children;

        BaseViewModel _selectedChild;
        #endregion

        #region Properties

        public ObservableCollection<BaseViewModel> Children
        {
            get 
            {
                return _children; 
            }
            set
            {
                if (_children != value)
                {
                    _children = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public BaseViewModel SelectedChild
        {
            get
            {
                return _selectedChild;
            }
            set
            {
                if (_selectedChild == value)
                {
                    _selectedChild = value;
                    NotifyPropertyChanged();
                }
            }
        }

        #endregion

        #region Methods
        public void AddChild(BaseViewModel child, bool insert=true)
        {
            if (!_children.Contains(child))
            {
                if (insert)
                    _children.Insert(0,child);
                else
                    _children.Add( child);
                child.Parent = this;
                NotifyPropertyChanged("Children");
            }
        }

        public void AddChildByID(BaseViewModel child)
        {
            var oldItem = _children.FirstOrDefault(f => f.ID == child.ID);

            if (!string.IsNullOrEmpty(child.ID) && oldItem==null)
            {
                _children.Add(child);
                child.Parent = this;
                NotifyPropertyChanged("Children");
            }
        }

        public void AddChildByTitle(BaseViewModel child)
        {
            var oldItem = _children.FirstOrDefault(f => f.Title == child.Title);

            if (!string.IsNullOrEmpty(child.Title) && oldItem == null)
            {
                _children.Add(child);
                child.Parent = this;
                NotifyPropertyChanged("Children");
            }
        }

        public void RemoveChild(BaseViewModel child)
        {
            if (_children.Contains(child))
            {
                child.Parent = null;
                _children.Remove(child);
                child = null; 
                NotifyPropertyChanged("Children");
            }
        }

        public virtual void ClearChildren()
        {
            var temp = new List<BaseViewModel>();
            temp.AddRange(_children);
            foreach (var vm in temp)
                RemoveChild(vm);
            temp.Clear();
            temp = null;
        }
        #endregion
    }
}
