using SliderPuzzleSolver;
using SliderPuzzleSolver.Interfaces;
using System;
using System.Windows;
using System.Windows.Controls;
using Unity;
using UserInterface;
using UserInterface.Pages.About;
using UserInterface.Pages.SliderPage;

namespace UserInterface.BootstraperSpace
{
    public class Bootstraper : INavigationService
    {
        private IUnityContainer _unityContainer;

        public Bootstraper()
        {
            _unityContainer = new UnityContainer();
        }
        public Type Resolve<Type>()
        {
            return _unityContainer.Resolve<Type>();
        }

        public Type ResolveView<Type>() where Type : FrameworkElement
        {
            return _unityContainer.Resolve<Type>();
        }

        public void ConfigureContainer()
        {
            RegisterTypes();
            RegisterViews();
        }

        public Page GetPage(string pageName)
        {
            return _unityContainer.Resolve<object>(pageName) as Page;
        }

        public TViewType GetView<TViewType>() where TViewType : FrameworkElement
        {
            return _unityContainer.Resolve<TViewType>();
        }

        private void RegisterTypes()
        {
            _unityContainer.RegisterType<IPuzzleSolver, PuzzleSolver>();
            _unityContainer.RegisterType<IBoard, Board>();
            _unityContainer.RegisterInstance<INavigationService>(this);
        }

        private void RegisterViews()
        {
            _unityContainer.RegisterView<MainWindow, MainWindowViewModel>();
            _unityContainer.RegisterPage<SliderPage, SliderPageViewModel>(AppPages.SliderPage);
            _unityContainer.RegisterNoViewModelPage<AboutPage>(AppPages.AboutPage);
        }

       
    }
}
