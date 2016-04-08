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

    public interface IBehavior 
    {
        void LinkBehaviors(Behaviors someBehaviors);
        void StandardApply(Object aT);
        bool StandardPattern(Object aT);
        TaskCompletionSource<Object> StandardCompletion {get;}
    }

    public interface IBehavior<T> : IBehavior 
    {
        Func<T,Boolean> Pattern { get; }
        Action<T> Apply { get; }
        TaskCompletionSource<T> Completion { get; }
    }

    public interface IBehavior<A,T> : IBehavior
    {
        Func<A, T, Boolean> Pattern { get; }
        Action<A, T> Apply { get; }
        TaskCompletionSource<Tuple<A,T>> Completion { get; }
    }

    public interface IBehavior<O, D, A> : IBehavior
    {
        Func<O, D, A, Boolean> Pattern { get; }
        Action<O, D, A> Apply { get; }
        TaskCompletionSource<Tuple<O, D, A>> Completion { get; }
    }

}
