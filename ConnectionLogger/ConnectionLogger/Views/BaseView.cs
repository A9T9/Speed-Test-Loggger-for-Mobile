using ConnectionLogger.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ConnectionLogger.Views
{
    public abstract class BaseView:ContentPage
    {
        #region ViewModels
        /// <summary>
        /// Set datacontex of this page to specific ViewModel instance
        /// </summary>
        /// <typeparam name="T">ViewModel</typeparam>
        public void SetViewModel<T>() where T : BaseViewModel
        {
            BindingContext = ViewModelProvider.GetViewModel<T>();
        }

        /// <summary>
        /// Returns Viemodel instance assotiated with this page
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetViewModel<T>() where T : BaseViewModel
        {
            if (BindingContext == null)
                SetViewModel<T>();
            return (T)BindingContext;
        }
        #endregion

        #region Navigation methods
        
        #endregion

        
    }
}
