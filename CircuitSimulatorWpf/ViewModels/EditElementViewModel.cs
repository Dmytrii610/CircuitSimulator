using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace CircuitSimulatorWpf.ViewModels
{
    public partial class EditElementViewModel : ObservableObject
    {
        private readonly CanvasElementViewModel canvasElement;

        public string IconPath => canvasElement.IconPath;
        public string DisplayName => canvasElement.DisplayName;
        public string ParameterLabel => canvasElement.ParameterLabel;

        [ObservableProperty]
        private double editedValue;
        public EditElementViewModel(CanvasElementViewModel element)
        {
            this.canvasElement = element;
            editedValue = element.ParameterValue;
        }
        public void Apply() 
        {
            canvasElement.ParameterValue = editedValue;
        }
    }
}
