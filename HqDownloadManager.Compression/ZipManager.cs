﻿using HqDownloadManager.Compression.CustomEventArgs;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace HqDownloadManager.Compression
{
    public class ZipManager {

        public event CompressionEventHandler CompressionStart;
        public event CompressionEventHandler CompressionEnd;
        public event CompressionErrorEventHandler CompressionError;

        public void Zip(string path, bool eraseOriginalPath = false) {
            var directory = new DirectoryInfo(path);
            Zip(directory, eraseOriginalPath);
        }

        public void ZipAllInDirectory(string mainPath, bool eraseOriginalPath = false) {
            var root = new DirectoryInfo(mainPath);
            DirectoryInfo[] paths = root.GetDirectories("*", SearchOption.TopDirectoryOnly);
            foreach (var path in paths) {
                Zip(path, eraseOriginalPath);
            }
        }

        public void Zip(DirectoryInfo path, bool eraseOriginalPath = false) {
            var start = DateTime.Now;
            CompressionStart(this, new CompressionEventArgs(path, start));
            TimeSpan totalTime;
            try {
                ZipFile.CreateFromDirectory(path.FullName, $"{path.FullName}.zip", CompressionLevel.Optimal, false);

                if (eraseOriginalPath) {
                    Directory.Delete(path.FullName, true);
                }

                CompressionEnd(this, new CompressionEventArgs(path, start, DateTime.Now, totalTime));
            } catch (Exception e) {
                CompressionError(this, new CompressionErrorEventArgs(path, e, DateTime.Now));
            }
        }
    }
}
