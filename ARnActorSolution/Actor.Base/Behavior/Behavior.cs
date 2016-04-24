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
using System.Threading.Tasks;
using System.Linq;

namespace Actor.Base
{
    /// <summary>
    /// Behaviors holds many behaviors.
    /// Behaviors are actor brain.
    /// Behaviors when null in an actor means this actor is dead (it can't change anymore his own behavior from this point)
    /// </summary>
    public class Behaviors
    {
        private List<IBehavior> fList = new List<IBehavior>();

        public BaseActor LinkedActor { get; private set; }

        public Behaviors()
        {
        }

        public IEnumerable<IBehavior> AllBehaviors()
        {
            return fList.ToList(); // force clone
        }

        public void LinkToActor(BaseActor anActor)
        {
            LinkedActor = anActor;
        }

        public bool FindBehavior(IBehavior aBehavior)
        {
            return fList.Contains(aBehavior);
        }

        public void AddBehavior(IBehavior aBehavior)
        {
            if (aBehavior != null)
            {
                aBehavior.LinkBehaviors(this);
                fList.Add(aBehavior);
            }
        }
        public void RemoveBehavior(IBehavior aBehavior)
        {
            CheckArg.Behavior(aBehavior);
            aBehavior.LinkBehaviors(null);
            fList.Remove(aBehavior);
        }

    }

    public class Behavior : Behavior<object>
    {
        public Behavior() : base()
        {
        }
    }

    public class Behavior<A, T> : IBehavior<A, T>, IBehavior
    {
        public Func<A, T, Boolean> Pattern { get; protected set; }
        public Action<A, T> Apply { get; protected set; }
        public TaskCompletionSource<Tuple<A, T>> Completion { get; protected set; }
        public TaskCompletionSource<Object> StandardCompletion
        {
            get
            {
                return Completion as TaskCompletionSource<Object>;
            }
        }

        private Behaviors fLinkedBehaviors;

        public BaseActor LinkedActor
        {
            get
            {
                return fLinkedBehaviors.LinkedActor;
            }
        }

        public Behaviors LinkedTo
        {
            get
            {
                return fLinkedBehaviors;
            }
        }

        public void LinkBehaviors(Behaviors someBehaviors)
        {
            fLinkedBehaviors = someBehaviors;
        }

        public Behavior(Func<A, T, Boolean> aPattern, Action<A, T> anApply)
        {
            Pattern = aPattern;
            Apply = anApply;
            Completion = null;
        }

        public Behavior(Func<A, T, Boolean> aPattern, TaskCompletionSource<Tuple<A, T>> aCompletion)
        {
            Pattern = aPattern;
            Apply = null;
            Completion = aCompletion;
        }

        public Behavior()
        {
        }

        public Func<T, Boolean> DefaultPattern()
        {
            return t => { return t is T; };
        }

        public Behavior(Action<A, T> anApply)
        {
            Pattern = (a, t) => { return a is A && t is T; };
            Apply = anApply;
            Completion = null;
        }

        public Boolean StandardPattern(object aT)
        {
            if (Pattern == null)
                return false;
            Tuple<A, T> tupleT = aT as Tuple<A, T>;
            if (tupleT != null)
                return Pattern(tupleT.Item1, tupleT.Item2);
            else return false;
        }

        public void StandardApply(object aT)
        {
            if (Apply != null)
            {
                Tuple<A, T> tupleT = (Tuple<A, T>)aT;
                Apply(tupleT.Item1, tupleT.Item2);
            }
        }
    }

    public class Behavior<O, D, A> : IBehavior<O, D, A>, IBehavior
    {
        public Func<O, D, A, bool> Pattern { get; protected set; }
        public Action<O, D, A> Apply { get; protected set; }
        public TaskCompletionSource<Tuple<O, D, A>> Completion { get; protected set; }
        public TaskCompletionSource<object> StandardCompletion
        {
            get
            {
                return Completion as TaskCompletionSource<object>;
            }
        }

