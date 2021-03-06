﻿using HqDownloadManager.Core.Models;
using HqDownloadManager.Core.Sources;
using System;
using System.Collections.Generic;
using System.Text;

namespace HqDownloadManager.Core.Managers {
    public class HipercoolSourceManager : HqSourceManager<HipercoolSource> {
        public HipercoolSourceManager(CacheManager cacheManager, HipercoolSource hqSource) : base(cacheManager, hqSource) {
            UpdatePage = "https://hiper.cool/";
            LibraryPage = "https://hiper.cool/";
        }
    }
}