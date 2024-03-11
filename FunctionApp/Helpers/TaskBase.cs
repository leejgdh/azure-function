namespace FunctionApp.Helpers
{
    /// <summary>
    /// 각종 로직들의 결과 확인을 위한 Wrapper class
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TaskBase<T>
    {
        public TaskBase(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public bool IsSuccess { get; set; }

        public T? Result { get; set; }

        public string? Key { get; set; }

        public string? Message { get; set; }

        public Exception? Exception { get; set; }
    }
}
