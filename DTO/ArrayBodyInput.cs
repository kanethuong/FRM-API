using System.Collections.Generic;

namespace kroniiapi.DTO
{
    public class ArrayBodyInput<T>
    {
        public ICollection<T> arrayData { get; set; }
    }
}