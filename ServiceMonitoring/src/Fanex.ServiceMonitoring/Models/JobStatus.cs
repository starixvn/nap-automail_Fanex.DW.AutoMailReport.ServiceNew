namespace Fanex.ServiceMonitoring.Models
{
    public enum JobStatus
    {
        Waiting = 0,
        Running = 1,
        DataIsNotReady = 2,
        JobFailed = 3,
        Success = 4,
        Abort = 6,
        SuccessWithoutSendmail = 7,
        Missing = 8
    }
}