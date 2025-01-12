namespace Learning_Backend.DTOS
{
        public class ReturnValues<T> where T : class
        {
            public int StatusCode { get; set; }
            public string Message { get; set; }
            public T? Data { get; set; } = null;
            public IEnumerable<T>? DataArray { get; set; }
            public string? Token { get; set; } = null;
            public string? ImagePath { get; set; } = null;
        }
}

