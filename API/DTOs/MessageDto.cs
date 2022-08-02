using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace API.DTOs
{
    public class MessageDto
    {
         public int Id { get; set; }

        public int SenderId { get; set; }

        public string SenderUsername { get; set; }

        public string SenderImageUrl { get; set; }

        public int RecipientId { get; set; }

        public string RecipientUserName { get; set; }

        public string RecipientImageUrl { get; set; }

        public string Content { get; set; }

        public DateTime? DateRead { get; set; }

        public DateTime MessageSent { get; set; } = DateTime.Now;

        [JsonIgnore]
        public bool RecipientDeleted { get; set; }

        [JsonIgnore]
        public bool SenderDeleted { get; set; }

    }
}