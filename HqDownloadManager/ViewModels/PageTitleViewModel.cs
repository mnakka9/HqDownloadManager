﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HqDownloadManager.ViewModels {
    public class PageTitleViewModel : ViewModelBase {
        private string _pageTitle;

        public string PageTitle {
            get {
                return _pageTitle;
            }
            set {
                _pageTitle = value;
                OnPropertyChanged("PageTitle");
            }
        }
    }
}