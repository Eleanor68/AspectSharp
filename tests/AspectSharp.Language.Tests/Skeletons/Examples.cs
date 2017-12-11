using AspectSharp.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspectSharp.Language.Tests.Skeletons
{
    public sealed class UserModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public int Age { get; set; }
    }

    public sealed class UserController
    {
        public Guid Create(UserModel user)
        {
            throw new NotImplementedException();
        }

        public UserModel Get(Guid id)
        {
            throw new NotImplementedException();
        }

        public void Update(UserModel user)
        {
            throw new NotImplementedException();
        }

        public UserModel Delete(Guid id)
        {
            throw new NotImplementedException();
        }
    }

    namespace AspectAttributeExamples
    {
        /// <remarks>
        /// The aspect visibility is highly coupled to the target resolution strategy.
        /// </remarks>
        [Aspect(InstanceMode = AspectInstanceMode.Auto, Factory = nameof(Create))]
        public class LogAspect : AspectBase
        {
            public static LogAspect Create()
            {
                return new LogAspect();
            }
        }
    }

    namespace Struct
    {
        [Aspect]
        public struct LogAspect
        {
        }
    }

    namespace StaticClass
    {
        namespace BeforePointcut
        {
            [Aspect]
            public static class ArgumentNullReferenceAspect
            {
                [Before("public * UserController.*(..)", Invoke = InvokeOption.Unsafe, Capture = CaptureOption.Arguments)]
                public static void MethodContract(IMethodJoinPoint joinPoint)
                {
                    if (joinPoint.Arguments.Count == 0) return;

                    foreach (var arg in joinPoint.Arguments)
                    {
                        Check(arg);
                    }
                }

                [Before("public * UserController.*([NotNull]*)", Invoke = InvokeOption.Unsafe)]
                public static void MethodContract1(IArgument arg1)
                {
                    Check(arg1);
                }


                [Before("public * UserController.*([NotNull]*, [NotNull]*)", Invoke = InvokeOption.Unsafe)]
                public static void MethodContract2(IArgument arg1, IArgument arg2)
                {
                    Check(arg1);
                    Check(arg2);
                }

                [Before("public * UserController.*([NotNull]*, [NotNull]*, [NotNull]*)", Invoke = InvokeOption.Unsafe)]
                public static void MethodContract3(IArgument arg1, IArgument arg2, IArgument arg3)
                {
                    Check(arg1);
                    Check(arg2);
                    Check(arg2);
                }

                private static void Check(IArgument arg)
                {
                    if (arg == null)
                    {
                        throw arg.Type == typeof(string)
                            ? new ArgumentException(arg.Name)
                            : new ArgumentNullException(arg.Name);
                    }
                    else if (arg.Value is string && string.IsNullOrEmpty(arg.Value as string))
                    {
                        throw new ArgumentException(arg.Name);
                    }
                }
            }

            [Aspect]
            public static class LogAspect
            {
                [Before("!void UserController.*")]
                public static void Before()
                {

                }

                [Before("UserController.*", CaptureOption.Auto, SignatureMatch.Auto, InvokeOption.Unsafe, 0)]
                public static void Before(IJoinPoint joinPoint)
                {
                }

                [Before("UserController.*(..)")]
                public static void BeforeMethod(MethodJoinPoint method, object arg1 = null)
                {
                    var methodDef = method.MemberDefinition;
                }

                [Before("UserController.*(..)")]
                public static void BeforeMethod(object[] args)
                {
                }

                /// <remarks>
                /// Strict parameters match regardless pointcut expression
                /// </remarks>
                [Before("UserController.*(..)")]
                public static void BeforeMethod(object arg1, object arg2)
                {
                }

                [Before("UserController.*(..)")]
                public static void BeforeMethod(int value1)
                {
                }

                [Before("UserController.*(..)")]
                public static void BeforeMethod2(object arg1 = null, object arg2 = null)
                {
                }
            }
        }

        namespace BeforePointcutRef
        {
            [Aspect]
            public static class LogAspect
            {
                public static string allMembersFromUserController = "UserController.*";
                public static string allMethodsFromUserController = "UserController.*(..)";

                /// <summary>
                /// Observe signature match
                /// </summary>
                [Before(PointcutRef = nameof(allMembersFromUserController))]
                public static void BeforeByRef()
                {
                }

                /// <summary>
                /// Observer signature match
                /// </summary>
                [Before(PointcutRef = nameof(allMembersFromUserController))]
                public static void BeforeByRef(IJoinPoint joinPoint)
                {
                }

                /// <summary>
                /// Strict signature match
                /// </summary>
                /// <param name="userModel"></param>
                [Before(PointcutRef = nameof(allMethodsFromUserController))]
                public static void BeforeByRef(UserModel userModel)
                {
                }

                /// <summary>
                /// Strict signature match
                /// </summary>
                /// <param name="userModel"></param>
                [Before(PointcutRef = nameof(allMethodsFromUserController))]
                public static void BeforeByRef(IJoinPoint joinPoint, object userModel)
                {
                }

                /// <summary>
                /// Compatible signature match
                /// </summary>
                /// <param name="userModel"></param>
                [Before(PointcutRef = nameof(allMethodsFromUserController))]
                public static void BeforeByRefOptional(IJoinPoint joinPoint, object userModel = null)
                {
                }
            }
        }
    }
}
