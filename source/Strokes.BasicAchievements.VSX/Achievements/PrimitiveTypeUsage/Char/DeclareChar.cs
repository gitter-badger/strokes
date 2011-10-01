﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Strokes.Core;

namespace Strokes.BasicAchievements.Achievements
{
    [AchievementDescription("@MultipleDeclareCharName",
        AchievementDescription = "@MultipleDeclareCharDescription",
        AchievementCategory = "@PrimitiveType")]
    public class DeclareChar : DeclarePrimitiveType<char>
    {
    }
}
