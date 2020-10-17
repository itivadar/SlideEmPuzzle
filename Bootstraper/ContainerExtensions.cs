using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using Unity;

namespace Bootstraper
{
    public static class ContainerExtensions
    {

        public static void RegisterView<TViewType, TViewModelType>(this IUnityContainer unityContainer, string viewName) where TViewType : FrameworkElement
        {
            unityContainer.RegisterType<object, TViewModelType>();
            unityContainer.RegisterType<object, TViewType>(viewName);

            var view = unityContainer.Resolve<TViewType>(viewName);
            var viewModel = unityContainer.Resolve<TViewModelType>();
            view.DataContext = viewModel;
        }

    }
}
