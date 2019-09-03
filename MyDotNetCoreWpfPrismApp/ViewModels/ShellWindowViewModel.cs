﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using MahApps.Metro.Controls;
using MyDotNetCoreWpfApp.Core.Helpers;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;

namespace MyDotNetCoreWpfPrismApp.ViewModels
{
    public class ShellWindowViewModel : BindableBase
    {
        private IRegionManager _regionManager;
        private HamburgerMenuItem _selectedMenuItem;
        private IRegionNavigationService _navigationService;

        public HamburgerMenuItem SelectedMenuItem
        {
            get { return _selectedMenuItem; }
            set { SetProperty(ref _selectedMenuItem, value); }
        }

        public ObservableCollection<HamburgerMenuItem> MenuItems { get; } = new ObservableCollection<HamburgerMenuItem>()
        {
            new HamburgerMenuGlyphItem() { Label = "Main", Glyph = "\uE8A5", Tag = "Main" },
            new HamburgerMenuGlyphItem() { Label = "Secondary", Glyph = "\uE8A5", Tag = "Secondary" }
        };

        public DelegateCommand LoadedCommand { get; private set; }

        public DelegateCommand GoBackCommand { get; private set; }

        public DelegateCommand MenuItemInvokedCommand { get; private set; }

        public ShellWindowViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;
            LoadedCommand = new DelegateCommand(OnLoaded);
            GoBackCommand = new DelegateCommand(OnGoBack, CanGoBack);
            MenuItemInvokedCommand = new DelegateCommand(OnMenuItemInvoked);
        }

        private void OnLoaded()
        {
            SelectedMenuItem = MenuItems.First();
            OnMenuItemInvoked();
            _navigationService = _regionManager.Regions[RegionNames.MainRegion].NavigationService;
            _navigationService.Navigated += OnNavigated;
        }

        private bool CanGoBack()
            => _navigationService != null && _navigationService.Journal.CanGoBack;

        private void OnGoBack()
            => _navigationService.Journal.GoBack();

        private void OnMenuItemInvoked()
        {
            _regionManager.RequestNavigate(RegionNames.MainRegion, SelectedMenuItem.Tag.ToString());
        }

        private void OnNavigated(object sender, RegionNavigationEventArgs e)
        {
            SelectedMenuItem = MenuItems.First(i => e.Uri.ToString() == i.Tag.ToString());
            GoBackCommand.RaiseCanExecuteChanged();
        }
    }
}