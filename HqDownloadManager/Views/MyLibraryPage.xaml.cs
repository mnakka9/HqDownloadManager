﻿using HqDownloadManager.Controller;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// O modelo de item de Página em Branco está documentado em https://go.microsoft.com/fwlink/?LinkId=234238

namespace HqDownloadManager.Views {
    /// <summary>
    /// Uma página vazia que pode ser usada isoladamente ou navegada dentro de um Quadro.
    /// </summary>
    public sealed partial class MyLibraryPage : Page {
        private MyLibraryPageController _controoller;

        public MyLibraryPage(MyLibraryPageController controoller) {
            _controoller = controoller;
            this.InitializeComponent();
            this.Loaded += _controoller.OnLoaded;
        }

        private void HqLibraryGrid_ItemClick(object sender, ItemClickEventArgs e) => _controoller.OpenDetails<DownloadDetailsPage>();
    }
}