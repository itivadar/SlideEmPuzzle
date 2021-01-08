using Prism.Events;
using Prism.Mvvm;
using UserInterface.BootstraperSpace;

namespace UserInterface.Helpers
{
    /// <summary>
    /// The based class for all the view models
    /// </summary>
    public class ViewModelBase : BindableBase
    {
        #region Protected Fields

        protected readonly IEventAggregator EventAggregator;
        protected readonly INavigationService NavigationService;

        #endregion Protected Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the ViewModelBase
        /// </summary>
        /// <param name="eventAggregator">The system for comunication between pages.</param>
        /// <param name="navigationService">The navigation service.</param>
        public ViewModelBase(IEventAggregator eventAggregator, INavigationService navigationService)
        {
            EventAggregator = eventAggregator;
            NavigationService = navigationService;
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// Triggered on displaying the page whom the ViewModel is associated.
        /// </summary>
        public virtual void OnDisplayed()
        {
        }

        #endregion Public Methods
    }
}