namespace ConsoleApplication1
{
    public class PaginatedConfiguration
    {
        public PaginatedConfiguration()
        {
            PageIndex = 1;
            PageSize = 10;
        }

        public PaginatedConfiguration(int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
        }

        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public bool Enabled { get; set; }
    }
}