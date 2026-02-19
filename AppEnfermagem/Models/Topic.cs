using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppEnfermagem.Models;

[Table("Topics", Schema = "Content")]
public class Topic
{
    [Key]
    public int TopicID { get; set; }

    [Required(ErrorMessage = "O nome do tópico é obrigatório.")]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(255)]
    public string? Description { get; set; }

    [StringLength(200)]
    public string? IconPath { get; set; }

    public int DisplayOrder { get; set; } = 0;

    public virtual ICollection<Article> Articles { get; set; } = new List<Article>();
    // Propriedade de Navegação: Um Tópico tem várias Imagens
    public virtual ICollection<TopicImage> Images { get; set; } = new List<TopicImage>();
    public List<TopicImage> Images { get; set; } = new();
}