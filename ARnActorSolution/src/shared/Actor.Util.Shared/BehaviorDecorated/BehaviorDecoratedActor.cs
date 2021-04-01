using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Actor.Base;

namespace Actor.Util
{
#if !(NETFX_CORE) 
    public class BehaviorDecoratedActor : BaseActor
    {
        private const string MessageCantUseDecoratedActporOnNullMessage = "Can't use Decorated Actor on null message";

        public BehaviorDecoratedActor() : base()
        {
            // start with empty behaviors
            // reflexion on current attribute Behavior
            // add this to behaviors
            BuildFromAttributes();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Ne pas passer de littéraux en paramètres localisés", Justification = "<En attente>")]
        private void BuildFromAttributes()
        {
            Behaviors bhvs = new Behaviors();
            // Launch reflexion
            MemberInfo[] memberInfo = GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var mi in memberInfo)
            {
#if NETCOREAPP1_1
                BehaviorAttribute deco = mi.GetCustomAttribute<BehaviorAttribute>();
#else
                BehaviorAttribute deco = (BehaviorAttribute)Attribute.GetCustomAttribute(mi, typeof(BehaviorAttribute));
#endif
                if (deco != null)
                {
                    var parameters = ((MethodInfo)mi).GetParameters();
                    switch (parameters.Length)
                    {
                        case 0:
                            {
                                throw new ActorException(MessageCantUseDecoratedActporOnNullMessage);
                            }
                        case 1:
                            {
                                Behavior bhv = new Behavior(
                                   s => parameters[0].ParameterType.IsAssignableFrom(s.GetType()),
                                   s => ((MethodInfo)mi).Invoke(this, new[] { s }));
                                bhvs.AddBehavior(bhv);
                                break;
                            }
                        case 2:
                            {
                                Behavior bhv = new Behavior(
                                   s =>
                                   {
                                       var ts = s.GetType();
                                       return ts.Name == typeof(MessageParam<,>).Name;
                                   },
                                   s =>
                                   {
                                       var ts = s.GetType();
                                       var arg1 = ts.GetProperty("Item1").GetValue(s);
                                       var arg2 = ts.GetProperty("Item2").GetValue(s);
                                       ((MethodInfo)mi).Invoke(this, new[] { arg1, arg2 });
                                   });
                                bhvs.AddBehavior(bhv);
                                break;
                            }
                        case 3:
                            {
                                Behavior bhv = new Behavior(
                                   s =>
                                   {
                                       var ts = s.GetType();
                                       return ts.Name == typeof(MessageParam<,,>).Name;
                                   },
                                   s =>
                                   {
                                       var ts = s.GetType();
                                       var arg1 = ts.GetProperty("Item1").GetValue(s);
                                       var arg2 = ts.GetProperty("Item2").GetValue(s);
                                       var arg3 = ts.GetProperty("Item3").GetValue(s);
                                       ((MethodInfo)mi).Invoke(this, new[] { arg1, arg2, arg3 });
                                   });
                                bhvs.AddBehavior(bhv);
                                break;
                            }
                        default:
                            {
                                throw new ActorException("Can't use Decorated Actor on too much arguments");
                            }
                    }
                }
            }
            Become(bhvs);
        }
    }

