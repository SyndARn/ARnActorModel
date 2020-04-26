#region license
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
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Actor.Base
{
    /// <summary>
    /// Behaviors holds many behaviors.
    /// Behaviors are actor brain.
    /// Behaviors when null in an actor means this actor is dead (it can't change anymore his own behavior from this point)
    /// </summary>
    public class Behaviors : IBehaviors
    {
        private readonly List<IBehavior> _behaviors = new List<IBehavior>();

        public IActor LinkedActor { get; private set; }

        public Behaviors()
        {
        }

        public Behaviors(IEnumerable<IBehavior> manyBehaviors)
        {
            CheckArg.BehaviorEnumerable(manyBehaviors);
            foreach (IBehavior item in manyBehaviors)
            {
                AddBehavior(item);
            }
        }

        public Behaviors(IBehavior[] someBehaviors)
        {
            CheckArg.BehaviorParam(someBehaviors);
            foreach (IBehavior item in someBehaviors)
            {
                AddBehavior(item);
            }
        }

        public IEnumerable<IBehavior> AllBehaviors() => _behaviors.ToList(); // force clone

        public virtual void LinkToActor(IActor anActor) => LinkedActor = anActor;

        public bool FindBehavior(IBehavior aBehavior) => _behaviors.Contains(aBehavior);

        public IBehaviors AddBehavior(IBehavior aBehavior)
        {
            CheckArg.Behavior(aBehavior);
            aBehavior.LinkBehaviors(this);
            _behaviors.Add(aBehavior);
            return this;
        }

        public IBehaviors BecomeBehavior(IBehavior aBehavior)
        {
            CheckArg.Behavior(aBehavior);
            _behaviors.Clear();
            aBehavior.LinkBehaviors(this);
            _behaviors.Add(aBehavior);
            return this;
        }

        public IBehaviors RemoveBehavior(IBehavior aBehavior)
        {
            CheckArg.Behavior(aBehavior);
            aBehavior.LinkBehaviors(null);
            _behaviors.Remove(aBehavior);
            return this;
        }
    }

    public class Behavior : IBehavior
    {
        public IBehaviors LinkedBehaviors { get; set; }

        public void LinkBehaviors(IBehaviors someBehaviors) => LinkedBehaviors = someBehaviors;

        public IActor LinkedActor => LinkedBehaviors?.LinkedActor;

        public Behavior(Func<object, bool> aPattern, Action<object> anApply)
        {
            Pattern = aPattern;
            Apply = anApply;
            Completion = null;
        }

        public Behavior(Func<object, bool> aPattern, TaskCompletionSource<object> aCompletion)
        {
            Pattern = aPattern;
            Apply = null;
            Completion = aCompletion;
        }

        public IBehaviors LinkedTo => LinkedBehaviors;

        public Func<object, bool> Pattern { get; protected set; }
        public Action<object> Apply { get; protected set; }
        public TaskCompletionSource<object> Completion { get; protected set; }

        public TaskCompletionSource<object> AwaitingPattern => Completion;

        public void StandardApply(object aT) => Apply?.Invoke(aT);

        public bool StandardPattern(object aT)
        {
            if (Pattern == null)
            {
                return false;
            }
            return Pattern(aT);
        }
    }

    public class Behavior<T1, T2> : IBehavior<T1, T2>, IBehavior
    {
        public Func<T1, T2, bool> Pattern { get; protected set; }
        public Action<T1, T2> Apply { get; protected set; }
        public TaskCompletionSource<IMessageParam<T1, T2>> Completion { get; protected set; }

        public TaskCompletionSource<object> AwaitingPattern => Completion as TaskCompletionSource<object>;

        public IBehaviors LinkedBehaviors { get; set; }

        public IActor LinkedActor => LinkedBehaviors.LinkedActor;

        public IBehaviors LinkedTo => LinkedBehaviors;

        public void LinkBehaviors(IBehaviors someBehaviors) => LinkedBehaviors = someBehaviors;

        public Behavior(Func<T1, T2, bool> aPattern, Action<T1, T2> anApply)
        {
            Pattern = aPattern;
            Apply = anApply;
            Completion = null;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public Behavior(Func<T1, T2, bool> aPattern, TaskCompletionSource<IMessageParam<T1, T2>> aCompletion)
        {
            Pattern = aPattern;
            Apply = null;
            Completion = aCompletion;
        }

        public Behavior()
        {
        }

        public Func<T1, T2, bool> DefaultPattern() => (t1, t2) => t1 is T1 && t2 is T2;

        public Behavior(Action<T1, T2> anApply)
        {
            Pattern = (t1, t2) => t1 is T1 && t2 is T2;
            Apply = anApply;
            Completion = null;
        }

        public bool StandardPattern(object aT)
        {
            if (Pattern == null)
            {
                return false;
            }

            return !(aT is IMessageParam<T1, T2> MessageParamT) ? false : Pattern(MessageParamT.Item1, MessageParamT.Item2);
        }

        public void StandardApply(object aT)
        {
            if (Apply == null)
            {
                return;
            }

            IMessageParam<T1, T2> MessageParamT = (IMessageParam<T1, T2>)aT;
#pragma warning disable CA1062 // Valider les arguments de méthodes publiques
            Apply(MessageParamT.Item1, MessageParamT.Item2);
#pragma warning restore CA1062 // Valider les arguments de méthodes publiques
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1005:AvoidExcessiveParametersOnGenericTypes")]
    public class Behavior<T1, T2, T3> : IBehavior<T1, T2, T3>, IBehavior
    {
        public Func<T1, T2, T3, bool> Pattern { get; protected set; }
        public Action<T1, T2, T3> Apply { get; protected set; }
        public TaskCompletionSource<IMessageParam<T1, T2, T3>> Completion { get; protected set; }

        public TaskCompletionSource<object> AwaitingPattern => Completion as TaskCompletionSource<object>;

        public IBehaviors LinkedBehaviors { get; set; }

        public IActor LinkedActor => LinkedBehaviors.LinkedActor;

        public IBehaviors LinkedTo => LinkedBehaviors;

        public void LinkBehaviors(IBehaviors someBehaviors) => LinkedBehaviors = someBehaviors;

        public Behavior(Func<T1, T2, T3, bool> aPattern, Action<T1, T2, T3> anApply)
        {
            Pattern = aPattern;
            Apply = anApply;
            Completion = null;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public Behavior(Func<T1, T2, T3, bool> aPattern, TaskCompletionSource<IMessageParam<T1, T2, T3>> aCompletion)
        {
            Pattern = aPattern;
            Apply = null;
            Completion = aCompletion;
        }

        public Behavior()
        {
        }

        public Func<T1, T2, T3, bool> DefaultPattern() => (t1, t2, t3) => t1 is T1 && t2 is T2 && t3 is T3;

        public Behavior(Action<T1, T2, T3> anApply)
        {
            Pattern = (o, d, a) => o is T1 && d is T2 && a is T3;
            Apply = anApply;
            Completion = null;
        }

        public bool StandardPattern(object aT)
        {
            if (Pattern == null)
            {
                return false;
            }

            return aT is IMessageParam<T1, T2, T3> MessageParamT ? Pattern(MessageParamT.Item1, MessageParamT.Item2, MessageParamT.Item3) : false;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1062:Valider les arguments de méthodes publiques", Justification = "<En attente>")]
        public void StandardApply(object aT)
        {
            if (Apply != null)
            {
                IMessageParam<T1, T2, T3> MessageParamT = (IMessageParam<T1, T2, T3>)aT;
                Apply(MessageParamT.Item1, MessageParamT.Item2, MessageParamT.Item3);
            }
        }
    }

    /// <summary>
    /// Behavior
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

        public TaskCompletionSource<object> AwaitingPattern => Completion as TaskCompletionSource<object>;

        public IBehaviors LinkedBehaviors { get; set; }

        public IActor LinkedActor => LinkedBehaviors?.LinkedActor;

        public IBehaviors LinkedTo => LinkedBehaviors;

        public void LinkBehaviors(IBehaviors someBehaviors) => LinkedBehaviors = someBehaviors;

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

        public Func<T, bool> DefaultPattern() => t => t is T;

        public Behavior(Action<T> anApply)
        {
            Pattern = DefaultPattern();
            Apply = anApply;
            Completion = null;
        }

        public bool StandardPattern(object aT)
        {
            if (Pattern == null)
            {
                return false;
            }
            if (aT is T t)
            {
                return Pattern(t);
            }
            return false;
        }

        public void StandardApply(object aT) => Apply?.Invoke((T)aT);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1005:AvoidExcessiveParametersOnGenericTypes")]
    public class Behavior<T1, T2, T3, T4> : IBehavior<T1, T2, T3, T4>, IBehavior
    {
        public Func<T1, T2, T3, T4, bool> Pattern { get; protected set; }
        public Action<T1, T2, T3, T4> Apply { get; protected set; }
        public TaskCompletionSource<IMessageParam<T1, T2, T3, T4>> Completion { get; protected set; }

        public TaskCompletionSource<object> AwaitingPattern => Completion as TaskCompletionSource<object>;

        public IBehaviors LinkedBehaviors { get; set; }

        public IActor LinkedActor => LinkedBehaviors.LinkedActor;

        public IBehaviors LinkedTo => LinkedBehaviors;

        public void LinkBehaviors(IBehaviors someBehaviors) => LinkedBehaviors = someBehaviors;

        public Behavior(Func<T1, T2, T3, T4, bool> aPattern, Action<T1, T2, T3, T4> anApply)
        {
            Pattern = aPattern;
            Apply = anApply;
            Completion = null;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public Behavior(Func<T1, T2, T3, T4, bool> aPattern, TaskCompletionSource<IMessageParam<T1, T2, T3, T4>> aCompletion)
        {
            Pattern = aPattern;
            Apply = null;
            Completion = aCompletion;
        }

        public Behavior()
        {
        }

        public Func<T1, T2, T3, T4, bool> DefaultPattern() => (o, d, a, r) => o is T1 && d is T2 && a is T3 && r is T4;

        public Behavior(Action<T1, T2, T3, T4> anApply)
        {
            Pattern = DefaultPattern();
            Apply = anApply;
            Completion = null;
        }

        public bool StandardPattern(object aT)
        {
            if (Pattern == null)
            {
                return false;
            }
            if (aT is IMessageParam<T1, T2, T3, T4> MessageParamT)
            {
                return Pattern(MessageParamT.Item1, MessageParamT.Item2, MessageParamT.Item3, MessageParamT.Item4);
            }

            return false;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1062:Valider les arguments de méthodes publiques", Justification = "<En attente>")]
        public void StandardApply(Object aT)
        {
            if (Apply != null)
            {
                IMessageParam<T1, T2, T3, T4> MessageParamT = (IMessageParam<T1, T2, T3, T4>)aT;
                Apply(MessageParamT.Item1, MessageParamT.Item2, MessageParamT.Item3, MessageParamT.Item4);
            }
        }
    }
}

