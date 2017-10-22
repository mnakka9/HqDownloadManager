﻿using HqDownloadManager.Core.Models;
using Repository.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HqDownloadManager.Models {
    public class UserFavorite {
        [PrimaryKey]
        public int Id { get; set; }
        public virtual User User { get; set; }
        public virtual Hq Hq { get; set; }
        public virtual Chapter Chapter { get; set; }
        public virtual Page Page { get; set; }
        public DateTime Date { get; set; }
    }
}
