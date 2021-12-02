using System.Collections.Generic;

namespace kroniiapi.DTO.ExcelDTO
{
    public class ExcelResponseDTO : ResponseDTO
    {
        public new ICollection<ExcelErrorDTO> Errors
        {
            get
            {
                return base.Errors as ICollection<ExcelErrorDTO>;
            }
            set
            {
                base.Errors = value;
            }
        }

        public ExcelResponseDTO() : base() { }

        public ExcelResponseDTO(int Status) : base(Status) { }

        public ExcelResponseDTO(int Status, string Message) : base(Status, Message) { }
    }
}
