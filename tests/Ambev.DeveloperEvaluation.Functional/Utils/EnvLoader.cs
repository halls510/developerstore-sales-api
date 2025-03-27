namespace Ambev.DeveloperEvaluation.Functional.Utils;

public static class EnvLoader
{
    public static void LoadEnvFromProjectRoot(string filename = ".env")
    {
        var current = new DirectoryInfo(AppContext.BaseDirectory);

        while (current != null && !File.Exists(Path.Combine(current.FullName, filename)))
        {
            current = current.Parent;
        }

        if (current == null)
        {
            Console.WriteLine($"⚠️ Arquivo {filename} não encontrado subindo diretórios.");
            return;
        }

        var envPath = Path.Combine(current.FullName, filename);
        DotNetEnv.Env.Load(envPath);
        Console.WriteLine($"✅ .env carregado de: {envPath}");
    }
}

