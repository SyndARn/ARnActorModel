/*****************************************************************************
		               ARnActor Actor Model Library .Net
     
	 Copyright (C) {2015}  {ARn/SyndARn} 
 
 
     This program is free software; you can redistribute it and/or modify 
     it under the terms of the GNU General Public License as published by 
     the Free Software Foundation; either version 2 of the License, or 
     (at your option) any later version. 
 
 
     This program is distributed in the hope that it will be useful, 
     but WITHOUT ANY WARRANTY; without even the implied warranty of 
     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the 
     GNU General Public License for more details. 
 
 
     You should have received a copy of the GNU General Public License along 
     with this program; if not, write to the Free Software Foundation, Inc., 
     51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA. 
*****************************************************************************/
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Actor.Base
{

    public class actDirectory : actActor
    {
        public enum DirectoryRequest { reqFind } ;
        private Dictionary<string, IActor> fDictionary = new Dictionary<string, IActor>();
        private static Lazy<actDirectory> fDirectory = new Lazy<actDirectory>(() => new actDirectory(), true);
        public actDirectory()
            : base()
        {
            Console.WriteLine("Dictionary starts and autoregisters");
            fDictionary.Add("Directory", this);

            Behaviors bhvs = new Behaviors();
            bhvs.AddBehavior(new bhvAction<IActor>()) ;
            bhvs.AddBehavior(new bhvAction<IActor,string>());
            BecomeMany(bhvs);
        }

        public static actDirectory GetDirectory()
        {
            return fDirectory.Value;
        }

        public string Stat()
        {
            return "Directory entries " + fDictionary.Count().ToString();
        }

        public void Disco(IActor anActor)
        {
            SendMessage(new Tuple<Action<IActor>,IActor>(DoDisco, anActor));
        }

        public void Register(IActor anActor, string aKey)
        {
            SendMessage(new Tuple<Action<IActor, string>, IActor, string>(DoRegister, anActor, aKey));
        }

        public void Find(IActor anActor, string aKey)
        {
            SendMessage(new Tuple<Action<IActor, string>, IActor, string>(DoFind, anActor, aKey));
        }

        private void DoDisco(IActor anActor)
        {
            Dictionary<string, string> directory = new Dictionary<string, string>();
            var localhost = Dns.GetHostName();
            var servername = ActorServer.GetInstance().Name;
            var prefix = "http://";
            var suffix = ":" + ActorServer.GetInstance().Port.ToString(CultureInfo.InvariantCulture);
            var fullhost = prefix + localhost + suffix + "/" + servername + "/";
            foreach (string key in fDictionary.Keys)
            {
                var value = fDictionary[key];
                directory.Add(key,fullhost + value.Tag.Id);
            }
            anActor.SendMessage(directory);
        }

        private void DoRegister(IActor anActor,string msg)
        {
            if (fDictionary.Keys.Any(t => t == msg) == false )
                fDictionary.Add(msg,anActor);
        }

        private void DoFind(IActor anActor,string msg)
        {
            // Exists
            IActor Relative = null;
            if (fDictionary.TryGetValue(msg, out Relative))
            {
                anActor.SendMessage(Tuple.Create(DirectoryRequest.reqFind,Relative));
            }
            else
            {
                anActor.SendMessage(new Tuple<DirectoryRequest,IActor>(DirectoryRequest.reqFind, null));
            }
        }

    }

}



