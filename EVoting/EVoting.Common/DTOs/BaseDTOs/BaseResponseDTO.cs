using System;
using System.Collections.Generic;
using System.Text;

namespace EVoting.Common.DTOs.BaseDTOs
{
    public class BaseResponseDTO
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }

    public class BaseResponseDTO<T>: BaseResponseDTO
    {
        public T Result { get; set; }
    }
}
    