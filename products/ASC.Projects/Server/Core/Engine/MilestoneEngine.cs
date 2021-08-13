/*
 *
 * (c) Copyright Ascensio System Limited 2010-2021
 *
 * This program is freeware. You can redistribute it and/or modify it under the terms of the GNU 
 * General Public License (GPL) version 3 as published by the Free Software Foundation (https://www.gnu.org/copyleft/gpl.html). 
 * In accordance with Section 7(a) of the GNU GPL its Section 15 shall be amended to the effect that 
 * Ascensio System SIA expressly excludes the warranty of non-infringement of any third-party rights.
 *
 * THIS PROGRAM IS DISTRIBUTED WITHOUT ANY WARRANTY; WITHOUT EVEN THE IMPLIED WARRANTY OF MERCHANTABILITY OR
 * FITNESS FOR A PARTICULAR PURPOSE. For more details, see GNU GPL at https://www.gnu.org/copyleft/gpl.html
 *
 * You can contact Ascensio System SIA by email at sales@onlyoffice.com
 *
 * The interactive user interfaces in modified source and object code versions of ONLYOFFICE must display 
 * Appropriate Legal Notices, as required under Section 5 of the GNU GPL version 3.
 *
 * Pursuant to Section 7 � 3(b) of the GNU GPL you must retain the original ONLYOFFICE logo which contains 
 * relevant author attributions when distributing the software. If the display of the logo in its graphic 
 * form is not reasonably feasible for technical reasons, you must include the words "Powered by ONLYOFFICE" 
 * in every copy of the program you distribute. 
 * Pursuant to Section 7 � 3(e) we decline to grant you any rights under trademark law for use of our trademarks.
 *
*/


using System;
using System.Collections.Generic;
using System.Linq;

using ASC.Common;
using ASC.Core;
using ASC.Core.Tenants;
using ASC.ElasticSearch;
using ASC.Projects.Core.DataInterfaces;
using ASC.Projects.Core.Domain;
using ASC.Projects.Core.Engine;
using ASC.Projects.Core.Services.NotifyService;
using ASC.Projects.EF;

namespace ASC.Projects.Engine
{
    [Scope]
    public class MilestoneEngine
    {
        private ProjectSecurity ProjectSecurity { get; set; }
        public bool DisableNotifications { get; set; }
        private FactoryIndexer<DbMilestone> FactoryIndexer { get; set; }
        private SecurityContext SecurityContext { get; set; }
        private TenantUtil TenantUtil { get; set; }
        private NotifyClient NotifyClient { get; set; }
        private IDaoFactory DaoFactory { get; set; }

        #region Get Milestones

        public MilestoneEngine(FactoryIndexer<DbMilestone> factoryIndexer, SecurityContext securityContext, TenantUtil tenantUtil, NotifyClient notifyClient, ProjectSecurity projectSecurity, IDaoFactory daoFactory)
        {
            FactoryIndexer = factoryIndexer;
            SecurityContext = securityContext;
            TenantUtil = tenantUtil;
            NotifyClient = notifyClient;
            ProjectSecurity = projectSecurity;
            DaoFactory = daoFactory;
        }

        public IEnumerable<Milestone> GetAll()
        {
            return DaoFactory.GetMilestoneDao().GetAll().Where(CanRead);
        }

        public List<Milestone> GetByFilter(TaskFilter filter)
        {
            var listMilestones = new List<Milestone>();
            var anyOne = ProjectSecurity.IsPrivateDisabled;
            var isAdmin = ProjectSecurity.CurrentUserAdministrator;

            while (true)
            {
                var milestones = DaoFactory.GetMilestoneDao().GetByFilter(filter, isAdmin, anyOne);

                if (filter.LastId != 0)
                {
                    var lastMilestoneIndex = milestones.FindIndex(r => r.ID == filter.LastId);

                    if (lastMilestoneIndex >= 0)
                    {
                        milestones = milestones.SkipWhile((r, index) => index <= lastMilestoneIndex).ToList();
                    }
                }

                listMilestones.AddRange(milestones);

                if (filter.Max <= 0 || filter.Max > 150000) break;

                listMilestones = listMilestones.Take((int)filter.Max).ToList();

                if (listMilestones.Count == filter.Max || milestones.Count == 0) break;

                if (listMilestones.Count != 0)
                    filter.LastId = listMilestones.Last().ID;

                filter.Offset += filter.Max;
            }

            return listMilestones;
        }

