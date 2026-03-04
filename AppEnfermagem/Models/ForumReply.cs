using System;

namespace AppEnfermagem.Models;

public class ForumReply
{
    public int ReplyID { get; set; }
    public int PostID { get; set; }
    public string ContentBody { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;
    public bool IsAdminReply { get; set; }
    public DateTime CreatedAt { get; set; }
}