using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;
using Unity;
using Unity.Lifetime;

namespace UserInterface.BootstraperSpace
{
    public static class ContainerExtensions
    {

        public static void RegisterView<TViewType, TViewModelType>(this IUnityContainer unityContainer) where TViewType : FrameworkElement
        {
            unityContainer.RegisterType<TViewType, TViewType>(new ContainerControlledLifetimeManager());

            var view = unityContainer.Resolve<TViewType>();
            var viewModel = unityContainer.Resolve<TViewModelType>();
            view.DataContext = viewModel;
        }

        public static void RegisterPage<TViewType, TViewModelType>(this IUnityContainer unityContainer, string name) where TViewType : FrameworkElement
        {
            unityContainer.RegisterType<object, TViewType>(name, new ContainerControlledLifetimeManager());

            var view = unityContainer.Resolve<object>(name) as TViewType;
            var viewModel = unityContainer.Resolve<TViewModelType>();
            view.DataContext = viewModel;
        }

        public static void RegisterNoViewModelPage<TViewType>(this IUnityContainer unityContainer, string name) where TViewType : FrameworkElement
        {
            unityContainer.RegisterType<object, TViewType>(name, new ContainerControlledLifetimeManager());
        }

    }
}