        private Behaviors fLinkedBehaviors;

        public BaseActor LinkedActor
        {
            get
            {
                return fLinkedBehaviors.LinkedActor;
            }
        }

        public Behaviors LinkedTo
        {
            get
            {
                return fLinkedBehaviors;
            }
        }

        public void LinkBehaviors(Behaviors someBehaviors)
        {
            fLinkedBehaviors = someBehaviors;
        }

        public Behavior(Func<O, D, A, bool> aPattern, Action<O, D, A> anApply)
        {
            Pattern = aPattern;
            Apply = anApply;
            Completion = null;
        }

        public Behavior(Func<O, D, A, bool> aPattern, TaskCompletionSource<Tuple<O, D, A>> aCompletion)
        {
            Pattern = aPattern;
            Apply = null;
            Completion = aCompletion;
        }

        public Behavior()
        {
        }

        public Func<O, Boolean> DefaultPattern()
        {
            return t => { return t is O; };
        }

        public Behavior(Action<O, D, A> anApply)
        {
            Pattern = (o, d, a) => { return o is O && d is D && a is A; };
            Apply = anApply;
            Completion = null;
        }

        public Boolean StandardPattern(Object aT)
        {
            if (Pattern == null)
                return false;
            Tuple<O, D, A> tupleT = aT as Tuple<O, D, A>;
            if (tupleT != null)
                return Pattern(tupleT.Item1, tupleT.Item2, tupleT.Item3);
            else return false;
        }

        public void StandardApply(Object aT)
        {
            if (Apply != null)
            {
                Tuple<O, D, A> tupleT = (Tuple<O, D, A>)aT;
                Apply(tupleT.Item1, tupleT.Item2, tupleT.Item3);
            }
        }
    }

    /// <summary>
    /// bhvBehavior
    /// A behavior is describe with two properties : Pattern and Apply.
    /// At message reception, it's tested against each Pattern and if it succeeded, 
    /// the Apply is invoke with this message as parameter.
    /// Patterns order can be relevant in this process.
    /// Type is the acq of the message to be send to this behavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Behavior<T> : IBehavior<T>, IBehavior
    {
        public Func<T, bool> Pattern { get; protected set; }
        public Action<T> Apply { get; protected set; }
        public TaskCompletionSource<T> Completion { get; protected set; }

        public TaskCompletionSource<object> StandardCompletion
        {
            get
            {
                return Completion as TaskCompletionSource<object>;
            }
        }

        private Behaviors fLinkedBehaviors;

        public BaseActor LinkedActor
        {
            get
            {
                return fLinkedBehaviors.LinkedActor;
            }
        }

        public Behaviors LinkedTo
        {
            get
            {
                return fLinkedBehaviors;
            }
        }

        public void LinkBehaviors(Behaviors someBehaviors)
        {
            fLinkedBehaviors = someBehaviors;
        }

        public Behavior(Func<T, bool> aPattern, Action<T> anApply)
        {
            Pattern = aPattern;
            Apply = anApply;
            Completion = null;
        }

        public Behavior(Func<T, bool> aPattern, TaskCompletionSource<T> aCompletion)
        {
            Pattern = aPattern;
            Apply = null;
            Completion = aCompletion;
        }

        public Behavior()
        {
        }

        public Func<T, bool> DefaultPattern()
        {
            return t => { return t is T; };
        }

        public Behavior(Action<T> anApply)
        {
            Pattern = t => { return t is T; };
            Apply = anApply;
            Completion = null;
        }

        public Boolean StandardPattern(object aT)
        {
            if (Pattern == null)
                return false;
            if (aT is T)
                return Pattern((T)aT);
            else return false;
        }

        public void StandardApply(object aT)
        {
            if (Apply != null)
            {
                Apply((T)aT);
            }
        }
    }
}

