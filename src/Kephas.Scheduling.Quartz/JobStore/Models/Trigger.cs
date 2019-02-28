﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Trigger.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the trigger class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling.Quartz.JobStore.Models
{
    using System;

    using global::Quartz;
    using global::Quartz.Impl.Triggers;

    using Kephas.Data;
    using Kephas.Scheduling.Quartz.JobStore.Model;

    [BsonDiscriminator(RootClass = true)]
    [BsonKnownTypes(typeof (CronTrigger), typeof (SimpleTrigger), typeof (CalendarIntervalTrigger),
        typeof (DailyTimeIntervalTrigger))]
    public abstract class Trigger : GroupedQuartzEntityBase, Model.ITrigger
    {
        protected Trigger()
        {
        }

        protected Trigger(global::Quartz.ITrigger trigger, Model.TriggerState state, string instanceName)
        {
            this.InstanceName = instanceName;
            this.Group = trigger.Key.Group;
            this.Name = trigger.Key.Name;
            this.JobKey = trigger.JobKey;
            this.Description = trigger.Description;
            this.NextFireTime = trigger.GetNextFireTimeUtc()?.UtcDateTime;
            this.PreviousFireTime = trigger.GetPreviousFireTimeUtc()?.UtcDateTime;
            this.State = state;
            this.StartTime = trigger.StartTimeUtc.UtcDateTime;
            this.EndTime = trigger.EndTimeUtc?.UtcDateTime;
            this.CalendarName = trigger.CalendarName;
            this.MisfireInstruction = trigger.MisfireInstruction;
            this.Priority = trigger.Priority;
            this.JobDataMap = trigger.JobDataMap;
        }

        object IIdentifiable.Id => this.Id;

        public JobKey JobKey { get; set; }

        public string Description { get; set; }

        //TODO [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime? NextFireTime { get; set; }

        //TODO [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime? PreviousFireTime { get; set; }

        //TODO [BsonRepresentation(BsonType.String)]
        public Model.TriggerState State { get; set; }

        //TODO [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime StartTime { get; set; }

        //TODO [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime? EndTime { get; set; }

        public string CalendarName { get; set; }

        public int MisfireInstruction { get; set; }

        public int Priority { get; set; }

        public string Type { get; set; }

        public JobDataMap JobDataMap { get; set; }

        public abstract global::Quartz.ITrigger GetTrigger();
    }
}