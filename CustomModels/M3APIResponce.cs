using System.Collections.Generic;

namespace Brandix.DCAP.API.CustomModels
{
    public class Result
    {
        public string transaction { get; set; }
        public List<object> records { get; set; }
        public string errorMessage { get; set; }
        public string errorType { get; set; }
        public string errorCode { get; set; }
        public string errorCfg { get; set; }
        public string errorField { get; set; }
    }

    public class RootObject
    {
        public List<Result> results { get; set; }
        public bool wasTerminated { get; set; }
        public int nrOfSuccessfullTransactions { get; set; }
        public int nrOfFailedTransactions { get; set; }
    }
}
