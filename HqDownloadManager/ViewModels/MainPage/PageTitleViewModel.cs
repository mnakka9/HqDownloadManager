﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HqDownloadManager.ViewModels.MainPage {
    public class PageTitleViewModel : ViewModelBase {
        private string _pageTitle;
        private int _compactModeMinWidth;

        public int CompactModeMinWidth {
            get => _compactModeMinWidth;
            set {
                _compactModeMinWidth = value;
                OnPropertyChanged("CompactModeMinWidth");
            }
        }


        public string Title {
            get => _pageTitle;
            set {
                _pageTitle = value;
                OnPropertyChanged("Title");
            }
        }
    }
}