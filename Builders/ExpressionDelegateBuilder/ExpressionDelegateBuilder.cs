using FlexNet.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace FlexNet.Builders.ExpressionDelegateBuilder
{
    /// <summary>
    /// an <see cref="IDelegateBuilder"/> implemented using <see cref="System.Linq.Expressions"/>
    /// </summary>
    public class ExpressionDelegateBuilder : IDelegateBuilder
    {
        public Func<Stream, Object> BuildReadDelegate(PacketDefinition definition, Dictionary<Type, INetworkAccessor> accessors)
        {
            var exprs = new List<Expression>();
            var vars = new List<ParameterExpression>();

            var instance = Expression.Variable(definition.Binding);
            vars.Add(instance);
            var stream = Expression.Parameter(typeof(Stream), "stream");

            exprs.Add(Expression.Assign(instance, Expression.New(definition.Binding)));

            var readMethod = typeof(INetworkAccessor).GetMethod(nameof(INetworkAccessor.Read));

            foreach (var binding in definition.Bindings)
            {
                if (binding is PropertyInfo propertyInfo)
                {
                    exprs.Add(Expression.Assign(
                        Expression.Property(instance, propertyInfo),
                        Expression.Convert(Expression.Call(Expression.Constant(accessors[propertyInfo.PropertyType]), readMethod, stream), propertyInfo.PropertyType)
                        ));
                }
                else if (binding is FieldInfo fieldInfo)
                {
                    exprs.Add(Expression.Assign(
                        Expression.Field(instance, fieldInfo),
                        Expression.Convert(Expression.Call(Expression.Constant(accessors[fieldInfo.FieldType]), readMethod, stream), fieldInfo.FieldType)
                        ));
                }
            }

            var retLabel = Expression.Label(typeof(Object));
            exprs.Add(Expression.Return(retLabel, Expression.Convert(instance, typeof(Object))));
            exprs.Add(Expression.Label(retLabel, Expression.Constant(null)));

            return Expression.Lambda<Func<Stream, Object>>(Expression.Block(vars, exprs), new ParameterExpression[] { stream }).Compile();
        }

        public Action<Stream, Object> BuildWriteDelegate(PacketDefinition definition, Dictionary<Type, INetworkAccessor> accessors)
        {
            var stream = Expression.Parameter(typeof(Stream), "stream");
            var instanceParam = Expression.Parameter(typeof(Object), "instanceParam");

            var exprs = new List<Expression>();
            var vars = new List<ParameterExpression>();
            var instance = Expression.Variable(definition.Binding, "instance");
            vars.Add(instance);

            exprs.Add(Expression.Assign(instance, Expression.Convert(instanceParam, definition.Binding)));

            var writeMethod = typeof(INetworkAccessor).GetMethod(nameof(INetworkAccessor.Write));

            foreach (var binding in definition.Bindings)
            {
                if (binding is PropertyInfo propertyInfo)
                {
                    exprs.Add(Expression.Call(Expression.Constant(accessors[propertyInfo.PropertyType]), writeMethod,
                        stream,
                        Expression.Convert(Expression.Property(instance, propertyInfo), typeof(Object))));
                }
                else if (binding is FieldInfo fieldInfo)
                {
                    exprs.Add(Expression.Call(Expression.Constant(accessors[fieldInfo.FieldType]), writeMethod,
                        stream,
                        Expression.Convert(Expression.Field(instance, fieldInfo), typeof(Object))));
                }
            }

            var retLabel = Expression.Label();
            exprs.Add(Expression.Return(retLabel));
            exprs.Add(Expression.Label(retLabel));

            return Expression.Lambda<Action<Stream, Object>>(Expression.Block(vars, exprs), new ParameterExpression[] { stream, instanceParam }).Compile();
        }
    }
}
