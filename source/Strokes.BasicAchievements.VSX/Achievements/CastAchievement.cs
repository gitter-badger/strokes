﻿using System.Collections.Generic;
using System.Linq;
using ICSharpCode.NRefactory.Ast;
using Strokes.BasicAchievements.NRefactory;
using Strokes.Core;

namespace Strokes.BasicAchievements.Achievements
{
    [AchievementDescription("Swords to plowshares", AchievementDescription = "Cast a variable explicitely", AchievementCategory = "Basic Achievements")]
    public class CastAchievement : NRefactoryAchievement
    {
        protected override AbstractAchievementVisitor CreateVisitor()
        {
            return new Visitor();
        }

        private class Visitor : AbstractAchievementVisitor
        {

            public override object VisitCastExpression(CastExpression castExpression, object data)
            {
                //This was the hardest achievement to write ever :p
                UnlockWith(castExpression);

                return base.VisitCastExpression(castExpression, data);
            }
            
        }
    }
}