        public int GetByFilterCount(TaskFilter filter)
        {
            return DaoFactory.GetMilestoneDao().GetByFilterCount(filter, ProjectSecurity.CurrentUserAdministrator, ProjectSecurity.IsPrivateDisabled);
        }

        public List<Tuple<Guid, int, int>> GetByFilterCountForReport(TaskFilter filter)
        {
            return DaoFactory.GetMilestoneDao().GetByFilterCountForReport(filter, ProjectSecurity.CurrentUserAdministrator, ProjectSecurity.IsPrivateDisabled);
        }

        public List<Milestone> GetByProject(int projectId)
        {
            var milestones = DaoFactory.GetMilestoneDao().GetByProject(projectId).Where(CanRead).ToList();
            milestones.Sort((x, y) =>
            {
                if (x.Status != y.Status) return x.Status.CompareTo(y.Status);
                if (x.Status == MilestoneStatus.Open) return x.DeadLine.CompareTo(y.DeadLine);
                return y.DeadLine.CompareTo(x.DeadLine);
            });
            return milestones;
        }

        public List<Milestone> GetByStatus(int projectId, MilestoneStatus milestoneStatus)
        {
            var milestones = DaoFactory.GetMilestoneDao().GetByStatus(projectId, milestoneStatus).Where(CanRead).ToList();
            milestones.Sort((x, y) =>
            {
                if (x.Status != y.Status) return x.Status.CompareTo(y.Status);
                if (x.Status == MilestoneStatus.Open) return x.DeadLine.CompareTo(y.DeadLine);
                return y.DeadLine.CompareTo(x.DeadLine);
            });
            return milestones;
        }

        public List<Milestone> GetUpcomingMilestones(int max, params int[] projects)
        {
            var offset = 0;
            var milestones = new List<Milestone>();
            while (true)
            {
                var packet = DaoFactory.GetMilestoneDao().GetUpcomingMilestones(offset, 2 * max, projects);
                milestones.AddRange(packet.Where(CanRead));
                if (max <= milestones.Count || packet.Count() < 2 * max)
                {
                    break;
                }
                offset += 2 * max;
            }
            return milestones.Count <= max ? milestones : milestones.GetRange(0, max);
        }

        public List<Milestone> GetLateMilestones(int max)
        {
            var offset = 0;
            var milestones = new List<Milestone>();
            while (true)
            {
                var packet = DaoFactory.GetMilestoneDao().GetLateMilestones(offset, 2 * max);
                milestones.AddRange(packet.Where(CanRead));
                if (max <= milestones.Count || packet.Count() < 2 * max)
                {
                    break;
                }
                offset += 2 * max;
            }
            return milestones.Count <= max ? milestones : milestones.GetRange(0, max);
        }

        public List<Milestone> GetByDeadLine(DateTime deadline)
        {
            return DaoFactory.GetMilestoneDao().GetByDeadLine(deadline).Where(CanRead).ToList();
        }

        public Milestone GetByID(int id)
        {
            return GetByID(id, true);
        }

        public Milestone GetByID(int id, bool checkSecurity)
        {
            var m = DaoFactory.GetMilestoneDao().GetById(id);

            if (!checkSecurity)
                return m;

            return CanRead(m) ? m : null;
        }

        public bool IsExists(int id)
        {
            return GetByID(id) != null;
        }

        public string GetLastModified()
        {
            return DaoFactory.GetMilestoneDao().GetLastModified();
        }

        private bool CanRead(Milestone m)
        {
            return ProjectSecurity.CanRead(m);
        }

        #endregion

        #region Save, Delete, Notify

        public Milestone SaveOrUpdate(Milestone milestone)
        {
            return SaveOrUpdate(milestone, false, false);
        }

        public Milestone SaveOrUpdate(Milestone milestone, bool notifyResponsible)
        {
            return SaveOrUpdate(milestone, notifyResponsible, false);
        }

