﻿using HqDownloadManager.Core;
using HqDownloadManager.Core.Models;
using HqDownloadManager.FollowUpdate.CustomEventArgs;
using HqDownloadManager.FollowUpdate.Databases;
using HqDownloadManager.FollowUpdate.Models;
using Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HqDownloadManager.FollowUpdate.Helpers
{
    internal class UpdateHelper {
        private FollowContext _followContext;
        private SourceManager _sourceManager;
        private FollowHelper _followHelper;
        private TaskTimer _taskTimer;

        public event UpdateEventHandler UpdateStart;
        public event UpdateEventHandler UpdateEnd;

        public UpdateHelper(FollowContext followContext, SourceManager sourceManager, FollowHelper followHelper, TaskTimer taskTimer) {
            _followContext = followContext;
            _sourceManager = sourceManager;
            _followHelper = followHelper;
            _taskTimer = taskTimer;
        }

        public List<Chapter> GetUpdates(string hqLink) {
            var update = new List<Chapter>();
            Hq hq = null;
            var startTime = DateTime.Now;
            if (!(_followContext.FollowedHq.FindOne(hqLink) is FollowedHq followedHq)) return update;
            hq = followedHq.Hq.ToObject<Hq>();
            UpdateStart(this, new UpdateEventArgs(hq, startTime));
            if (!(_sourceManager.GetInfo<Hq>(hqLink) is Hq hqInfo)) return update;
            update.AddRange(hqInfo.Chapters.Where(chap => !hq.Chapters.Contains(chap)));
            
            return update;
        }
    }
}
