﻿using System.Collections.Generic;
using System.Linq;
using ICSharpCode.NRefactory.Ast;
using Strokes.BasicAchievements.NRefactory;
using Strokes.Core;

namespace Strokes.BasicAchievements.Achievements
{
    [AchievementDescription("@ParamsParameterAchievementName",
        AchievementDescription = "@ParamsParameterAchievementDescription",
        AchievementCategory = "@Method")]
    public class ParamsParameterAchievement : NRefactoryAchievement
    {
        protected override AbstractAchievementVisitor CreateVisitor()
        {
            return new Visitor();
        }

        private class Visitor : AbstractAchievementVisitor
        {
            public override object VisitParameterDeclarationExpression(ParameterDeclarationExpression parameterDeclarationExpression, object data)
            {
                if (parameterDeclarationExpression.ParamModifier == ParameterModifiers.Params)
                {
                    UnlockWith(parameterDeclarationExpression);
                }
                return base.VisitParameterDeclarationExpression(parameterDeclarationExpression, data);
            }
        }
    }
}