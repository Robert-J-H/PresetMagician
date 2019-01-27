﻿using System.ComponentModel;
using Catel;
using Orchestra;
using PresetMagician.Services.Interfaces;

namespace PresetMagician.Views
{
    /// <summary>
    /// Interaction logic for RibbonView.xaml
    /// </summary>
    public partial class RibbonView
    {
        private IRuntimeConfigurationService _runtimeConfigurationService;


        public RibbonView(IRuntimeConfigurationService runtimeConfigurationService)
        {
            Argument.IsNotNull(() => runtimeConfigurationService);

            InitializeComponent();
            ribbon.AddAboutButton();


            _runtimeConfigurationService = runtimeConfigurationService;

            _runtimeConfigurationService.ApplicationState.PropertyChanged += ApplicationStateOnPropertyChanged;
        }

        private void ApplicationStateOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedRibbonTabIndex")
            {
                ribbon.SelectedTabIndex = _runtimeConfigurationService.ApplicationState.SelectedRibbonTabIndex;
            }
        }
    }
}