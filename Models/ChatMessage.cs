﻿using System;
using System.ComponentModel.DataAnnotations;

namespace StudentSwipe.Models
{
    public class ChatMessage
    {
        public int Id { get; set; }

        [Required]
        public string SenderId { get; set; }

        [Required]
        public string ReceiverId { get; set; }

        [Required]
        public string Message { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        [Required]
        public string Context { get; set; }  // "Roommate" or "Study"
    }
}
