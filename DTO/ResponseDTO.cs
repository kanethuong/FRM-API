using System.Collections.Generic;
using Newtonsoft.Json;

namespace kroniiapi.DTO
{
    public class ResponseDTO
    {
        public int Status { get; set; } = 500;
        public string Message { get; set; }
        public object Errors { get; set; } = new object();
        private IDictionary<int, string> statusMessage = new Dictionary<int, string>(){
            {200, "Request has succceeded"},
            {201, "Resource has been created"},
            {204, "Server has fulfilled the request"},
            {400, "Request could not be understood by the server due to incorrect syntax"},
            {401, "Request requires user authentication information"},
            {403, "Client does not have access rights to the content"},
            {404, "Resource not found"},
            {409, "Conflict with the current state of resource"},
            {415, "Mediatype is not supported by the server"}
        };

        public ResponseDTO() { }

        public ResponseDTO(int Status)
        {
            this.Status = Status;

            if (statusMessage.ContainsKey(Status))
            {
                this.Message = statusMessage[Status];
            }
            else
            {
                this.Message = "The server encountered an unexpected condition";
            }
        }

        public ResponseDTO(int Status, string Message)
        {
            this.Status = Status;
            this.Message = Message;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}