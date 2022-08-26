using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SteamAccountSwitcher.Model;
using System.Text.Json;
using System.IO;

namespace SteamAccountSwitcher.Services
{
    class AccountDataStorage
    {
        static string pathToSave = AssemblyDirectory + "\\accounts.json";

        public static void Init()
        {
            if (!File.Exists(pathToSave))
            {
                File.WriteAllText(pathToSave, "[]");
            }
        }
        public static string AssemblyDirectory
        {
            get
            {
                string codeBase = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }
        public static List<Account> ReadAccounts()
        {
            return JsonSerializer.Deserialize<List<Account>>(File.ReadAllText(pathToSave));
        }
        public static void AddAccount(Account acc)
        {
            try
            {
                var readed = ReadAccounts();
                acc.ID = readed.Count != 0  ? readed.Last().ID + 1 : 0;
                readed.Add(acc);
                File.WriteAllText(pathToSave, JsonSerializer.Serialize(readed));
            } catch (Exception ex) { System.Windows.MessageBox.Show("Ошибка чтения данных, очистите файл accounts.json\n" + ex.Message +"\n" + ex.StackTrace); }
        }
        public static void SaveAccounts(List<Account> accs)
        {
            File.WriteAllText(pathToSave , JsonSerializer.Serialize(accs));
        }
        public static bool ReplaceAccount(Account replace, Account toreplace)
        {
            var readed = ReadAccounts();
            int i = readed.FindIndex(a => a.Equals(a, toreplace));
            if (i == -1)
                return false;
            readed[i] = replace;
            SaveAccounts(readed);
            return true;
        }
        public static void ReplaceAtIndex(int index, Account acc)
        {
            var readed = ReadAccounts();
            readed[index] = acc;
            SaveAccounts(readed);
        }

        public static void DeleteAccount(Account acc)
        {
            var readed = ReadAccounts();
            readed.RemoveAll(a => a.Equals(a, acc));
            SaveAccounts(readed);
        }
    }
}
