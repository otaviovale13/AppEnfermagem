namespace AppEnfermagem.Models;

public class TopicImage
{
    public int ImageID { get; set; }
    public int TopicID { get; set; }
    public string ImageUrl { get; set; }
    public string Caption { get; set; }
    public int DisplayOrder { get; set; }
}