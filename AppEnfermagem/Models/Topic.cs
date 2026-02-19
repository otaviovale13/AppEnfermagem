using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

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

    // Mantém uma única definição para Images para evitar ambiguidade (CS0229).
    // Uso ICollection<T> é preferível para compatibilidade com EF e flexibilidade.
    public virtual ICollection<TopicImage> Images { get; set; } = new List<TopicImage>();

    // Mantém Articles como colecção virtual para EF (se aplicável).
    public virtual ICollection<Article> Articles { get; set; } = new List<Article>();
}