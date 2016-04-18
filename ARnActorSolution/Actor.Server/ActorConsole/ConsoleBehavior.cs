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
using System.Text;
using System.Threading.Tasks;
using Actor.Base;

namespace Actor.Server
{

    public class ConsoleBehavior : Behaviors
    {
        public ConsoleBehavior()
            : base()
        {
            AddBehavior(new ConsoleBehavior<string>());
            AddBehavior(new ConsoleStringListbehavior());
            AddBehavior(new ConsoleDictionaryBehavior());
            AddBehavior(new ConsoleBehavior<int>());
            AddBehavior(new ConsoleBehavior<double>());
            AddBehavior(new ConsoleBehavior<object>());
            //TODO add anything else if needed
        }
    }

    public class ConsoleBehavior<T> : Behavior<T>
    {
        public ConsoleBehavior()
            : base()
        {
            Pattern = t => { return t is T; };
            Apply = DoConsole;
        }
        private void DoConsole(T msg)
        {
            Console.WriteLine(msg.ToString());
        }
    }

    public class ConsoleDictionaryBehavior : Behavior<Dictionary<string,string>>
    {
        public ConsoleDictionaryBehavior()
            : base()
        {
            Pattern = t => t is Dictionary<string,string> ;
            Apply = DoConsole;
        }
        private void DoConsole(Dictionary<string,string> dico)
        {
            foreach (var item in dico)
            {
                Console.WriteLine(String.Format(CultureInfo.InvariantCulture,"{0} - {1}", item.Key,item.Value));
            }
        }
    }

    public class ConsoleStringListbehavior : Behavior<IEnumerable<String>>
    {
        public ConsoleStringListbehavior()
            : base()
        {
            Pattern = t => t is IEnumerable<String>; ;
            Apply = DoConsole;
        }
        private void DoConsole(IEnumerable<String> msg)
        {
            foreach (String s in msg)
            {
                Console.WriteLine(s);
            }
        }
    }

}
