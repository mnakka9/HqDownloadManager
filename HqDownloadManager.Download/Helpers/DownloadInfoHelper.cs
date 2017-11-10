﻿using HqDownloadManager.Core.Database;
using HqDownloadManager.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HqDownloadManager.Download.Databases;
using HqDownloadManager.Download.Models;
using HqDownloadManager.Utils;
using Newtonsoft.Json;

namespace HqDownloadManager.Download.Helpers {
    internal class DownloadInfoHelper {
        private readonly DownloadContext _downloadContext;
        private readonly object _lock = new object();
        private readonly object _lock2 = new object();


        public DownloadInfoHelper(DownloadContext downloadContext) {
            _downloadContext = downloadContext;
        }

        public void SaveHqDownloadInfo(Hq hq, string path, DateTime time) {
            lock (_lock) {
                var dInfo = new HqDownloadInfo {
                    Link = hq.Link,
                    SavedIn = path, Time = time,
                    HqDownloaded = hq.ToBytes()
                };
                if (_downloadContext.HqDownloadInfo.Find().Where(x => x.SavedIn == path).Execute().FirstOrDefault() != null) {
                    _downloadContext.HqDownloadInfo.Update(dInfo);
                } else {
                    _downloadContext.HqDownloadInfo.Save(dInfo);
                }
            }
        }

        public List<HqDownloadInfo> GetHqsDownloadInfo() {
            lock (_lock2) {
                return _downloadContext.HqDownloadInfo.FindAll();
            }
        }
    }
}
