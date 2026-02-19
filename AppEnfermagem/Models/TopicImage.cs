using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace AppEnfermagem.Models;

public class TopicImage
{
    public int ImageId { get; set; }
    public int TopicId { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string? Caption { get; set; }
    public int DisplayOrder { get; set; } = 0;
}
