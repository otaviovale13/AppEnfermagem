using System.Globalization;

namespace AppEnfermagem.Converters;

public class IdToTextConverter : IValueConverter
{
    // Converte o ID (int) para o texto do botão
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int id)
        {
            // Se o ID for 0, é um novo registro. Caso contrário, é edição.
            return id == 0 ? "Salvar Imagem" : "Atualizar Imagem";
        }
        return "Salvar";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}