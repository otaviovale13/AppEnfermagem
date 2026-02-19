using System.Text.Json;

namespace AppEnfermagem.Services;

public class UserSession
{
    private static readonly string _userPreferenceKey = "LoggedInUserJson";

    // Variável que guarda o usuário na memória RAM enquanto o app está aberto
    public static Models.Admin LoggedInUser { get; private set; }

    public static void Login(Models.Admin user)
    {
        LoggedInUser = user;

        try
        {
            // Transforma o objeto Usuário em texto (JSON) e salva no celular
            string userJson = JsonSerializer.Serialize(user);
            Preferences.Set(_userPreferenceKey, userJson);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao salvar sessão: {ex.Message}");
        }
    }

    public static void Logout()
    {
        LoggedInUser = null;

        // Remove do armazenamento
        Preferences.Remove(_userPreferenceKey);

        // Limpa também as chaves soltas que você usou no MainViewModel
        Preferences.Remove("UserId");
        Preferences.Remove("UserName");
    }

    // --- NOVO MÉTODO: RESTAURAR SESSÃO ---
    public static bool RestoreSession()
    {
        // 1. Verifica se existe o dado salvo
        if (Preferences.ContainsKey(_userPreferenceKey))
        {
            try
            {
                // 2. Pega o texto salvo
                string userJson = Preferences.Get(_userPreferenceKey, string.Empty);

                if (!string.IsNullOrEmpty(userJson))
                {
                    // 3. Converte de volta para Objeto User e joga na memória
                    LoggedInUser = JsonSerializer.Deserialize<Models.Admin>(userJson);
                    return LoggedInUser != null; // Retorna VERDADEIRO se deu certo
                }
            }
            catch
            {
                // Se o dado estiver corrompido, limpa tudo para evitar crash
                Logout();
            }
        }
        return false; // Retorna FALSO se não tinha login salvo
    }
}