/*
 *
 * (c) Copyright Ascensio System Limited 2010-2021
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * http://www.apache.org/licenses/LICENSE-2.0
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
*/


using System;
using System.Collections.Generic;
using System.Linq;

using ASC.Core;
using ASC.Core.Common.EF;
using ASC.Core.Common.EF.Context;
using ASC.Projects.EF;
using ASC.Core.Tenants;

using ASC.Projects.Core.DataInterfaces;
using ASC.Projects.Core.Domain;
using ASC.Core.Common.Settings;
using ASC.Common;

namespace ASC.Projects.Data.DAO
{
    [Scope]
    public class TimeSpendDao : BaseDao, ITimeSpendDao
    {
        private TenantUtil TenantUtil { get; set; }
        private SettingsManager SettingsManager { get; set; }
        public TimeSpendDao(SecurityContext securityContext, DbContextManager<WebProjectsContext> dbContextManager, TenantUtil tenantUtil, SettingsManager settingsManager, TenantManager tenantManager) : base(securityContext, dbContextManager, tenantManager)
        {
            TenantUtil = tenantUtil;
            SettingsManager = settingsManager;
        }

        public List<TimeSpend> GetByFilter(TaskFilter filter, bool isAdmin, bool checkAccess)
        {
            var query = CreateQuery();

            if (filter.Max != 0 && !filter.Max.Equals(int.MaxValue))
            {
                query = query.Skip((int)filter.Offset);
                query = query.Take((int)filter.Max * 2);
            }
            var fQuery = CreateQueryFilter(query, filter, isAdmin, checkAccess);

            if (!string.IsNullOrEmpty(filter.SortBy))
            {
                var sortColumns = filter.SortColumns["TimeSpend"];
                sortColumns.Remove(filter.SortBy);

                var sortedQuery = SortBy(filter.SortBy, filter.SortOrder, fQuery);

                foreach (var sort in sortColumns.Keys)
                {
                    sortedQuery = ThenBy(filter.SortBy, filter.SortOrder, sortedQuery);
                }
                return sortedQuery.ToList()
                .ConvertAll(tt => ToTimeSpend(tt));
            }
            return fQuery
                .ToList()
                .ConvertAll(tt => ToTimeSpend(tt));
        }

        private IOrderedEnumerable<DbTimeTracking> SortBy(string sortBy, bool sortOrder, IEnumerable<DbTimeTracking> query)
        {
            switch (sortBy)
            {
                case "create_on":
                    return sortOrder ? query.OrderBy(q => q.CreateOn) : query.OrderByDescending(q => q.CreateOn);
                case "date":
                    return sortOrder ? query.OrderBy(q => q.Date) : query.OrderByDescending(q => q.Date);
                case "hours":
                    return sortOrder ? query.OrderBy(q => q.Hours) : query.OrderByDescending(q => q.Hours);
                case "note":
                    return sortOrder ? query.OrderBy(q => q.Note) : query.OrderByDescending(q => q.Note);
            }
            return null;
        }

        private IOrderedEnumerable<DbTimeTracking> ThenBy(string sortBy, bool sortOrder, IOrderedEnumerable<DbTimeTracking> query)
        {
            switch (sortBy)
            {
                case "create_on":
                    return sortOrder ? query.ThenBy(q => q.CreateOn) : query.ThenByDescending(q => q.CreateOn);
                case "date":
                    return sortOrder ? query.ThenBy(q => q.Date) : query.ThenByDescending(q => q.Date);
                case "hours":
                    return sortOrder ? query.ThenBy(q => q.Hours) : query.ThenByDescending(q => q.Hours);
                case "note":
                    return sortOrder ? query.ThenBy(q => q.Note) : query.ThenByDescending(q => q.Note);
            }
            return null;
        }

        public int GetByFilterCount(TaskFilter filter, bool isAdmin, bool checkAccess)
        {
            var query = WebProjectsContext.TimeTracking.Join(WebProjectsContext.Task,
                tt => tt.RelativeTaskId,
                t => t.Id,
                (tt, t) => new QueryTimeTracking()
                {
                    TimeTracking = tt,
                    Task = t
                }).Where(q => q.TimeTracking.TenantId == Tenant && q.Task.TenantId == Tenant);


            var fQuery = CreateQueryFilter(query, filter, isAdmin, checkAccess);

            return fQuery.ToList().Count();
        }