    public static class BehaviorAttributeBuilder 
    {
        private const string MessageNullMessageOnDecoratedActor = "Can't use Decorated Actor on null message";
        private const string MessageTooMuchArgumentsOnDecoratedActor = "Can't use Decorated Actor on too much arguments";

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Ne pas passer de littéraux en paramètres localisés", Justification = "<En attente>")]
        public static IEnumerable<IBehavior> BuildFromAttributes(IActor linkedActor)
        {
            CheckArg.Actor(linkedActor);
            var bhvs = new List<IBehavior>();
            // Launch reflexion
            MemberInfo[] memberInfo = linkedActor.GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var mi in memberInfo)
            {
#if NETCOREAPP1_1
                BehaviorAttribute deco = mi.GetCustomAttribute<BehaviorAttribute>();
#else
                BehaviorAttribute deco = (BehaviorAttribute)Attribute.GetCustomAttribute(mi, typeof(BehaviorAttribute));
#endif
                if (deco != null)
                {
                    var parameters = ((MethodInfo)mi).GetParameters();
                    switch (parameters.Length)
                    {
                        case 0:
                            {
                                throw new ActorException(MessageNullMessageOnDecoratedActor);
                            }
                        case 1:
                            {
                                Behavior bhv = new Behavior(
                                   s => parameters[0].ParameterType.IsAssignableFrom(s.GetType()),
                                   s => ((MethodInfo)mi).Invoke(linkedActor, new[] { s }));
                                bhvs.Add(bhv);
                                break;
                            }
                        case 2:
                            {
                                Behavior bhv = new Behavior(
                                   s =>
                                   {
                                       return s.GetType().Name == typeof(MessageParam<,>).Name;
                                   },
                                   s =>
                                   {
                                       var ts = s.GetType();
                                       var arg1 = ts.GetProperty("Item1").GetValue(s);
                                       var arg2 = ts.GetProperty("Item2").GetValue(s);
                                       ((MethodInfo)mi).Invoke(linkedActor, new[] { arg1, arg2 });
                                   });
                                bhvs.Add(bhv);
                                break;
                            }
                        case 3:
                            {
                                Behavior bhv = new Behavior(
                                   s =>
                                   {
                                       return s.GetType().Name == typeof(MessageParam<,,>).Name;
                                   },
                                   s =>
                                   {
                                       var ts = s.GetType();
                                       var arg1 = ts.GetProperty("Item1").GetValue(s);
                                       var arg2 = ts.GetProperty("Item2").GetValue(s);
                                       var arg3 = ts.GetProperty("Item3").GetValue(s);
                                       ((MethodInfo)mi).Invoke(linkedActor, new[] { arg1, arg2, arg3 });
                                   });
                                bhvs.Add(bhv);
                                break;
                            }
                        default:
                            {
                                throw new ActorException(MessageTooMuchArgumentsOnDecoratedActor);
                            }
                    }
                }
            }
            return bhvs;
        }
    }
#endif

#if !(NETFX_CORE) 
    public static class ActionBehaviorAttributeBuilder
    {
        private const string MessageNullMessageOnDecoratedActor = "Can't use Decorated Actor on null message";
        private const string MessageTooMuchArgumentsOnDecoratedActor = "Can't use Decorated Actor on too much arguments";

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Ne pas passer de littéraux en paramètres localisés", Justification = "<En attente>")]
        public static IEnumerable<IBehavior> BuildFromAttributes(IActor linkedActor)
        {
            CheckArg.Actor(linkedActor);
            var bhvs = new List<IBehavior>();
            // Launch reflexion
            MemberInfo[] memberInfo = linkedActor.GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var mi in memberInfo)
            {
#if NETCOREAPP1_1
                ActionBehaviorAttribute deco = mi.GetCustomAttribute<ActionBehaviorAttribute>();
#else
                ActionBehaviorAttribute deco = (ActionBehaviorAttribute)Attribute.GetCustomAttribute(mi, typeof(ActionBehaviorAttribute));
#endif
                if (deco != null)
                {
                    var parameters = ((MethodInfo)mi).GetParameters();
                    switch (parameters.Length)
                    {
                        case 0:
                            {
                                IBehavior bhv = new ActionBehavior(
                                    (a) => a.GetType() == mi.GetType(),
                                    (a) => a()
                                    ) ;
                                bhvs.Add(bhv);
                                break;
                            }
                        case 1:
                            {
                                var genericArg = parameters[0].ParameterType;
                                var genericType = typeof(ActionBehavior<>).MakeGenericType(new Type[] { genericArg });
                                ConstructorInfo ci = genericType.GetConstructor(Type.EmptyTypes);
                                object o = ci.Invoke(new object[] { });
                                bhvs.Add(o as IBehavior);
                                break;
                            }
                        case 2:
                            {
                                IBehavior bhv = new ActionBehavior<object, object>(
                                    (a, o1, o2) => a.GetType() == mi.GetType(),
                                    (a, o1, o2) => a(o1,o2)
                                    );
                                bhvs.Add(bhv);
                                break;
                            }
                        case 3:
                            {
                                Behavior bhv = new Behavior(
                                   s =>
                                   {
                                       return s.GetType().Name == typeof(MessageParam<,,>).Name;
                                   },
                                   s =>
                                   {
                                       var ts = s.GetType();
                                       var arg1 = ts.GetProperty("Item1").GetValue(s);
                                       var arg2 = ts.GetProperty("Item2").GetValue(s);
                                       var arg3 = ts.GetProperty("Item3").GetValue(s);
                                       ((MethodInfo)mi).Invoke(linkedActor, new[] { arg1, arg2, arg3 });
                                   });
                                bhvs.Add(bhv);
                                break;
                            }
                        default:
                            {
                                throw new ActorException(MessageTooMuchArgumentsOnDecoratedActor);
                            }
                    }
                }
            }
            return bhvs;
        }
    }
#endif
}