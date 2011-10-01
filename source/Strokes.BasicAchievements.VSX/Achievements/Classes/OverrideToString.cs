﻿using System.Collections.Generic;
using System.Linq;
using ICSharpCode.NRefactory.Ast;
using Strokes.BasicAchievements.NRefactory;
using Strokes.Core;

namespace Strokes.BasicAchievements.Achievements
{
    [AchievementDescription("@OverrideToStringAchievementName",
        AchievementDescription = "@OverrideToStringAchievementDescription",
        AchievementCategory = "@Class")]
    public class OverrideToStringAchievement : NRefactoryAchievement
    {
        protected override AbstractAchievementVisitor CreateVisitor()
        {
            return new Visitor();
        }

        private class Visitor : AbstractAchievementVisitor
        {

            public override object VisitMethodDeclaration(MethodDeclaration methodDeclaration, object data)
            {
                if (methodDeclaration.Name == "ToString" && methodDeclaration.Modifier.HasFlag(Modifiers.Override) && methodDeclaration.TypeReference.Type == "System.String")
                    UnlockWith(methodDeclaration);
                return base.VisitMethodDeclaration(methodDeclaration, data);
            }

        }
    }
}