        public float GetByFilterTotal(TaskFilter filter, bool isAdmin, bool checkAccess)
        {
            var query = WebProjectsContext.TimeTracking.Join(WebProjectsContext.Task,
                tt => tt.RelativeTaskId,
                t => t.Id,
                (tt, t) => new QueryTimeTracking()
                {
                    TimeTracking = tt,
                    Task = t
                }).Where(q => q.TimeTracking.TenantId == Tenant && q.Task.TenantId == Tenant);

            var fQuery = CreateQueryFilter(query, filter, isAdmin, checkAccess);
            var count = fQuery.Select(q => q.Hours).Sum();

            return (float)count;
        }

        private IEnumerable<DbTimeTracking> CreateQueryFilter(IQueryable<QueryTimeTracking> query, TaskFilter filter, bool isAdmin, bool checkAccess)
        {
            if (filter.MyProjects || filter.MyMilestones)
            {
                query = query.Join(WebProjectsContext.Participant,
                    q => q.TimeTracking.ProjectId,
                    p => p.ProjectId,
                    (q, p) => new
                    {
                        TimeTracking = q.TimeTracking,
                        Task = q.Task,
                        Participant = p
                    }).Where(q => q.Participant.Removed == 0 && q.TimeTracking.TenantId == q.Participant.Tenant && q.Participant.ParticipantId == CurrentUserID.ToString())
                    .Select(q=> new QueryTimeTracking
                    {
                        Task= q.Task,
                        TimeTracking = q.TimeTracking
                    });
            }

            if (filter.ProjectIds.Count != 0)
            {
                query = query.Where(q => filter.ProjectIds.Contains(q.TimeTracking.ProjectId));
            }
            else
            {
                if (SettingsManager.Load<ProjectsCommonSettings>().HideEntitiesInPausedProjects)
                {
                    if (!checkAccess && isAdmin)
                    {
                        query = query.Join(WebProjectsContext.Project,
                            q => q.TimeTracking.ProjectId,
                            p => p.Id,
                            (q, p) => new
                            {
                                TimeTracking = q.TimeTracking,
                                Task = q.Task,
                                Project = p
                            }).Where(q => q.TimeTracking.TenantId == q.Project.TenantId && (ProjectStatus)q.Project.Status != ProjectStatus.Paused)
                            .Select(q => new QueryTimeTracking
                            {
                                Task = q.Task,
                                TimeTracking = q.TimeTracking
                            });
                    }
                }
            }

            if (filter.Milestone.HasValue || filter.MyMilestones)
            {
                var queryM = query.Join(WebProjectsContext.Milestone,
                    q => q.TimeTracking.ProjectId,
                    m => m.ProjectId,
                    (q, m) => new
                    {
                        TimeTracking = q.TimeTracking,
                        Task = q.Task,
                        Milestone = m
                    }).Where(q => q.TimeTracking.TenantId == q.Milestone.TenantId && q.Milestone.Id == q.Task.MilestoneId);

                if (filter.Milestone.HasValue)
                {
                    queryM = queryM.Where(q=> q.Milestone.Id == filter.Milestone); 
                }
                else if (filter.MyMilestones)
                {
                    queryM = queryM.Where(q => q.Milestone.Id > 0);
                }
                query = queryM.Select(q => new QueryTimeTracking
                {
                    Task = q.Task,
                    TimeTracking = q.TimeTracking
                });
            }

            if (filter.TagId != 0)
            {
                if (filter.TagId == -1)
                {
                    query = query.Join(WebProjectsContext.TagToProject,
                        q => q.TimeTracking.ProjectId,
                        tp => tp.ProjectId,
                        (q, tp) => new
                        {
                            TimeTracking = q.TimeTracking,
                            Task = q.Task,
                            TagToProject = tp
                        }).Where(q => q.TagToProject.TagId == null)
                        .Select(q => new QueryTimeTracking
                        {
                            Task = q.Task,
                            TimeTracking = q.TimeTracking
                        });
                }
                else
                {
                    query = query.Join(WebProjectsContext.TagToProject,
                        q => q.TimeTracking.ProjectId,
                        tp => tp.ProjectId,
                        (q, tp) => new
                        {
                            TimeTracking = q.TimeTracking,
                            Task = q.Task,
                            TagToProject = tp
                        }).Where(q => q.TagToProject.TagId == filter.TagId)
                        .Select(q => new QueryTimeTracking
                        {
                            Task = q.Task,
                            TimeTracking = q.TimeTracking
                        });
                }
            }


            if (filter.UserId != Guid.Empty)
            {
                query = query.Where(q => q.TimeTracking.PersonId == filter.UserId.ToString());
            }


            if (filter.DepartmentId != Guid.Empty)
            {
                query = query.Join(WebProjectsContext.UserGroup,
                    q => q.TimeTracking.PersonId,
                    u => u.UserId.ToString(),
                    (q, u) => new
                    {
                        TimeTracking = q.TimeTracking,
                        Task = q.Task,
                        UserGroup = u
                    }).Where(q => q.UserGroup.Removed == false && q.UserGroup.Tenant == q.TimeTracking.TenantId && q.UserGroup.GroupId == filter.DepartmentId)
                    .Select(q => new QueryTimeTracking
                    {
                        Task = q.Task,
                        TimeTracking = q.TimeTracking
                    });
            }

            var minDate = DateTime.MinValue;
            var maxDate = DateTime.MaxValue;

            if (!filter.FromDate.Equals(minDate) && !filter.FromDate.Equals(maxDate) &&
                !filter.ToDate.Equals(minDate) && !filter.ToDate.Equals(maxDate))
            {
                query = query.Where(q => q.TimeTracking.Date >= filter.FromDate && q.TimeTracking.Date <= filter.ToDate);
            }

            if (filter.PaymentStatuses.Any())
            {
                query = query.Where(q => filter.PaymentStatuses.Contains((PaymentStatus)q.TimeTracking.PaymentStatus));
            }

            if (!string.IsNullOrEmpty(filter.SearchText))
            {
                query = query.Where(q => q.TimeTracking.Note.Contains(filter.SearchText) || q.Task.Title.Contains(filter.SearchText));
            }

            if (checkAccess)
            {
                query = query.Join(WebProjectsContext.Project,
                    q => q.TimeTracking.ProjectId,
                    p => p.Id,
                    (q, p) => new
                    {
                        TimeTracking = q.TimeTracking,
                        Task = q.Task,
                        Project = p
                    }).Where(q => q.Project.TenantId == q.TimeTracking.TenantId && q.Project.Private == 0)
                    .Select(q => new QueryTimeTracking
                    {
                        Task = q.Task,
                        TimeTracking = q.TimeTracking
                    });
            }
            else if (!isAdmin)
            {
                var queryP = query.Join(WebProjectsContext.Project,
                    q => q.TimeTracking.ProjectId,
                    p => p.Id,
                    (q, p) => new
                    {
                        Query = q,
                        Project = p
                    }).Where(q => q.Query.TimeTracking.TenantId == q.Project.TenantId);

                if (!(filter.MyProjects || filter.MyMilestones)) // check!!!!!!!!!!!!!!!!!
                {
                    var queryPP = queryP.Join(WebProjectsContext.Participant,
                        q => q.Query.TimeTracking.ProjectId,
                        p => p.ProjectId,
                        (q, p) => new
                        {
                            Query = q.Query,
                            Project = q.Project,
                            Participant = p
                        }).Where(q => q.Participant.ParticipantId == CurrentUserID.ToString() && q.Participant.Tenant == q.Query.TimeTracking.TenantId);
                    query = queryPP.Where(q => q.Project.Private == 0 
                    || q.Participant.Security != null && q.Participant.Removed == 0 
                    &&
                    (WebProjectsContext.TasksResponsible.Where(tr => tr.TenantId == q.Query.Task.TenantId && tr.TaskId == q.Query.Task.Id && tr.ResponsibleId == CurrentUserID.ToString()).Any() 
                    || q.Participant.Security != 0
                    && (q.Query.Task.MilestoneId == 0 || q.Participant.Security != 0)))
                        .Select(q => q.Query);
                }
                else
                {
                    query = queryP.Where(q => q.Project.Private == 0
                     && WebProjectsContext.TasksResponsible.Where(tr => tr.TenantId == q.Query.Task.TenantId && tr.TaskId == q.Query.Task.Id && tr.ResponsibleId == CurrentUserID.ToString()).Any())
                         .Select(q => q.Query);
                }
            }
            return query
                .AsEnumerable()
                .GroupBy(q => new { q.TimeTracking.Id , q.TimeTracking})
                .Select(q => q.Key.TimeTracking);
        }

