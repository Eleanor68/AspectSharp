using AspectSharp.Core.Language;
using System;
using System.Collections.Generic;

namespace AspectSharp.Core
{
    public enum InvokeOption
    {
        Safe,
        Unsafe,
        Default = Unsafe,
    }

    /// <summary>
    /// Specifies how advice signature matches joinpoint signature.
    /// </summary>
    public enum SignatureMatch
    {
        Auto,
        Strict,
        Compatible,
        Observe
    }

    public enum CaptureOption
    {
        Auto,
        Context,
        Target,
        Arguments,
        PointcutExp
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public abstract class PoitcutBase : Attribute
    {
        protected PoitcutBase()
        {
        }

        protected PoitcutBase(CaptureOption capture, SignatureMatch signature, InvokeOption invoke, byte order)
        {
            Capture = capture;
            Signature = signature;
            Invoke = invoke;
            Order = order;
        }


        public string PointcutRef { get; set; }

        public CaptureOption Capture { get; set; }

        public InvokeOption Invoke { get; set; }

        public SignatureMatch Signature { get; set; }

        public byte Order { get; set; }

    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class BeforeAttribute : PoitcutBase
    {
        public BeforeAttribute()
        {
        }

        public BeforeAttribute(string pointcutExpression, CaptureOption captureOption = CaptureOption.Auto, SignatureMatch signatureMatch = SignatureMatch.Auto, InvokeOption invokeOption = InvokeOption.Default, byte order = 0)
            : base(captureOption, signatureMatch, invokeOption, order)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class AfterAttribute : PoitcutBase
    {
        public AfterAttribute()
        {
        }

        public AfterAttribute(string pointcutExpression, CaptureOption captureOption = CaptureOption.Auto, SignatureMatch signatureMatch = SignatureMatch.Auto, InvokeOption invokeOption = InvokeOption.Default, byte order = 0)
            : base(captureOption, signatureMatch, invokeOption, order)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class AroundAttribute : Attribute
    {
        public AroundAttribute(string pointcutExpression)
        {
        }
    }

    public enum AspectInstanceMode
    {
        Auto,
        Instance,
        Static
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
    public sealed class AspectAttribute : Attribute
    {
        public AspectAttribute()
        {
        }

        public AspectInstanceMode InstanceMode { get; set; } = AspectInstanceMode.Auto;

        public string Factory { get; set; }

        /// <summary>
        /// ??
        /// </summary>
        //public InvokeOption InvokeOption { get; set; }
    }

    public abstract class AspectBase
    {

    }

    public interface IJoinPoint : IJoinPoint<IMemberDefinition>
    {
    }

    public interface IJoinPoint<TMember> where TMember : IMemberDefinition
    {
        TMember MemberDefinition { get; }
    }

    public interface IMethodJoinPoint : IJoinPoint<IMethodDefinition>
    {
        IReadOnlyCollection<IArgument> Arguments { get; } 
    }

    public class MethodJoinPoint : IMethodJoinPoint
    {
        public MethodJoinPoint(IMethodDefinition methodDefinition)
        {
            MemberDefinition = methodDefinition;
        }

        public IReadOnlyCollection<IArgument> Arguments => throw new NotImplementedException();

        public IMethodDefinition MemberDefinition { get; private set; }
    }

    public interface IArgument : IArgument<object>
    {
        Type Type { get; }
    }

    public interface IArgument<A>
    {
        string Name { get; }

        A Value { get; }

        A DefaultValue { get; }

        IReadOnlyCollection<Attribute> Attributes { get; }
    }

    public class Argument<A> : IArgument<A>
    {
        public string Name { get; set; }

        public A Value { get; set; }

        public A DefaultValue { get; set; }

        public IReadOnlyCollection<Attribute> Attributes { get; set; }
    }

    public interface IMemberDefinition
    {
        string Name { get; set; }

        string Namespace { get; set; }
        
        MemberDefinitionType MemberType { get; }
    }

    public interface IMethodDefinition : IMemberDefinition
    {
    }

    public class MethodDefinition : IMethodDefinition
    {
        public string Name { get; set; }

        public string Namespace { get; set; }

        public MemberDefinitionType MemberType => MemberDefinitionType.Method;
    }

    public enum MemberDefinitionType
    {
        Member,
        Constructor,
        Property,
        Method
    }

    public static class MemberDefinitionFactory
    {
        public static IMethodDefinition CreateMethod(string namespaceName, string typeName)
        {
            throw new NotImplementedException();
        }
    }

    public interface IPointcutExpressionMatch : IPointcutExpressionMatch<IMemberDefinition>
    {
        
    }

    public interface IPointcutExpressionMatch<M> where M : IMemberDefinition
    {
        bool IsMatch(M memberDefinition);
    }

    public class PointcutExpressionMatch : IPointcutExpressionMatch
    {
        public PointcutExpressionMatch(PointcutSyntax pointcutSyntax)
        {

        }

        public bool IsMatch(IMemberDefinition memberDefinition)
        {
            return false;
        }
    }

    public sealed class AndPointcutMatch : IPointcutExpressionMatch
    {
        private readonly IPointcutExpressionMatch left;
        private readonly IPointcutExpressionMatch right;

        public AndPointcutMatch(IPointcutExpressionMatch left, IPointcutExpressionMatch right)
        {
            this.left = left;
            this.right = right;
        }

        public bool IsMatch(IMemberDefinition memberDefinition) => left.IsMatch(memberDefinition) && right.IsMatch(memberDefinition);
    }

    public sealed class MethodPointcutMatch : IPointcutExpressionMatch<MethodDefinition>
    {
        private readonly MethodPointcutSyntax methodPointcutSyntax;

        public MethodPointcutMatch(MethodPointcutSyntax methodPointcutSyntax)
        {
            if (methodPointcutSyntax == null) throw new ArgumentNullException(nameof(methodPointcutSyntax));
            this.methodPointcutSyntax = methodPointcutSyntax;
        }

        public bool IsMatch(MethodDefinition methodDefinition)
        {
            if (methodDefinition == null) return false;

            

            return false;
        }
    }







}