using System;
using System.Collections.Generic;

namespace AppEnfermagem.Models;

public class ForumPost
{
    public int PostID { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ContentBody { get; set; } = string.Empty;
    public string AuthorName { get; set; } = "Anônimo";
    public DateTime CreatedAt { get; set; }

    // Lista de respostas atreladas a esta dúvida
    public List<ForumReply> Replies { get; set; } = new();
}