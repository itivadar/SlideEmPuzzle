using System.Windows;
using Unity;

namespace UserInterface.BootstraperSpace
{
    /// <summary>
    /// Class containing the extension methods to register views. 
    /// </summary>
    public static class ContainerExtensions
    {
        
        /// <summary>
        /// Register a View and a ViewModel into the container. Associate the ViewModel to the View.
        /// </summary>
        /// <typeparam name="TViewType">Type of the View</typeparam>
        /// <typeparam name="TViewModelType">Type of the ViewModel</typeparam>
        public static void RegisterView<TViewType, TViewModelType>(this IUnityContainer unityContainer) where TViewType : FrameworkElement
        {
            unityContainer.RegisterSingleton<TViewType, TViewType>();
            unityContainer.RegisterSingleton<TViewModelType, TViewModelType>();

            var view = unityContainer.Resolve<TViewType>();
            var viewModel = unityContainer.Resolve<TViewModelType>();
            view.DataContext = viewModel;
        }

        /// <summary>
        /// Register a <see cref="Page"/> object with a name.
        /// Associate the page with the given ViewModel.
        /// </summary>
        /// <typeparam name="TViewType">Type of the View</typeparam>
        /// <typeparam name="TViewModelType">Type of the ViewModel</typeparam>
        /// <param name="name">The name of the page used for resolving.</param>
        public static void RegisterPage<TViewType, TViewModelType>(this IUnityContainer unityContainer, string name) where TViewType : FrameworkElement
        {
            unityContainer.RegisterSingleton<object, TViewType>(name);
            unityContainer.RegisterSingleton<TViewModelType, TViewModelType>();

            var view = unityContainer.Resolve<object>(name) as TViewType;
            var viewModel = unityContainer.Resolve<TViewModelType>();
            view.DataContext = viewModel;
        }


        /// <summary>
        /// Register a <see cref="Page"/> with a name.
        /// </summary>
        /// <typeparam name="TViewType">Type of the View</typeparam>
        /// <param name="name">The name of the page used for resolving.</param>
        public static void RegisterNoViewModelPage<TViewType>(this IUnityContainer unityContainer, string name) where TViewType : FrameworkElement
        {
            unityContainer.RegisterSingleton<object, TViewType>(name);
        }

    }
}
