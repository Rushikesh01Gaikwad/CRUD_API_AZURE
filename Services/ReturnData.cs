namespace CRUD_API.Services
{
    public class ReturnData
    {
        public object Data { get; set; }
        public string Message { get; set; }
        public int Status { get; set; } = 1;
        public string Exception { get; set; }
    }
}
