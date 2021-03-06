﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Strokes.Core;
using Strokes.BasicAchievements.NRefactory;
using System.Text.RegularExpressions;

namespace Strokes.BasicAchievements.Achievements
{
    [AchievementDescriptor("{EC805EF5-BAB8-43DB-B014-6A1ED350A9B9}", "@DeclareHexShorthandCharAchievementName",
        AchievementDescription = "@DeclareHexShorthandCharAchievementDescription",
        AchievementCategory = "@PrimitiveType")]
    public class DeclareHexShorthandCharAchievement : NRefactoryAchievement
    {
        protected override AbstractAchievementVisitor CreateVisitor(DetectionSession detectionSession)
        {
            return new Visitor();
        }

        private class Visitor : AbstractAchievementVisitor
        {

            public override object VisitPrimitiveExpression(ICSharpCode.NRefactory.Ast.PrimitiveExpression primitiveExpression, object data)
            {
                //Tim: perhaps this is too low in the tree? Resulting in having this run almost all the time?
                if (primitiveExpression.LiteralFormat == ICSharpCode.NRefactory.Parser.LiteralFormat.CharLiteral)
                {
                    var regex = new Regex("\\\\u");
                    if (regex.IsMatch(primitiveExpression.StringValue))
                    {
                        UnlockWith(primitiveExpression);
                    }
                }
                return base.VisitPrimitiveExpression(primitiveExpression, data);
            }

        }
    }
}
