namespace EVoting.Server.Utils.ShellExecutors
{
    public class ShellResult<T>
    {
        public bool Success { get; set; }
        public T Result { get; set; }
        public string Message { get; set; }
    }
}
