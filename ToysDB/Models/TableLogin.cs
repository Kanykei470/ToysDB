using System;
using System.Collections.Generic;

#nullable disable

namespace ToysDB.Models
{
    public partial class TableLogin
    {
        public byte Id { get; set; }
        public string UserLogin { get; set; }
        public string UserPassword { get; set; }
    }
}
