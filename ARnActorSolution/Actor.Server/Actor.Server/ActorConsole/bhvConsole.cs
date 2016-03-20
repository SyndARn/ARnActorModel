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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Actor.Base
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "bhv")]
    public class bhvConsole : Behaviors
    {
        public bhvConsole()
            : base()
        {
            AddBehavior(new bhvConsole<string>());
            AddBehavior(new bhvConsoleStringList());
            AddBehavior(new bhvConsoleDictionary());
            AddBehavior(new bhvConsole<int>());
            AddBehavior(new bhvConsole<double>());
            AddBehavior(new bhvConsole<object>());
            //TODO add anything else if needed
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "bhv")]
    public class bhvConsole<T> : bhvBehavior<T>
    {
        public bhvConsole()
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

    public class bhvConsoleDictionary : bhvBehavior<Dictionary<string,string>>
    {
        public bhvConsoleDictionary()
            : base()
        {
            Pattern = t => t is Dictionary<string,string> ;
            Apply = DoConsole;
        }
        private void DoConsole(Dictionary<string,string> dico)
        {
            foreach (var item in dico)
            {
                Console.WriteLine(item.Key+" - "+item.Value);
            }
        }
    }

    public class bhvConsoleStringList : bhvBehavior<IEnumerable<String>>
    {
        public bhvConsoleStringList()
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
