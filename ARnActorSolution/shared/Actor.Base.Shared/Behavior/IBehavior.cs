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
        void StandardApply(object aT);
        bool StandardPattern(object aT);
        TaskCompletionSource<object> StandardCompletion {get;}
    }

    public interface IBehavior<T> : IBehavior 
    {
        Func<T, bool> Pattern { get; }
        Action<T> Apply { get; }
        TaskCompletionSource<T> Completion { get; }
    }

    public interface IBehavior<T1, T2> : IBehavior
    {
        Func<T1, T2, bool> Pattern { get; }
        Action<T1, T2> Apply { get; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        TaskCompletionSource<IMessageParam<T1,T2>> Completion { get; }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1005:AvoidExcessiveParametersOnGenericTypes")]
    public interface IBehavior<T1, T2, T3> : IBehavior
    {
        Func<T1, T2, T3, bool> Pattern { get; }
        Action<T1, T2, T3> Apply { get; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        TaskCompletionSource<IMessageParam<T1, T2, T3>> Completion { get; }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1005:AvoidExcessiveParametersOnGenericTypes")]
    public interface IBehavior<T1, T2, T3, T4> : IBehavior
    {
        Func<T1, T2, T3, T4, bool> Pattern { get; }
        Action<T1, T2, T3, T4> Apply { get; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        TaskCompletionSource<IMessageParam<T1, T2, T3, T4>> Completion { get; }
    }
}
