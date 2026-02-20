using System.Linq.Expressions;

namespace FGS.Common.SearchFilter;

public class PredicateBuilder<TEntity>
{
    private Expression<Func<TEntity, bool>>? _lambda;

    private class ReplaceVisitor(Expression from, Expression to) : ExpressionVisitor
    {
        public static Expression Replace(Expression expression,
            Expression searchEx, Expression replaceEx) => new ReplaceVisitor(searchEx, replaceEx).Visit(expression);

        public override Expression Visit(Expression? node) =>
            (node == from ? to : base.Visit(node)) ?? throw new InvalidOperationException();
    }

    public PredicateBuilder<TEntity> And(Expression<Func<TEntity, bool>> lambda)
    {
        if (_lambda == null)
        {
            _lambda = lambda;
            return this;
        }

        var addBody = ReplaceVisitor.Replace(lambda.Body, lambda.Parameters[0], _lambda!.Parameters[0]);
        var body = Expression.AndAlso(_lambda.Body, addBody);
        _lambda = Expression.Lambda<Func<TEntity, bool>>(body, _lambda.Parameters[0]);
        return this;
    }

    public PredicateBuilder<TEntity> Or(Expression<Func<TEntity, bool>> lambda)
    {
        if (_lambda == null)
        {
            _lambda = lambda;
            return this;
        }

        var addBody = ReplaceVisitor.Replace(lambda.Body, lambda.Parameters[0], _lambda!.Parameters[0]);
        var body = Expression.OrElse(_lambda.Body, addBody);
        _lambda = Expression.Lambda<Func<TEntity, bool>>(body, _lambda.Parameters[0]);
        return this;
    }

    public Expression<Func<TEntity, bool>> GetExpression(bool @default = true)
        => _lambda ??
           Expression.Lambda<Func<TEntity, bool>>(
               Expression.Constant(@default),
               Expression.Parameter(
                   typeof(TEntity)));
}