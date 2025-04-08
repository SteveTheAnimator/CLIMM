using Newtonsoft.Json.Linq;
using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.NamingConventionBinder;
using System.IO.Compression;

class Program
{
    static int Main(string[] args)
    {
        var gamePathOption = new Option<string>(
            "--game-path",
            description: "The path to the Unity game"
        )
        {
            IsRequired = true
        };

        var bepinexCommand = new Command("install-bepinex", "Installs BepInEx")
        {
            gamePathOption
        };

        bepinexCommand.Handler = CommandHandler.Create<string>(async (gamePath) => await InstallBepInEx(gamePath));

        var modCommand = new Command("install-mod", "Installs a mod from a GitHub URL")
        {
            gamePathOption,
            new Option<string>("--mod-url", "The GitHub URL of the mod")
            {
                IsRequired = true
            }
        };

        modCommand.Handler = CommandHandler.Create<string, string>(InstallMod);

        var rootCommand = new RootCommand
        {
            bepinexCommand,
            modCommand
        };

        rootCommand.Description = "CLIMM";

        return rootCommand.InvokeAsync(args).Result;
    }

    static async Task InstallBepInEx(string gamePath, string version = "5.4.23.2", string x = "64")
    {
        string bepinexUrl = $"https://github.com/BepInEx/BepInEx/releases/download/v{version}/BepInEx_win_x{x}_{version}.zip";
        string tempFile = Path.GetTempFileName();

        using (HttpClient client = new HttpClient())
        {
            var response = await client.GetAsync(bepinexUrl);
            response.EnsureSuccessStatusCode();
            await using (var fs = new FileStream(tempFile, FileMode.Create))
            {
                await response.Content.CopyToAsync(fs);
            }
        }

        ZipFile.ExtractToDirectory(tempFile, gamePath, true);
        File.Delete(tempFile);

        Console.WriteLine("BepInEx installed successfully.");
    }

    static async Task InstallMod(string gamePath, string modUrl)
    {
        string tempFile = Path.GetTempFileName();
        string tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

        using (HttpClient client = new HttpClient())
        {
            client.DefaultRequestHeaders.UserAgent.ParseAdd("request");
            var response = await client.GetAsync(modUrl);
            response.EnsureSuccessStatusCode();
            await using (var fs = new FileStream(tempFile, FileMode.Create))
            {
                await response.Content.CopyToAsync(fs);
            }
        }

        Directory.CreateDirectory(tempDir);
        ZipFile.ExtractToDirectory(tempFile, tempDir, true);

        string bepinexPath = Path.Combine(gamePath, "BepInEx");
        string pluginsPath = Path.Combine(bepinexPath, "plugins");
        Directory.CreateDirectory(pluginsPath);

        MoveModContents(tempDir, bepinexPath, pluginsPath);

        File.Delete(tempFile);
        Directory.Delete(tempDir, true);

        Console.WriteLine("Mod installed successfully.");
    }

    static void MoveModContents(string sourceDir, string bepinexPath, string pluginsPath)
    {
        foreach (var dir in Directory.GetDirectories(sourceDir, "*", SearchOption.AllDirectories))
        {
            string dirName = Path.GetFileName(dir).ToLowerInvariant();

            if (dirName == "plugins")
            {
                string targetDir = dirName == "plugins" ? pluginsPath : bepinexPath;

                foreach (var file in Directory.GetFiles(dir))
                {
                    var destFile = Path.Combine(targetDir, Path.GetFileName(file));
                    File.Move(file, destFile, true);
                }

                foreach (var subDir in Directory.GetDirectories(dir))
                {
                    var destDir = Path.Combine(targetDir, Path.GetFileName(subDir));
                    if (Directory.Exists(destDir))
                    {
                        Directory.Delete(destDir, true);
                    }
                    Directory.Move(subDir, destDir);
                }

                Directory.Delete(dir, true);
            }
        }
    }
}
