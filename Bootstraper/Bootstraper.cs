using SliderPuzzleSolver;
using SliderPuzzleSolver.Interfaces;
using System;
using System.Windows;
using Unity;
using UserInterface;
using UserInterface.Pages.SliderPage;

namespace Bootstraper
{
    public class Bootstraper

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

        public Type ResolveView<Type> (string viewName) where Type: FrameworkElement
        {
            return _unityContainer.Resolve<Type>(viewName);
        }

        public void ConfigureContainer()
        {
            RegisterTypes();
            RegisterViews();
        }


        private void RegisterTypes()
        {
            _unityContainer.RegisterType<IPuzzleSolver, PuzzleSolver>();
            _unityContainer.RegisterType<IBoard, Board>();
        }

        private void RegisterViews()
        {
            _unityContainer.RegisterView<SliderPage, SliderPageViewModel>();
            _unityContainer.RegisterView<MainWindow, MainWindowViewModel>();
        }

    }
}
