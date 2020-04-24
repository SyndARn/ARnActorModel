using System;
using System.Reflection;
using Actor.Base;

namespace Actor.Util
{
#if !(NETFX_CORE) 
    public class BehaviorDecoratedActor : BaseActor
    {
        public BehaviorDecoratedActor() : base()
        {
            // start with empty behaviors
            // reflexion on current attribute Behavior
            // add this to behaviors
            BuildFromAttributes();
        }

        private void BuildFromAttributes()
        {
            Behaviors bhvs = new Behaviors();
            // Launch reflexion
            MemberInfo[] memberInfo = GetType().GetMethods();
            foreach (var mi in memberInfo)
            {
#if NETCOREAPP1_1
                BehaviorAttribute deco = (BehaviorAttribute)mi.GetType().GetTypeInfo().GetCustomAttribute(typeof(BehaviorAttribute));
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
                                throw new ActorException("Can't use Decorated Actor on null message");
                            }
                        case 1:
                            {
                                Behavior bhv = new Behavior(
                                   s => parameters[0].ParameterType == s.GetType(),
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
                                       var mp = typeof(MessageParam<,,>);
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
#endif
}