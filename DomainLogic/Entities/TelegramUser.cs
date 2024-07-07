﻿namespace DomainLogic.Entities;

public class TelegramUser
{
    public long Id { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Username { get; set; }
    
    public string? PhotoUrl { get; set; }
    
    public DateTimeOffset CreatedAt { get; set; }
}