        public Milestone SaveOrUpdate(Milestone milestone, bool notifyResponsible, bool import)
        {
            if (milestone == null) throw new ArgumentNullException("milestone");
            if (milestone.Project == null) throw new Exception("milestone.project is null");
            if (milestone.Responsible.Equals(Guid.Empty)) throw new Exception("milestone.responsible is empty");

            // check guest responsible
            if (ProjectSecurity.IsVisitor(milestone.Responsible))
            {
                ProjectSecurity.CreateGuestSecurityException();
            }

            milestone.LastModifiedBy = SecurityContext.CurrentAccount.ID;
            milestone.LastModifiedOn = TenantUtil.DateTimeNow();

            var isNew = milestone.ID == default(int);//Task is new
            var oldResponsible = Guid.Empty;

            if (isNew)
            {
                if (milestone.CreateBy == default(Guid)) milestone.CreateBy = SecurityContext.CurrentAccount.ID;
                if (milestone.CreateOn == default(DateTime)) milestone.CreateOn = TenantUtil.DateTimeNow();

                ProjectSecurity.DemandCreate<Milestone>(milestone.Project);
                milestone = DaoFactory.GetMilestoneDao().SaveOrUpdate(milestone);
            }
            else
            {
                var oldMilestone = DaoFactory.GetMilestoneDao().GetById(new[] { milestone.ID }).FirstOrDefault();

                if (oldMilestone == null) throw new ArgumentNullException("milestone");

                oldResponsible = oldMilestone.Responsible;

                ProjectSecurity.DemandEdit(milestone);
                milestone = DaoFactory.GetMilestoneDao().SaveOrUpdate(milestone);

            }

            if (!milestone.Responsible.Equals(Guid.Empty))
                NotifyMilestone(milestone, notifyResponsible, isNew, oldResponsible);

            _ = FactoryIndexer.IndexAsync(DaoFactory.GetMilestoneDao().ToDbMilestone(milestone));

            return milestone;
        }

        public Milestone ChangeStatus(Milestone milestone, MilestoneStatus newStatus)
        {
            ProjectSecurity.DemandEdit(milestone);

            if (milestone == null) throw new ArgumentNullException("milestone");
            if (milestone.Project == null) throw new Exception("Project can be null.");
            if (milestone.Status == newStatus) return milestone;
            if (milestone.Project.Status == ProjectStatus.Closed) throw new Exception(EngineResource.ProjectClosedError);
            if (milestone.ActiveTaskCount != 0 && newStatus == MilestoneStatus.Closed) throw new Exception("Can not close a milestone with open tasks");

            milestone.Status = newStatus;
            milestone.LastModifiedBy = SecurityContext.CurrentAccount.ID;
            milestone.LastModifiedOn = TenantUtil.DateTimeNow();
            milestone.StatusChangedOn = TenantUtil.DateTimeNow();

            var senders = new HashSet<Guid> { milestone.Project.Responsible, milestone.CreateBy, milestone.Responsible };

            if (newStatus == MilestoneStatus.Closed && !false && senders.Count != 0)
            {
                NotifyClient.SendAboutMilestoneClosing(senders, milestone);
            }

            if (newStatus == MilestoneStatus.Open && !false && senders.Count != 0)
            {
                NotifyClient.SendAboutMilestoneResumed(senders, milestone);
            }

            return DaoFactory.GetMilestoneDao().SaveOrUpdate(milestone);
        }

        private void NotifyMilestone(Milestone milestone, bool notifyResponsible, bool isNew, Guid oldResponsible)
        {
            //Don't send anything if notifications are disabled
            if (DisableNotifications) return;

            if (isNew && milestone.Project.Responsible != SecurityContext.CurrentAccount.ID && !milestone.Project.Responsible.Equals(milestone.Responsible))
            {
                NotifyClient.SendAboutMilestoneCreating(new List<Guid> { milestone.Project.Responsible }, milestone);
            }

            if (notifyResponsible && milestone.Responsible != SecurityContext.CurrentAccount.ID)
            {
                if (isNew || !oldResponsible.Equals(milestone.Responsible))
                    NotifyClient.SendAboutResponsibleByMilestone(milestone);
                else
                    NotifyClient.SendAboutMilestoneEditing(milestone);
            }
        }


        public void Delete(Milestone milestone)
        {
            if (milestone == null) throw new ArgumentNullException("milestone");

            ProjectSecurity.DemandDelete(milestone);
            DaoFactory.GetMilestoneDao().Delete(milestone.ID);

            var users = new HashSet<Guid> { milestone.Project.Responsible, milestone.Responsible };

            NotifyClient.SendAboutMilestoneDeleting(users, milestone);

            _ = FactoryIndexer.DeleteAsync(DaoFactory.GetMilestoneDao().ToDbMilestone(milestone));
        }

        #endregion
    }
}