using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace UserInterface.Pages.SliderPage
{
    class SliderPageViewModel : BindableBase
    {
        private byte[] _sliderState;

        public SliderPageViewModel()
        {
            RandomizeCommand = new DelegateCommand(OnRandomize);
        }
        public ICommand RandomizeCommand { get; set; }

        public byte[] SliderState
        {
            get => _sliderState;
            set 
            {
                _sliderState = value;
                RaisePropertyChanged(nameof(SliderState));
            }
        }

        private void OnRandomize()
        {
            Random randGen = new Random();

            int n = 16;
            var tempi = new byte[16] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
            while (n > 1)
            {
                byte k = (byte)randGen.Next(n--);
                byte temp = tempi[n];
                tempi[n] = tempi[k];
                tempi[k] = temp;
            }
            SliderState = tempi;
        }

    }
}

