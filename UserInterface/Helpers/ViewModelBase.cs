using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Text;

namespace UserInterface.Helpers
{
    
    /// <summary>
    /// The based class for all the view models
    /// </summary>
    public class ViewModelBase : BindableBase
    {
      /// <summary>
      /// Triggered on displaying the page whom the ViewModel is associated.
      /// </summary>
      public virtual void OnDisplayed()
      {
      }
    }
}
