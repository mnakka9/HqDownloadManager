﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HqDownloadManager.Core.Models;
using HqDownloadManager.Core.Helpers;
using AngleSharp.Dom;
using System.Threading;
using HqDownloadManager.Core.CustomEventArgs;
using HqDownloadManager.Core.Database;

namespace HqDownloadManager.Core.Sources {
    internal class YesMangasSource : HqSource {
        public YesMangasSource(LibraryContext libraryContext, HtmlSourceHelper htmlHelper, BrowserHelper browserHelper) : base(libraryContext, htmlHelper, browserHelper) {
        }

        public override List<Hq> GetUpdates(string updatePage) {
            lock (Lock1) {
                try {
                    OnProcessingProgress(new ProcessingEventArgs(DateTime.Now, $"Buscando Lançamentos..."));
                    var source = HtmlHelper.GetSourceCodeFromUrl(updatePage);
                    OnProcessingProgress(new ProcessingEventArgs(DateTime.Now, $"Pegando dados da página"));
                    var hqs = new List<Hq>();
                    if (source == null) throw new Exception("Ocorreu um erro ao buscar informaçoes da Hq");
                    var hqsEl = source.QuerySelectorAll("div#lancamentos table.u-full-width tr");
                    OnProcessingProgress(new ProcessingEventArgs(DateTime.Now, $"{hqsEl.Count()} mangas encontrados!"));
                    foreach (var hq in hqsEl) {
                        var title = hq.QuerySelector("td a h4")?.TextContent;
                        OnProcessingProgress(new ProcessingEventArgs(DateTime.Now, $"Pegando dados de {title}"));
                        var img = hq.QuerySelector("td a img")?.GetAttribute("src");
                        var link = hq.QuerySelector("td div a")?.GetAttribute("href");
                        if (!string.IsNullOrEmpty(link)) {
                            var update = new Hq { Link = link, Title = title, CoverSource = img };
                            if (!hqs.Contains(update)) {
                                hqs.Add(update);
                                OnProcessingProgress(new ProcessingEventArgs(DateTime.Now, update, $"{title} Adicionado"));
                            }
                        }
                    }
                    OnProcessingProgress(new ProcessingEventArgs(DateTime.Now, $"Tudo pronto"));
                    return hqs;
                } catch (Exception e) {
                    OnProcessingProgressError(new ProcessingErrorEventArgs(DateTime.Now, updatePage, e));
                    return null;
                }

            }
        }

        public override LibraryPage GetLibrary(String linkPage) {
            lock (Lock2) {
                try {
                    OnProcessingProgress(new ProcessingEventArgs(DateTime.Now, "Processando os Mangas..."));
                    var source = HtmlHelper.GetSourceCodeFromUrl(linkPage);
                    OnProcessingProgress(new ProcessingEventArgs(DateTime.Now, $"Pegando dados da página"));
                    var hqs = new List<Hq>();
                    if (source == null) throw new Exception("Ocorreu um erro ao buscar informaçoes da Hq");
                    var hqsEl = source.QuerySelectorAll("#mangas .two");
                    OnProcessingProgress(new ProcessingEventArgs(DateTime.Now, $"{hqsEl.Count()} mangas encontrados!"));
                    var nextPage = source.QuerySelector("#mangas a.next")?.GetAttribute("href");
                    var finalizadosEl = source.QuerySelectorAll("#mangas ul.nav-tabs li a");
                    var finalizedPage = "";
                    foreach (var el in finalizadosEl) {
                        if (el.TextContent.Contains("Completos")) {
                            finalizedPage = el.GetAttribute("href");
                        }
                    }
                    var lethers = source.QuerySelectorAll("#mangas .letters a");
                    var letherLink = new Dictionary<string, string>();
                    foreach (var lether in lethers) {
                        letherLink[lether.TextContent] = lether.GetAttribute("href");
                    }
                    OnProcessingProgress(new ProcessingEventArgs(DateTime.Now, $"Preparando para retornar mangas!"));
                    foreach (var hq in hqsEl) {
                        var title = hq.QuerySelector("h3")?.TextContent;
                        var img = hq.QuerySelector("a img")?.GetAttribute("src");
                        var link = hq.QuerySelector("h3 a")?.GetAttribute("href");
                        if (!string.IsNullOrEmpty(link)) {
                            var manga = new Hq { Link = link, Title = title, CoverSource = img };
                            if (!hqs.Contains(manga)) {
                                hqs.Add(manga);
                                OnProcessingProgress(new ProcessingEventArgs(DateTime.Now, manga, $"{title} Adicionado"));
                            }
                        }
                    }
                    OnProcessingProgress(new ProcessingEventArgs(DateTime.Now, $"Tudo pronto"));
                    return new LibraryPage {
                        Hqs = hqs, NextPage = nextPage, FinalizedPage = finalizedPage, Letras = letherLink
                    };
                } catch (Exception e) {
                    OnProcessingProgressError(new ProcessingErrorEventArgs(DateTime.Now, linkPage, e));
                    return null;
                }
            }
        }