        public List<TimeSpend> GetByProject(int projectId)
        {
            return WebProjectsContext.TimeTracking
                .Where(tt => tt.ProjectId == projectId)
                .OrderBy(tt => tt.Date)
                .ToList()
                .ConvertAll(tt => ToTimeSpend(tt));
        }

        public List<TimeSpend> GetByTask(int taskId)
        {
            return WebProjectsContext.
                TimeTracking
                .Where(tt => tt.RelativeTaskId == taskId)
                .OrderBy(tt => tt.Date)
                .ToList()
                .ConvertAll(tt => ToTimeSpend(tt));
        }

        public TimeSpend GetById(int id)
        {
            var tt = WebProjectsContext.TimeTracking
                .Where(tt => tt.Id == id)
                .SingleOrDefault();
            return tt == null ? null : ToTimeSpend(tt);
        }

        public TimeSpend SaveOrUpdate(TimeSpend timeSpend)
        {

            if(WebProjectsContext.TimeTracking.Where(tt=> tt.Id == timeSpend.ID).Any())
            {
                var timeTracking = WebProjectsContext.TimeTracking.Where(tt => tt.Id == timeSpend.ID).SingleOrDefault();
                timeTracking.StatusChanged = TenantUtil.DateTimeNow();
                timeTracking.Date = TenantUtil.DateTimeToUtc(timeTracking.Date);
                timeTracking.CreateBy = CurrentUserID.ToString();
                timeTracking.Note = timeSpend.Note;
                timeTracking.Hours = (float)timeSpend.Hours;
                timeTracking.RelativeTaskId = timeSpend.Task.ID;
                timeTracking.PersonId = timeSpend.Person.ToString();
                timeTracking.CreateOn = timeSpend.CreateOn;
                timeTracking.PaymentStatus = (int)timeSpend.PaymentStatus;
                timeTracking.TenantId = Tenant;
                timeTracking.ProjectId = timeSpend.Task.Project.ID;

                WebProjectsContext.TimeTracking.Update(timeTracking);
                WebProjectsContext.SaveChanges();
                return timeSpend;
            }
            else
            {
                var timeTracking = DbTimeTracking(timeSpend);
                timeTracking.StatusChanged = TenantUtil.DateTimeNow();
                timeTracking.Date = TenantUtil.DateTimeToUtc(timeTracking.Date);
                timeTracking.CreateBy = CurrentUserID.ToString();

                WebProjectsContext.TimeTracking.Add(timeTracking);
                WebProjectsContext.SaveChanges();
                timeSpend.ID = timeTracking.Id;
                return timeSpend;
            }
        }

