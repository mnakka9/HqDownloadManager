﻿using DependencyInjectionResolver;
using HqDownloadManager.Core.Managers;
using HqDownloadManager.Core.Sources;
using Reflection.Optimization.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HqDownloadManager.Core.Helpers {
    public class SiteHelper {
        private DependencyInjection _dependencyInjection;
        private Dictionary<String, Type> SupportedSites;
        private Object lockThis = new Object();
        private Object lockThis2 = new Object();
        private Object lockThis3 = new Object();
        private Object lockThis4 = new Object();

        public SiteHelper(DependencyInjection dependencyInjection) {
            _dependencyInjection = dependencyInjection;
            SupportedSites = new Dictionary<string, Type> {
                ["ymangas"] = typeof(YesMangasSourceManager),
                ["yesmangas"] = typeof(YesMangasSourceManager),
                ["mangashost"] = typeof(MangaHostSourceManager),
                ["mangahost"] = typeof(MangaHostSourceManager),
                ["mangahosts"] = typeof(MangaHostSourceManager),
                ["unionmangas"] = typeof(UnionMangasSourceManager),
                ["mangas"] = typeof(MangasProjectSourceManager),
                ["mangastream"] = typeof(MangasProjectSourceManager),
                ["leitor"] = typeof(MangasProjectSourceManager),
                ["mangalivre"] = typeof(MangaLivreSourceManager),
                ["hiper"] = typeof(HipercoolSourceManager)
            };
        }

        public Type GetHqSourceTypeFromUrl(String link) {
            lock (lockThis) {
                Uri url = new Uri(link);
                var host = url.Host.Split('.')[0];
                if (SupportedSites.ContainsKey(host)) {
                    return SupportedSites[host];
                }
                throw new Exception("Não é possivel fazer download deste site!");
            }
        }

        public IHqSourceManager GetHqSourceFromUrl(String link) {
            lock (lockThis) {
                return _dependencyInjection
                    .BindingTypes(typeof(IHqSourceManager), GetHqSourceTypeFromUrl(link))
                    .Resolve<IHqSourceManager>();
            }
        }

        public bool IsHqPage(String link) {
            lock (lockThis2) {
                Uri url = new Uri(link);
                var host = url.Host.Split('.')[0];
                if (SupportedSites.ContainsKey(host)) {
                    if ((host == "mangas" || host == "mangastream")) {
                        if (url.Segments.Count() == 4 && (url.Segments[1].ToLower() == "manga/" || url.Segments[1].ToLower() == "titulos/")) {
                            return true;
                        }
                    } else {
                        if (url.Segments.Count() == 3 && (url.Segments[1].ToLower() == "manga/" || url.Segments[1].ToLower() == "mangas/" || url.Segments[1].ToLower() == "titulos/")) {
                            return true;
                        }
                    }

                    return false;
                }
                throw new Exception("Este site ainda não é suportado!");
            }
        }

        public bool IsChapterReader(String link) {
            lock (lockThis3) {
                Uri url = new Uri(link);
                var host = url.Host.Split('.')[0];
                if (SupportedSites.ContainsKey(host)) {
                    var count = url.Segments.Count();
                    var page = url.Segments[1];

                    if (host == "mangas" || host == "mangastream") {
                        if (url.Segments.Count() > 4 && (url.Segments[1].ToLower() == "manga/" || url.Segments[1].ToLower() == "titulos/" || url.Segments[1].ToLower() == "leitor/" || url.Segments[1].ToLower() == "r/")) {
                            return true;
                        }
                    } else {
                        if (host == "mangashost") {
                            if (url.Segments.Count() > 3 && url.Segments[1].ToLower() == "manga/") {
                                return true;
                            }
                        }else {
                            if (url.Segments.Count() >= 3 && (url.Segments[1].ToLower() == "manga/" || url.Segments[1].ToLower() == "titulos/" || url.Segments[1].ToLower() == "leitor/" || url.Segments[1].ToLower() == "r/" || url.Segments[1].ToLower() == "g/")) {
                                return true;
                            }
                        }
                    }

                    return false;
                }
                throw new Exception("Este site ainda não é suportado!");
            }
        }

        public bool IsSupported(string link) {
            lock (lockThis4) {
                Uri url = new Uri(link);
                var host = url.Host.Split('.')[0];
                return SupportedSites.ContainsKey(host);
            }
        }
    }
}
