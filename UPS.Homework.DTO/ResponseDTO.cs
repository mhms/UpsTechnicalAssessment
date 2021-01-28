using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UPS.Homework.DTO
{
    public class ResponseDTO<T>
    {
        public int code { get; set; }
        public Meta meta { get; set; }

        public T data;

    }
    public class Pagination
    {
        public int total { get; set; }
        public int pages { get; set; }
        public int page { get; set; }
        public int limit { get; set; }
    }

    public class Meta
    {
        public Pagination pagination { get; set; }
    }



    
       
    

}
