using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ica.SalesOrders.Web.Extensions
{
    public static class IQueryableExtensions
    {
        public static Object GetPropValue(this Object obj, String propName)
        {
            try
            {
                object ret = obj;
                string[] nameParts = propName.Split('.');
                foreach (string property in nameParts)
                {
                    Type type = ret.GetType();
                    if (type.IsArray)
                    {
                        type = type.GetElementType();
                        PropertyInfo info = type.GetProperty(property);
                        if (info == null) {
                            throw new Exception("NO PROPERTY FOUND");
                        }
                        Array a = (Array)ret;
                        ret = info.GetValue(a.GetValue(0), null);
                    }
                    else
                    {
                        PropertyInfo info = type.GetProperty(property);
                        if (info == null) {
                            throw new Exception("NO PROPERTY FOUND");
                        }
                        ret = info.GetValue(ret, null);
                    }
                }
               

                return ret;
            }
            catch (Exception ex )
            {
                return "NO PROPERTY FOUND";
            }
        }

        public static IQueryable<T> Where<T>(this IQueryable<T> source, string columnName, string keyword)
        {
            var arg = Expression.Parameter(typeof(T), "p");

            Type propertyType = Expression.Property(arg, columnName).Type;

            object keywordValue = null;

            if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                if (propertyType.FullName.Contains("System.Int32"))
                {
                    int baseTypeValue = 0;
                    if (int.TryParse(keyword, out baseTypeValue))
                    {
                        int? baseTypeValueNull = baseTypeValue;
                        keywordValue = baseTypeValueNull;
                    }
                }
            }
            else
            {
                keywordValue = System.Convert.ChangeType(keyword, propertyType);
            }

            if (keywordValue == null)
            {
                return source;
            }
            
            String method = "Equals";
            if (propertyType == typeof(string))
            {
                //method = "StartsWith";
                method = "Contains";
            }

            MethodCallExpression body =   Expression.Call(
               Expression.Property(arg, columnName),
               method,
               null,
               Expression.Constant(keywordValue));
            
            
            var predicate = Expression.Lambda<Func<T, bool>>(body, arg);

            return source.Where(predicate);
        }

        public static IQueryable<T> OrderBy<T>(this IQueryable<T> source, string ordering, string direction)
        {
            if (string.IsNullOrWhiteSpace(ordering)) return source;
            if (string.IsNullOrWhiteSpace(direction)) return source;
            Type type = typeof(T);


            ParameterExpression parameterExpression = Expression.Parameter(type, "p");
            PropertyInfo property = type.GetProperty(ordering);


            MemberExpression memberExpression = null;
            string[] fieldNames = ordering.Split('.');
            foreach (string filed in fieldNames)
            {
                if (memberExpression == null)
                {
                    memberExpression = Expression.Property(parameterExpression, filed);
                }
                else
                {
                    memberExpression = Expression.Property(memberExpression, filed);
                }
            }


            ParameterExpression[] parameterExpressionArray = new ParameterExpression[] { parameterExpression };
            LambdaExpression lambdaExpression = Expression.Lambda(memberExpression, parameterExpressionArray);
            if (String.IsNullOrEmpty(direction))
            {
                direction = "asc";
            }
            string str = "OrderBy";
            if (!direction.Equals("asc", StringComparison.InvariantCultureIgnoreCase))
            {
                str = "OrderByDescending";
            }
            Type type1 = typeof(Queryable);
            Type[] propertyType = new Type[] { type, memberExpression.Type };
            Expression[] expression = new Expression[] { source.Expression, Expression.Quote(lambdaExpression) };
            MethodCallExpression methodCallExpression = Expression.Call(type1, str, propertyType, expression);
            return source.Provider.CreateQuery<T>(methodCallExpression);
        }


        public static IOrderedQueryable<T> OrderingHelper<T>(this IQueryable<T> source, string propertyName, bool descending, bool anotherLevel)
        {
            var param = Expression.Parameter(typeof(T), "p");
            var property = Expression.PropertyOrField(param, propertyName);
            var sort = Expression.Lambda(property, param);

            var call = Expression.Call(
                typeof(Queryable),
                (!anotherLevel ? "OrderBy" : "ThenBy") + (descending ? "Descending" : string.Empty),
                new[] { typeof(T), property.Type },
                source.Expression,
                Expression.Quote(sort));

            return (IOrderedQueryable<T>)source.Provider.CreateQuery<T>(call);
        }



    }
}