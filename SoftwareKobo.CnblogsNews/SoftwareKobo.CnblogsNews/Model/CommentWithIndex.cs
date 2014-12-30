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

        public CommentWithIndex()
        {
        }
    }
}