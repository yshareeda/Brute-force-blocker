using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;

class Program
{
    private const string Logfilepath = "auth.txt";
    private static readonly Dictionary<string, int> Failedattempts = new();

    static void Main()
    {
        string fullPath = Path.GetFullPath(Logfilepath);
        Console.WriteLine($"[*] Monitoring log file: {fullPath}");

        if (!File.Exists(Logfilepath))
        {
            File.WriteAllText(Logfilepath, "");
        }

        using FileStream stream = new(Logfilepath, Filemode.Open, Fileaccess.Read, Fileshare.ReadWrite);
        using StreamReader reader = new(stream);
        stream.Seek(0, SeekOrigin.End);

        while (true)
        {
            string line = reader.ReadLine();

            if (line != null)
            {
                Match match = Regex.Match(line, @"from\s+(\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3})");

                if (match.Success)
                {
                    string ip = match.groups[1].Value;
                    Failedattempts[ip] = Failedattempts.GetValueOrDefault(ip, 0) + 1;

                    Console.WriteLine($"[-] Failed attempt #{Failedattempts[ip]} from IP: {ip}");

                    if (Failedattempts[ip] >= 5)
                    {
                        Console.WriteLine($"[!] Brute force threshold met. Blocking {ip} in Windows Firewall...");
                        BlockIp(ip);
                        Failedattempts[ip] = 0;
                    }
                }
            }
            else
            {
                Thread.Sleep(200);
            }
        }
    }

    private static void BlockIp(string ip)
    {
        ProcessStartInfo psi = new ProcessStartInfo
        {
            FileName = "netsh",
            Arguments = $"advfirewall firewall add rule name=\"Block_{ip}\" dir=in action=block remoteip={ip}",
            CreateNoWindow = true,
            UseShellExecute = false
        };

        using Process process = Process.Start(psi);
        process?.WaitForExit();

        Console.WriteLine($"[SUCCESS] Firewall rule added for IP: {ip}");
    }
}