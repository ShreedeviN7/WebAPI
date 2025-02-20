namespace WebAPITemplate.BusinessModel
{
    public class ClsReturnResult
    {
        public Int64 statusCode { get; set; }
        public Boolean isSuccess { get; set; }
        public String StatusMessage { get; set; }
        public DateTime RequestDTm { get; set; }
        public DateTime ResponseDTm { get; set; }
        public TimeSpan ResponseDuration { get; set; }
        public String TotalRecords { get; set; }
        public object ReturnLst { get; set; }
    }
    public class ResultException
    {
        public string ExceptionMessage { get; set; }
    }

    public class LogError
    {
        public Int64 IdLogError { get; set; }
        public Int64 IdInstitute { get; set; }
        public String ModuleName { get; set; }
        public String ErrorLog { get; set; }
        public DateTime ErrorLogDateTime { get; set; }

    }
}
