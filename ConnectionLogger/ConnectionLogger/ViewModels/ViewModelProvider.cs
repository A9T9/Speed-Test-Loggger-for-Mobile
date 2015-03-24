using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ConnectionLogger.ViewModels
{
    public static class ViewModelProvider
    {
        static List<BaseViewModel> _viewModels = new List<BaseViewModel>();

        /// <summary>
        /// Searches for specified viewmodel
        /// retuns new instance of this type if nothing found
        /// </summary>
        /// <typeparam name="T">ViewModel type</typeparam>
        /// <returns></returns>
        public static T GetViewModel<T>() where T:BaseViewModel
        {
            T vm = (T)_viewModels.Where(f=>f is T).FirstOrDefault();
            if (vm == null)
            {
                vm=(T)Activator.CreateInstance(typeof(T));
                _viewModels.Add(vm);
            }
            return vm;
        }
        
        /// <summary>
        /// Force to create new instanse of  ViewModel
        /// </summary>
        /// <typeparam name="T">ViewModel type</typeparam>
        /// <param name="removeOld">if true (default) removes old instace of this type</param>
        /// <returns></returns>
        public static T GetNewViewModel<T>(bool removeOld=true) where T : BaseViewModel
        {
            T vm;
            if (removeOld)
            {
                vm = (T)_viewModels.Where(f => f is T).FirstOrDefault();

                if (vm != null)
                {
                    _viewModels.Remove(vm);
                    vm = null;
                }
            }
            vm = (T)Activator.CreateInstance(typeof(T));
            _viewModels.Add(vm);
            return vm;
        }

        /// <summary>
        /// Checks if at least one instance of this viewmodel type presents
        /// </summary>
        /// <typeparam name="T">ViewModelType</typeparam>
        /// <returns></returns>
        public static bool HasViewModel<T>() where T : BaseViewModel
        {
            T vm = (T)_viewModels.Where(f => f is T).FirstOrDefault();
            return (vm != null);
        }

        public static void AddViewModel<T>(T viewModel) where T:BaseViewModel
        {
            _viewModels.Add(viewModel);
        }
    }
}
