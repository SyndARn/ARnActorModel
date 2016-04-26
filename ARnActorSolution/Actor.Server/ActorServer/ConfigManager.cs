using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace Actor.Server
{
    public class ConfigManager
    {

        private ISerializeService fSerializeService;

        public ISerializeService GetSerializeService()
        {
            if (fSerializeService == null)
            {
                string r = ConfigurationManager.AppSettings["SerializeService"];
                // parse r to get a better thing than this with reflection
                if (string.IsNullOrEmpty(r))
                {
                    fSerializeService = new NetDataContractSerializeService();
                }
                else
                {
                    switch (r)
                    {
                        case "NetDataContractSerializeService":
                            {
                                fSerializeService = new NetDataContractSerializeService();
                                break;
                            }
                        default:
                            {
                                fSerializeService = new NetDataContractSerializeService();
                                break;
                            }
                    }
                }
                
            }
            return fSerializeService;
        }
    }
}
