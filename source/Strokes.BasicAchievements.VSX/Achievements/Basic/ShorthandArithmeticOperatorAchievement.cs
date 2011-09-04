﻿using System.Collections.Generic;
using System.Linq;
using ICSharpCode.NRefactory.Ast;
using Strokes.BasicAchievements.NRefactory;
using Strokes.Core;

namespace Strokes.BasicAchievements.Achievements
{
    [AchievementDescription("Use the ++ operator", AchievementDescription = "Write an expression that uses the ++ operator", AchievementCategory = "Fundamentals")]
    public class PlusPlusOperatorAchievement : NRefactoryAchievement
    {
        protected override AbstractAchievementVisitor CreateVisitor()
        {
            return new Visitor();
        }

        private class Visitor : AbstractAchievementVisitor
        {
            public override object VisitUnaryOperatorExpression(UnaryOperatorExpression unaryOperatorExpression, object data)
            {
                if (unaryOperatorExpression.Op == UnaryOperatorType.PostIncrement || unaryOperatorExpression.Op == UnaryOperatorType.Increment)
                    UnlockWith(unaryOperatorExpression);
                return base.VisitUnaryOperatorExpression(unaryOperatorExpression, data);
            }
        }
    }

    [AchievementDescription("Use the -- operator", AchievementDescription = "Write an expression that uses the -- operator", AchievementCategory = "Fundamentals")]
    public class MinusMinusOperatorAchievement : NRefactoryAchievement
    {
        protected override AbstractAchievementVisitor CreateVisitor()
        {
            return new Visitor();
        }

        private class Visitor : AbstractAchievementVisitor
        {
            public override object VisitUnaryOperatorExpression(UnaryOperatorExpression unaryOperatorExpression, object data)
            {
                if (unaryOperatorExpression.Op == UnaryOperatorType.PostDecrement || unaryOperatorExpression.Op == UnaryOperatorType.Decrement)
                    UnlockWith(unaryOperatorExpression);
                return base.VisitUnaryOperatorExpression(unaryOperatorExpression, data);
            }
        }
    }




}