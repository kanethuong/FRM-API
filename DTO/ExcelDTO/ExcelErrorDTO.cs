using System.Collections.Generic;

namespace kroniiapi.DTO.ExcelDTO
{
    public class ExcelErrorDTO
    {
        public string Sheet { get; set; } = string.Empty;
        public int Row { get; set; } = 0;
        public int Column { get; set; } = 0;
        public string ColumnName { get; set; } = string.Empty;
        public ICollection<string> Errors { get; set; } = new List<string>();
        public object Value { get; set; } = new object();
        public virtual string Error
        {
            set
            {
                if (Errors is not null && value is not null)
                {
                    Errors.Add(value);
                }
            }
        }
    }
}
