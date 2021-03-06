﻿using System.Collections.Generic;
using System.Linq;
using ICSharpCode.NRefactory.Ast;
using Strokes.BasicAchievements.NRefactory;
using Strokes.Core;
using TypeDeclaration = ICSharpCode.NRefactory.Ast.TypeDeclaration;

namespace Strokes.BasicAchievements.Achievements
{
    [AchievementDescriptor("{520A7282-CF0E-40C7-9E3B-C02E37675AEB}", "@AbstractClassAchievementName",
        AchievementDescription = "@AbstractClassAchievementDescription",
        AchievementCategory = "@Class",
        DependsOn = new[]
                {
                    "{0ec683c7-8005-4da1-abf9-7d027ec1256f}"
                })]
    public class AbstractClassAchievement : NRefactoryAchievement
    {
        protected override AbstractAchievementVisitor CreateVisitor(DetectionSession detectionSession)
        {
            return new Visitor();
        }

        private class Visitor : AbstractAchievementVisitor
        {
            public override object VisitTypeDeclaration(TypeDeclaration typeDeclaration, object data)
            {
                if (typeDeclaration.Type == ClassType.Class && typeDeclaration.Modifier.HasFlag(Modifiers.Abstract))
                    UnlockWith(typeDeclaration);

                return base.VisitTypeDeclaration(typeDeclaration, data);
            }
        }
    }
}