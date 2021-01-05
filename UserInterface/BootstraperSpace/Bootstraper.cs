using Prism.Events;
using SliderPuzzleGenerator;
using SliderPuzzleSolver;
using SliderPuzzleSolver.Interfaces;
using System;
using System.Windows;
using System.Windows.Controls;
using Unity;
using UserInterface;
using UserInterface.Pages.About;
using UserInterface.Pages.MainMenu;
using UserInterface.Pages.PuzzleSelectorPage;
using UserInterface.Pages.SliderPage;

namespace UserInterface.BootstraperSpace
{
    public class Bootstraper : INavigationService
    {
        private readonly IUnityContainer _unityContainer;
        private  MainWindowViewModel _mainViewModel;
        public Bootstraper()
        {
            _unityContainer = new UnityContainer();
            
        }

        /// <summary>
        /// Registers types and views to the container.
        /// </summary>
        public void ConfigureContainer()
        {
            RegisterTypes();
            RegisterViews();
            _mainViewModel = _unityContainer.Resolve<MainWindowViewModel>();
        }

        public void SetMainPage(string page)
        {
            _mainViewModel.MainFrame = GetPage(page);
        }

       /// <summary>
       /// Gets a <see cref="Page"/> object by name. 
       /// </summary>
       /// <param name="pageName">The page name</param>
       /// <returns>a page</returns>
        public Page GetPage(string pageName)
        {
            return _unityContainer.Resolve<object>(pageName) as Page;
        }

        /// <summary>
        /// Resolve a view from the container.
        /// </summary>
        public TViewType GetView<TViewType>() where TViewType : FrameworkElement
        {
            return _unityContainer.Resolve<TViewType>();
        }


        /// <summary>
        /// Register types into the container.
        /// </summary>
        private void RegisterTypes()
        {
            _unityContainer.RegisterSingleton<IEventAggregator, EventAggregator>();
            _unityContainer.RegisterType<IPuzzleSolver, PuzzleSolver>();
            _unityContainer.RegisterType<IPuzzleGenerator, PuzzleGenerator>();
            _unityContainer.RegisterInstance<INavigationService>(this);

        }

        /// <summary>
        /// Register the views and associate it with the view model
        /// </summary>
        private void RegisterViews()
        {
            _unityContainer.RegisterWindow<MainWindow, MainWindowViewModel>();
            _unityContainer.RegisterPage<SliderPage, SliderPageViewModel>(AppPages.SliderPage);
            _unityContainer.RegisterPage<MainMenuPage, MainMenuViewModel>(AppPages.MainMenuPage);
            _unityContainer.RegisterPage<PuzzleSelectorPage, PuzzleSelectorViewModel>(AppPages.PuzzleSelectorPage);

            _unityContainer.RegisterNoViewModelPage<AboutPage>(AppPages.AboutPage);
            
        }
    }
}
