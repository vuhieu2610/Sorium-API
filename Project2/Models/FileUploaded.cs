using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project2.Models
{
    public class FileUploaded
    {
        public int Id { set; get; }
        public string FileName { set; get; }
        public string Url { set; get; }
        public string Table { set; get; }
        public int RowId { set; get; }
        public string Desc { set; get; }
    }
    public class FileUploadCompact
    {
        public int Id { set; get; }
        public string FIleName { set; get; }
        public string Url { set; get; }
        public string Desc { set; get; }
    }
}