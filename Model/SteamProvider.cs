﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.Win32;
using System.Net;
using System.Text.RegularExpressions;
using System.Text.Json;
using System.IO;
using System.Xml;
using System.Windows;

namespace SteamAccountSwitcher.Model
{
    public struct Game
    {
        public string id { get; set; }
        public string link { get; set; }
    }
    class SteamProvider
    {
        private static readonly string SteamPath_x64 = @"SOFTWARE\Wow6432Node\Valve\Steam";
        private static readonly string SteamPath_x32 = @"Software\Valve\Steam";
        public static string location = GetLocationSteam();


        public static string SteamStatus()
        {
            try
            {
                WebRequest.Create("https://steamcommunity.com/").GetResponse();
                return "OK";
            }
            catch (Exception ex)
            {
                return "NoConnection: " + ex.Message;
            }
        }

        public static async void Login(string login, string pass)
        {
            if (SteamRunning())
                LogOut();
            await Task.Delay(2000);
            Process steam_process = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = location + "\\steam.exe",
                    Arguments = $"-login {login} {pass}"
                }
            };
            steam_process.Start();
        }
        public static bool SteamRunning()
        {
            return Process.GetProcessesByName("steam.exe") != null;
        }

        public static string GetLocationSteam(string Inst = "InstallPath", string Source = "SourceModInstallPath")
        {
            using (var BaseSteam = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, (Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32)))
            {
                using (RegistryKey Key = BaseSteam.OpenSubKey(SteamPath_x64, Environment.Is64BitOperatingSystem))
                {
                    using (RegistryKey Key2 = BaseSteam.OpenSubKey(SteamPath_x32, Environment.Is64BitOperatingSystem))
                    {
                        return Key?.GetValue(Inst)?.ToString() ?? Key2?.GetValue(Source)?.ToString();
                    }
                }
            }
        }
        static void LogOut()
        {
            var cmd = Process.Start(new ProcessStartInfo()
            {
                CreateNoWindow = true,
                FileName = "cmd.exe",
                Arguments = "/c taskkill /im steam.exe /f",
                WindowStyle = ProcessWindowStyle.Hidden,
                UseShellExecute = true
            });
        }
        private static string ConvertSteamId(string steamID64)
        {
            long communityId = long.Parse(steamID64);
            if (communityId < 76561197960265729L || !Regex.IsMatch(communityId.ToString((IFormatProvider)System.Globalization.CultureInfo.InvariantCulture), "^7656119([0-9]{10})$"))
            {
                return string.Empty;
            }
            return string.Format("{0}", communityId - 76561197960265728L);
        }

        public async static Task<List<Game>> GetGamesInFolder(string SteamID64)
        {
            try
            {
                List<Game> result = new List<Game>();
                await Task.Run(() =>
                {
                    var steamId32 = ConvertSteamId(SteamID64);
                    var ids = Directory.GetDirectories($"{location}\\userdata\\{steamId32}").Select(s => s.Split('\\').Last()).ToArray();

                    foreach (var item in ids)
                        result.Add(new Game { id = item, link = "https://store.steampowered.com/app/" + (char.IsNumber(item[0]) ? item : "") });
                    return result;
                });
                return result;
            }
            catch { return null; }
        }
        static void CopyDirectory(string begin_dir, string end_dir)
        {
            //Берём нашу исходную папку
            DirectoryInfo dir_inf = new DirectoryInfo(begin_dir);
            //Перебираем все внутренние папки
            foreach (DirectoryInfo dir in dir_inf.GetDirectories())
            {
                //Проверяем - если директории не существует, то создаём;
                if (Directory.Exists(end_dir + "\\" + dir.Name) != true)
                {
                    Directory.CreateDirectory(end_dir + "\\" + dir.Name);
                }

                //Рекурсия (перебираем вложенные папки и делаем для них то-же самое).
                CopyDirectory(dir.FullName, end_dir + "\\" + dir.Name);
            }

            //Перебираем файлики в папке источнике.
            foreach (string file in Directory.GetFiles(begin_dir))
            {
                //Определяем (отделяем) имя файла с расширением - без пути (но с слешем "\").
                string filik = file.Substring(file.LastIndexOf('\\'), file.Length - file.LastIndexOf('\\'));
                //Копируем файлик с перезаписью из источника в приёмник.
                File.Copy(file, end_dir + "\\" + filik, true);
            }
        }

        public static void ExportSetting(string origin, string dest, List<Game> games)
        {
            //in develop...
            origin = ConvertSteamId(origin.ToString());
            dest = ConvertSteamId(dest.ToString());

            string path = $"{location}\\userdata\\{dest}\\";
            string path_origin = $"{location}\\userdata\\{origin}\\";
            //Delete folders in dest and copy folders from origin to dest
            foreach (var game in games)
            {
                if (Directory.Exists(path + game.id))
                {
                    Directory.Delete(path + game.id, true);
                    Directory.CreateDirectory(path + game.id);
                }
                CopyDirectory(path_origin + game.id, path + game.id);

            }
        }

        public static async Task<Account> FetchProfileData(Account account, Action FallbackCallback, bool caching = true)
        {
            if (account.SteamUrl == null)
                return account;

            if (caching)
            if (account.lastFetch == new DateTime() || (DateTime.Today - account.lastFetch).Days > 3)
                account.lastFetch = DateTime.Today;
            else
                return account;

            var result = await Task.Run(() =>
            {
                try
                {
                    XmlDocument doc = new XmlDocument();
                    account.SteamUrl = account.SteamUrl.Contains("http") ? account.SteamUrl : "https://steamcommunity.com/id/" + account.SteamUrl;

                    if (account.SteamUrl.Contains("?xml=1"))
                        doc.Load(account.SteamUrl);
                    else
                        doc.Load(account.SteamUrl + "?xml=1");
                    XmlElement xRoot = doc.DocumentElement;
                    if (xRoot != null)
                    {
                        // обход всех узлов в корневом элементе
                        foreach (XmlElement xnode in xRoot)
                        {
                            if (xnode.Name == "steamID")
                                account.ViewName = xnode.InnerText;
                            if (xnode.Name == "avatarMedium" || xnode.Name == "avatarFull")
                                account.ImageUrl = xnode.InnerText;
                            if (xnode.Name == "steamID64")
                                account.SteamID64 = xnode.InnerText;
                        }
                    }
                }
                catch{ FallbackCallback.Invoke(); }
                return Task.FromResult(account);
            });
            return result;
        }
    }
}
