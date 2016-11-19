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
using Actor.Base;

namespace Actor.Server
{

    public class DirectoryActor : BaseActor
    {
        public enum DirectoryRequest { Find } ;
        private Dictionary<string, IActor> fDictionary = new Dictionary<string, IActor>();
        private static Lazy<DirectoryActor> fDirectory = new Lazy<DirectoryActor>(() => new DirectoryActor(), true);
        public DirectoryActor()
            : base()
        {
            Console.WriteLine("Dictionary starts and autoregisters");
            fDictionary.Add("Directory", this);

            Behaviors bhvs = new Behaviors();
            bhvs.AddBehavior(new ActionBehavior<IActor>()) ;
            bhvs.AddBehavior(new ActionBehavior<IActor,string>());
            Become(bhvs);
        }

        public static DirectoryActor GetDirectory()
        {
            return fDirectory.Value;
        }

        public string Stat()
        {
            return "Directory entries " + fDictionary.Count().ToString(CultureInfo.InvariantCulture);
        }

        public void Disco(IActor anActor)
        {
            this.SendMessage((Action<IActor>)DoDisco, anActor);
        }

        public void Register(IActor anActor, string aKey)
        {
            this.SendMessage((Action<IActor,string>)DoRegister, anActor, aKey);
        }

        public void Find(IActor anActor, string aKey)
        {
            this.SendMessage((Action<IActor, string>)DoFind, anActor, aKey);
        }

        public IFuture<DirectoryRequest,IActor> FindActor(string aKey)
        {
            IFuture<DirectoryRequest,IActor> future = new Future<DirectoryRequest, IActor>();
            this.SendMessage((Action<IActor, string>)DoFind, future, aKey);
            return future;
        }

        public IActor GetActorByName(string actorName)
        {
            var future = new Future<DirectoryRequest, IActor>();
            Find(future, actorName);
            return future.Result().Item2;
        }

        private void DoDisco(IActor anActor)
        {
            Dictionary<string, string> directory = new Dictionary<string, string>();
            var fullhost = new ConfigManager().Host().Host;
            foreach (string key in fDictionary.Keys)
            {
                var value = fDictionary[key];
                directory.Add(key,fullhost + value.Tag.Key());
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
                anActor.SendMessage(DirectoryRequest.Find,Relative);
            }
            else
            {
                anActor.SendMessage(DirectoryRequest.Find, (IActor)null);
            }
        }

    }

}