        public override Hq GetHqInfo(string link) {
            lock (Lock3) {
                try {
                    OnProcessingProgress(new ProcessingEventArgs(DateTime.Now, $"Pegando dados da página"));
                    var source = HtmlHelper.GetSourceCodeFromUrl(link);
                    var hqInfo = new Hq();
                    if (source == null) throw new Exception("Ocorreu um erro ao buscar informaçoes da Hq");
                    var title = source.QuerySelector("h1.title")?.TextContent;
                    var img = source.QuerySelector("#descricao img");
                    var synopsis = source.QuerySelector("#descricao article")?.TextContent;
                    OnProcessingProgress(new ProcessingEventArgs(DateTime.Now, $"Buscando informações de {title}"));
                    hqInfo.Title = title;
                    hqInfo.CoverSource = img?.GetAttribute("src");
                    hqInfo.Synopsis = synopsis.Replace("\n", "").Trim();
                    hqInfo.Link = link; var hqInfos = source.QuerySelectorAll("#descricao div.content ul li");
                    foreach (var info in hqInfos) {
                        if (info.TextContent.Contains("Autor")) {
                            hqInfo.Author = info.TextContent.Replace("Autor:", "").Trim();
                        }
                    }
                    hqInfo.Chapters = GetListChapters(source).Reverse<Chapter>().ToList();

                    OnProcessingProgress(new ProcessingEventArgs(DateTime.Now, $"Tudo pronto!"));
                    return hqInfo;
                } catch (Exception e) {
                    OnProcessingProgressError(new ProcessingErrorEventArgs(DateTime.Now, link, e));
                    return null;
                }
            }
        }

        public override Chapter GetChapterInfo(string link) {
            lock (Lock4) {
                try {
                    OnProcessingProgress(new ProcessingEventArgs(DateTime.Now, $"Pegando dados da página"));
                    IDocument leitor = HtmlHelper.GetSourceCodeFromUrl(link);
                    var chapter = new Chapter();
                    if (leitor == null) throw new Exception("Ocorreu um erro ao buscar informaçoes do capitulo");
                    var chapterTitle = leitor.QuerySelector("header.container h1.title")?.TextContent;
                    OnProcessingProgress(new ProcessingEventArgs(DateTime.Now, $"Buscando informações de {chapterTitle}"));
                    chapter.Title = chapterTitle;
                    chapter.Link = link;
                    var pages = GetPageList(leitor);
                    if (pages.Count <= 2) {
                        UsingIe = true;
                        var driver = BrowserHelper.GetDriver(link);
                        var pageSource = driver.PageSource;
                        OnProcessingProgress(new ProcessingEventArgs(DateTime.Now, $"Pegando dados da página"));
                        leitor = HtmlHelper.GetSourceCodeFromHtml(pageSource);
                        pages = GetPageList(leitor);
                        driver.Quit();
                        UsingIe = false;
                    }
                    chapter.Pages = pages;
                    OnProcessingProgress(new ProcessingEventArgs(DateTime.Now, $"Tudo pronto"));
                    return chapter;
                } catch (Exception e) {
                    OnProcessingProgressError(new ProcessingErrorEventArgs(DateTime.Now, link, e));
                    return null;
                }
            }
        }

        public List<Chapter> GetListChapters(IDocument hqSource) {
            lock (Lock5) {
                OnProcessingProgress(new ProcessingEventArgs(DateTime.Now, $"Buscando capitulos"));
                var chapterList = new List<Chapter>();
                var chapters = hqSource.QuerySelectorAll(".eight a.button");
                if (chapters == null) return chapterList;
                foreach (var chapter in chapters) {
                    var chapterLink = chapter.GetAttribute("href");
                    var chapterTitle = $"Capitulo - {chapter.TextContent}";
                    OnProcessingProgress(new ProcessingEventArgs(DateTime.Now, $"Adicionando {chapterTitle}"));
                    chapterList.Add(new Chapter { Title = chapterTitle, Link = chapterLink });
                }
                return chapterList;
            }
        }

        public List<Models.Page> GetPageList(IDocument chapterSoure) {
            lock (Lock6) {
                OnProcessingProgress(new ProcessingEventArgs(DateTime.Now, $"Buscando páginas"));
                var pageList = new List<Models.Page>();
                var pages = GetPageListFromScript(chapterSoure);
                var mangaPages = pages.QuerySelectorAll("img");
                if (mangaPages == null) return pageList;
                var num = 1;
                foreach (var img in mangaPages) {
                    OnProcessingProgress(new ProcessingEventArgs(DateTime.Now, $"Adicionando página {num}"));
                    pageList.Add(new Page { Number = num, Source = img.GetAttribute("src") });
                    num++;
                }
                return pageList;
            }
        }

        private IDocument GetPageListFromScript(IDocument source) {
            lock (Lock7) {
                var element = "";
                var imageElement = source.QuerySelector(".content-slideshow a");
                if (imageElement == null) return HtmlHelper.GetSourceCodeFromHtml(element);
                var openTag = "<div class='pages-content'>";
                var closeTag = "</div>";
                var content = imageElement.ParentElement?.InnerHtml;
                if (UsingIe) {
                    element = $"{openTag}{content}{closeTag}";
                } else {
                    var html = source.Body.InnerHtml;
                    html = html.Substring(html.IndexOf("var images = ["));
                    html = html.Substring(html.IndexOf("["), html.IndexOf("];"));
                    var otherPages = html.Split(']')[0];
                    otherPages = otherPages.Split('[')[1];
                    otherPages = otherPages.Replace("\"", "");
                    element = $"{openTag}{content}{otherPages}{closeTag}";
                }

                element = element.Replace("\"", "'");
                element = element.Replace(".webp", "");
                element = element.Replace("/images", "/mangas_files");

                return HtmlHelper.GetSourceCodeFromHtml(element);
            }
        }
    }
}