        public void Delete(int id)
        {
            var timeTracking = WebProjectsContext.TimeTracking.Where(tt => tt.Id == id).SingleOrDefault();
            WebProjectsContext.TimeTracking.Remove(timeTracking);
            WebProjectsContext.SaveChanges();
        }

        private IQueryable<QueryTimeTracking> CreateQuery()
        {
            return WebProjectsContext.TimeTracking.Join(WebProjectsContext.Task,
                tt => tt.RelativeTaskId,
                t => t.Id,
                (tt, t) => new QueryTimeTracking
                {
                    TimeTracking = tt,
                    Task = t
                }).Where(q => q.Task.TenantId == Tenant && q.TimeTracking.TenantId == Tenant);
        }

        private TimeSpend ToTimeSpend(DbTimeTracking timeTracking)
        {
            return new TimeSpend
            {
                ID = timeTracking.Id,
                Note = timeTracking.Note,
                Date = TenantUtil.DateTimeFromUtc(timeTracking.Date),
                Hours = (float)timeTracking.Hours,
                Task = new Task { ID = timeTracking.RelativeTaskId },
                Person = ToGuid(timeTracking.PersonId),
                CreateOn = timeTracking.CreateOn.GetValueOrDefault(),
                CreateBy = timeTracking.CreateBy != null ? ToGuid(timeTracking.CreateBy) : ToGuid(timeTracking.PersonId),
                PaymentStatus = (PaymentStatus)timeTracking.PaymentStatus,
                StatusChangedOn = timeTracking.StatusChanged.GetValueOrDefault()
            };
        }

        private DbTimeTracking DbTimeTracking(TimeSpend timeSpend)
        {
            return new DbTimeTracking
            {
                Id = timeSpend.ID,
                Note = timeSpend.Note,
                Date = TenantUtil.DateTimeFromUtc(timeSpend.Date),
                Hours = (float)timeSpend.Hours,
                RelativeTaskId = timeSpend.Task.ID,
                PersonId = timeSpend.Person.ToString(),
                CreateOn = timeSpend.CreateOn,
                CreateBy = timeSpend.CreateBy.ToString(),
                PaymentStatus = (int)timeSpend.PaymentStatus,
                StatusChanged = timeSpend.StatusChangedOn,
                TenantId = Tenant,
                ProjectId = timeSpend.Task.Project.ID
            };
        }
    }

    public class QueryTimeTracking
    {
        public DbTimeTracking TimeTracking { get; set; } 
        public DbTask Task { get; set; } 
    }
}
