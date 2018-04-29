using Actor.Base;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Actor.Util
{
    // proxy generator
#if !(NETFX_CORE) && !(NETCOREAPP1_1) && !(NETCOREAPP2_0) && !(NETSTANDARD2_0)
    public static class ActorProxyGenerator<T, I>
    where T : class // real object
        where I : class // interface ...
{
    public static I GenerateFacade(T aT)
    {
        // create facadeT object
        var createdType = CreateInterfaceType();
        I instanceI = (I)Activator.CreateInstance(createdType);
        // create actor for proxy
        IActor actor = CreateActorFor(aT);
        // associate instance and actor
        instanceI.GetType().GetField("actorRef").SetValue(instanceI, actor);
        return instanceI;
    }

    private static Type CreateInterfaceType()
    {
        Type type = typeof(I);
        // Get the current application domain for the current thread.
        AppDomain myCurrentDomain = AppDomain.CurrentDomain;
            AssemblyName myAssemblyName = new AssemblyName()
            {
                Name = "TempAssembly"
            };

            // Define a dynamic assembly in the current application domain.
            AssemblyBuilder myAssemblyBuilder = myCurrentDomain.DefineDynamicAssembly(myAssemblyName, AssemblyBuilderAccess.Run);

        // Define a dynamic module in this assembly.
        ModuleBuilder myModuleBuilder = myAssemblyBuilder.DefineDynamicModule("TempModule");

        // Define a runtime class with specified name and attributes.
        TypeBuilder typeBuilder = myModuleBuilder.DefineType("ImplOf" + type.Name, TypeAttributes.Class | TypeAttributes.Public);
        typeBuilder.AddInterfaceImplementation(type);

        // Create Constructor
        ConstructorInfo baseConstructorInfo = typeof(object).GetConstructor(new Type[0]);
        ConstructorBuilder constructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, Type.EmptyTypes);
        ILGenerator ilGenerator = constructorBuilder.GetILGenerator();
        ilGenerator.Emit(OpCodes.Ldarg_0);                      // Load "this"
        ilGenerator.Emit(OpCodes.Call, baseConstructorInfo);    // Call the base constructor
        ilGenerator.Emit(OpCodes.Ret);                          // return

        // actor field
        var actorField = typeBuilder.DefineField("actorRef", typeof(IActor), FieldAttributes.Public);

        // what methods ?
        List<MethodInfo> methods = new List<MethodInfo>();
        AddMethodsToList(methods, type);

        // properties ?
        List<PropertyInfo> properties = new List<PropertyInfo>();
        AddPropertiesToList(properties, type);

        // add properties
        foreach (PropertyInfo pi in properties)
        {
            string piName = pi.Name;
            Type propertyType = pi.PropertyType;

            // Create underlying field; all properties have a field of the same type
            FieldBuilder field =
                typeBuilder.DefineField("_" + piName, propertyType, FieldAttributes.Private);

            // If there is a getter in the interface, create a getter in the new type
            MethodInfo getMethod = pi.GetGetMethod();
            if (null != getMethod)
            {
                // This will prevent us from creating a default method for the property's getter
                methods.Remove(getMethod);

                // Now we will generate the getter method
                MethodBuilder methodBuilder = typeBuilder.DefineMethod(getMethod.Name,
                    MethodAttributes.Public | MethodAttributes.Virtual, propertyType,
                    Type.EmptyTypes);

                // The ILGenerator class is used to put op-codes (similar to assembly) 
                // into the method
                ilGenerator = methodBuilder.GetILGenerator();

                // These are the op-codes, (similar to assembly)
                ilGenerator.Emit(OpCodes.Ldarg_0);      // Load "this"
                                                        // Load the property's underlying field onto the stack
                ilGenerator.Emit(OpCodes.Ldfld, field);
                ilGenerator.Emit(OpCodes.Ret);          // Return the value on the stack

                // We need to associate our new type's method with the 
                // getter method in the interface
                typeBuilder.DefineMethodOverride(methodBuilder, getMethod);
            }

            // If there is a setter in the interface, create a setter in the new type
            MethodInfo setMethod = pi.GetSetMethod();
            if (null != setMethod)
            {
                // This will prevent us from creating a default method for the property's setter
                methods.Remove(setMethod);

                // Now we will generate the setter method
                MethodBuilder methodBuilder = typeBuilder.DefineMethod
                    (setMethod.Name, MethodAttributes.Public |
                    MethodAttributes.Virtual, typeof(void), new Type[] { pi.PropertyType });

                // The ILGenerator class is used to put op-codes (similar to assembly) 
                // into the method
                ilGenerator = methodBuilder.GetILGenerator();

                // These are the op-codes, (similar to assembly)
                ilGenerator.Emit(OpCodes.Ldarg_0);      // Load "this"
                ilGenerator.Emit(OpCodes.Ldarg_1);      // Load "value" onto the stack
                                                        // Set the field equal to the "value" on the stack
                ilGenerator.Emit(OpCodes.Stfld, field);
                ilGenerator.Emit(OpCodes.Ret);          // Return nothing

                // We need to associate our new type's method with the 
                // setter method in the interface
                typeBuilder.DefineMethodOverride(methodBuilder, setMethod);
            }
        }

        // add method
        foreach (MethodInfo methodInfo in methods)
        {
            // Get the return type and argument types

            Type returnType = methodInfo.ReturnType;

            List<Type> argumentTypes = new List<Type>();
            foreach (ParameterInfo parameterInfo in methodInfo.GetParameters())
                argumentTypes.Add(parameterInfo.ParameterType);

            // Define the method
            MethodBuilder methodBuilder = typeBuilder.DefineMethod
                (methodInfo.Name, MethodAttributes.Public |
                MethodAttributes.Virtual, returnType, argumentTypes.ToArray());

            // The ILGenerator class is used to put op-codes
            // (similar to assembly) into the method
            ilGenerator = methodBuilder.GetILGenerator();

            // If there's a return type, create a default value or null to return
            if (returnType != typeof(void))
            {
                // send message with a future param
                // this declares the local object, int, long, float, etc.
                LocalBuilder localBuilder = ilGenerator.DeclareLocal(returnType);
                var constructorInfo = returnType.GetConstructor(Type.EmptyTypes);
                var localMethodinfo = typeof(ActorProxyModel<T>).GetMethod("SendMethodAndParam");
                // build local future
                ilGenerator.Emit(OpCodes.Newobj, constructorInfo);
                ilGenerator.Emit(OpCodes.Stloc_0);

                ilGenerator.Emit(OpCodes.Ldarg_0); // this
                ilGenerator.Emit(OpCodes.Ldfld, actorField); // actor ref
                ilGenerator.Emit(OpCodes.Ldstr, methodInfo.Name);
                ilGenerator.Emit(OpCodes.Ldloc_0); // future
                ilGenerator.Emit(OpCodes.Callvirt, localMethodinfo);

                // load the value on the stack to return
                ilGenerator.Emit(OpCodes.Ldloc, localBuilder);
                ilGenerator.Emit(OpCodes.Ret);                       // return                    
            }
            else
            {
                var localMethodinfo = typeof(ActorProxyModel<T>).GetMethod("SendMethodAndParam");
                ilGenerator.Emit(OpCodes.Ldarg_0); // this
                ilGenerator.Emit(OpCodes.Ldfld, actorField); // actor ref
                ilGenerator.Emit(OpCodes.Ldstr, methodInfo.Name);
                ilGenerator.Emit(OpCodes.Ldarg_1); // message
                ilGenerator.Emit(OpCodes.Callvirt, localMethodinfo);
                ilGenerator.Emit(OpCodes.Ret);
            }
            // We need to associate our new type's method with the method in the interface
            typeBuilder.DefineMethodOverride(methodBuilder, methodInfo);
        }

        Type createdType = typeBuilder.CreateType();
        // InterfaceImplementations[type] = createdType;
        return createdType;
    }

    private static IActor CreateActorFor(T aT)
    {
        // what methods ?
        List<MethodInfo> methods = new List<MethodInfo>();
        AddMethodsToList(methods, typeof(I));

        // add interface method for I

        // proxy to create
        ActorProxyModel<T> actor = new ActorProxyModel<T>();

        // method to behavior
        Behaviors behaviors = new Behaviors();
        foreach (MethodInfo methodInfo in methods)
        {
            // Get the return type and argument types
            Type returnType = methodInfo.ReturnType;

            List<Type> argumentTypes = new List<Type>();
            foreach (ParameterInfo parameterInfo in methodInfo.GetParameters())
                argumentTypes.Add(parameterInfo.ParameterType);

            // no return type => send message
            if (returnType != typeof(void))
            // return type => send to a future
            {
                // add behavior future message here
                behaviors.AddBehavior(new Behavior<string, Future<string>>((s, f) => s == methodInfo.Name,
                    (s, f) =>
                    {
                        var ret = methodInfo.Invoke(aT, Type.EmptyTypes);
                        f.SendMessage(ret);
                    }
                    ));
            }
            else
            {
                // add behavior send message here
                behaviors.AddBehavior(new Behavior<string, object>((s, t) => s == methodInfo.Name,
                    (s, t) => methodInfo.Invoke(aT, new object[] { t })));
            }
        }
        actor.AddBehaviors(behaviors);
        actor.SetObject(aT);
        return actor;
    }

    private static void AddMethodsToList(List<MethodInfo> methods, Type type)
    {
        methods.AddRange(type.GetMethods());

        foreach (Type subInterface in type.GetInterfaces())
            AddMethodsToList(methods, subInterface);
    }

    private static void AddPropertiesToList(List<PropertyInfo> properties, Type type)
    {
        properties.AddRange(type.GetProperties());

        foreach (Type subInterface in type.GetInterfaces())
            AddPropertiesToList(properties, subInterface);
    }
}
#endif
}
