using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SoftwareKobo.CnblogsAPI.Model;

namespace SoftwareKobo.CnblogsNews.Model
{
    public class CommentWithIndex
    {
        public Comment Comment
        {
            get;
            set;
        }

        public int Index
        {
            get;
            set;
        }
    }
}
