﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HqDownloadManager.Models;

namespace HqDownloadManager.ViewModels.ComboBox {
    public class ComboBoxViewModelBase : ViewModelBase {
        private ObservableCollection<SourceLink> _sourceLinks;
        private SourceLink _selectedSourceLink;

        public SourceLink SelectedSourceLink {
            get => _selectedSourceLink;
            set {
                _selectedSourceLink = value;
                OnPropertyChanged("SelectedSourceLink");
            }
        }


        public ObservableCollection<SourceLink> SourceLinks {
            get => _sourceLinks;
            set {
                _sourceLinks = value;
                OnPropertyChanged("SourceLinks");
            }
        }

    }
}
