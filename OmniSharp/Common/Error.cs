namespace OmniSharp.Common
{
    public class Error
    {
        public string Message { get; set; }
        public int Line { get; set; }
        public int Column { get; set; }
        public string FileName { get; set; }
    }
}
