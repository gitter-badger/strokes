﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Strokes.Core;
using Strokes.Core.Data;
using Strokes.Core.Data.Model;
using Strokes.Data;
using System.Collections.Specialized;
using StructureMap;
using GalaSoft.MvvmLight.Messaging;

namespace Strokes.GUI
{
    public class AllAchievementsViewModel : ViewModelBase
    {
        private const string OrderedAchievementsFieldName = "AchievementsOrdered";
        private const string TotalAchievementsFieldName = "TotalAchievements";
        private const string TotalCompletedFieldName = "TotalCompleted";
        private const string PercentageCompletedFieldName = "PercentageCompleted";
        private readonly IAchievementRepository achievementRepository;

        public AllAchievementsViewModel()
        {
            this.AchievementsOrdered = new ObservableCollection<AchievementsPerCategory>();
            this.ResetCommand = new RelayCommand(ResetExecute);

            AchievementContext.AchievementsUnlocked += AchievementContext_AchievementsUnlocked;

            achievementRepository = ObjectFactory.GetInstance<IAchievementRepository>();

            ReloadViewModel();
        }

        public ObservableCollection<AchievementsPerCategory> AchievementsOrdered
        {
            get;
            private set;
        }

        public ICommand ResetCommand
        {
            get;
            private set;
        }

        public int TotalAchievements
        {
            get
            {
                return AchievementsOrdered.Sum(t => t.Count);
            }
        }

        public int TotalCompleted
        {
            get
            {
                return AchievementsOrdered.Sum(t => t.TotalCompleted);
            }
        }

        public int PercentageCompleted
        {
            get
            {
                return TotalAchievements != 0 ? (int)(((double)TotalCompleted / (double)TotalAchievements) * 100) : 0;
            }
        }

        private void ResetExecute()
        {
            if (achievementRepository != null)
            {
                achievementRepository.ResetAchievements();
                ReloadViewModel();
                Messenger.Default.Send(new ResetAchievementsMessage());
            }
        }

        private void ReloadViewModel()
        {
            AchievementsOrdered.Clear();

            var achievements = Enumerable.Empty<Achievement>();
            if (achievementRepository != null)
                achievements = achievementRepository.GetAchievements();

            foreach (var category in achievements.AsCategories())
            {
                var sortedAchievements = category.Achievements
                        .Where(a => IsUnlocked(achievements, a))
                        .OrderByDescending(a => a.IsCompleted)
                        .ThenByDescending(a => a.DateCompleted)
                        .ThenBy(a => a.Name);

                AchievementsOrdered.Add(new AchievementsPerCategory(sortedAchievements)
                {
                    CategoryName = category.CategoryName,
                });
            }
        }

        private bool IsUnlocked(IEnumerable<Achievement> achievements, Achievement achievement)
        {
            if (achievement.DependsOn.Any() == false)
                return true;
            else if (achievement.DependsOn.All(a => a.IsCompleted == true && IsUnlocked(achievements, a)))
                return true;
            else
                return false;
        }

        private void AchievementContext_AchievementsUnlocked(object sender, AchievementsUnlockedEventArgs args)
        {
            foreach (var achievement in args.Achievements)
            {
                var category = AchievementsOrdered.FirstOrDefault(c => c.CategoryName == achievement.Category);
                if (category != null)
                    category.Update(achievement);
            }

            RaisePropertyChanged(OrderedAchievementsFieldName);
            RaisePropertyChanged(TotalCompletedFieldName);
            RaisePropertyChanged(PercentageCompletedFieldName);
        }
    }

    public class AchievementsPerCategory : ObservableCollection<Achievement>, INotifyPropertyChanged
    {
        public AchievementsPerCategory()
        {
            CategoryName = "Unknown";
        }

        public AchievementsPerCategory(IEnumerable<Achievement> collection)
            : base(collection)
        {
            CategoryName = "Unknown";
        }

        public string CategoryName
        {
            get;
            set;
        }

        public int TotalCompleted
        {
            get
            {
                return this.Count(t => t.IsCompleted);
            }
        }

        public int PercentageCompleted
        {
            get
            {
                return this.Count != 0 ? (int)(((double)TotalCompleted / (double)this.Count) * 100) : 0;
            }
        }

        internal void Update(Achievement descriptor)
        {
            var achievementDescriptor = this.SingleOrDefault(a => a.Name == descriptor.Name);

            if (achievementDescriptor != null)
            {
                achievementDescriptor = descriptor;
                
                foreach (var unlocked in achievementDescriptor.Unlocks)
                    this.Add(unlocked);

                ApplySort();

                RaisePropertyChanged("TotalCompleted");
                RaisePropertyChanged("PercentageCompleted");
            }
        }

        internal void RaisePropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        private void ApplySort()
        {
             var sortedItems = this.OrderByDescending(a => a.IsCompleted)
                                   .ThenByDescending(a => a.DateCompleted)
                                   .ThenBy(a => a.Name)
                                   .ToList();

            foreach (var item in sortedItems)
            {
                Move(IndexOf(item), sortedItems.IndexOf(item));
            }
        }
    }
}
