using System;
using System.Linq;

namespace DimaDevi.Libs
{
    public class Property
    {
        public class RemoteWMICredential : IDisposable
        {
            /// <summary>
            /// Example: COMPUTER-NAME or {IP}
            /// </summary>
            public string Domain { set; get; }
            /// <summary>
            /// Example: "Administrador"
            /// </summary>
            public string Username { set; get; }
            public string Password { set; get; }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="domain">Example: COMPUTER-NAME or IP</param>
            /// <param name="user">Example: @"{IP}\Administrador"</param>
            /// <param name="pass"></param>
            public RemoteWMICredential(string domain, string user, string pass)
            {
                Domain = domain;
                Username = user;
                Password = pass;
            }
            //[Obfuscation(Feature = "all")]
            public bool IsEmpty()
            {
                return this.GetType().GetProperties().Where(x=>x.GetValue(this) != null).All(x => string.IsNullOrEmpty(x.GetValue(this).ToString()));
            }

            public void Dispose()
            {
                this.RandomizedStringDispose();
            }
        }
    }
}
