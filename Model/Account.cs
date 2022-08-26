using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamAccountSwitcher.Model
{
    [Serializable]
    public class Account : EqualityComparer<Account>, ICloneable
    {
        public string ViewName { get; set; }
        public string password { get; set; }
        public string login { get; set; }
        public string tag { get; set; }
        public string ImageUrl { get; set; }
        public string SteamUrl { get; set; }
        public int ID { get; set; }
        public string SteamID64 { get; set; }

        public DateTime lastFetch { get; set; }

        public Account(string login, string password)
        {
            this.login = login;
            this.password = password;
        }

        public override bool Equals(Account x, Account y)
        {
            if (x.login == y.login && x.ViewName == y.ViewName && x.password == y.password && x.tag == y.tag)
                return true;
            else
                return false;
        }

        public override int GetHashCode(Account obj)
        {
            throw new NotImplementedException();
        }
        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
