﻿using Rights.Site.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Rights.IService.Rights;
using Rights.Entity.Common;
using Rights.Entity.Db;
using Tracy.Frameworks.Common.Extends;
using Tracy.Frameworks.Common.Const;
using Rights.Common.Helper;

namespace Rights.Site.Controllers
{
    /// <summary>
    /// 组织机构
    /// </summary>
    public class OrganizationController : BaseController
    {
        [LoginAuthorization]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 获取组织机构树，返回json数据
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetTreeOrgData()
        {
            //先获取所有机构
            //然后递归生成JSON数据
            var result = string.Empty;
            StringBuilder sb = new StringBuilder();

            using (var factory = new ChannelFactory<IRightsOrganizationService>("*"))
            {
                var client = factory.CreateChannel();
                var rs = client.GetAll();
                if (rs.ReturnCode == ReturnCodeType.Success)
                {
                    var orgs = rs.Content;
                    if (orgs.HasValue())
                    {
                        sb.Append(RecursionOrg(orgs, 0));
                        sb = sb.Remove(sb.Length - 2, 2);
                        result = sb.ToString();
                    }
                    else
                    {
                        result = "[]";
                    }
                }
            }

            return Content(result);
        }

        /// <summary>
        /// 获取当前用户当前页面可以访问的按钮列表
        /// </summary>
        /// <param name="menuCode">联表查询时用到</param>
        /// <param name="pageName">生成按钮的事件时用到</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetButtonsByUserIdAndMenuCode(string menuCode, string pageName)
        {
            if (menuCode.IsNullOrEmpty())
            {
                throw new ArgumentNullException("menuCode");
            }
            if (pageName.IsNullOrEmpty())
            {
                throw new ArgumentNullException("pageName");
            }

            var result = string.Empty;
            using (var factory = new ChannelFactory<IRightsOrganizationService>("*"))
            {
                var client = factory.CreateChannel();
                var rs = client.GetButtonsByUserIdAndMenuCode(menuCode, loginInfo.Id);
                if (rs.ReturnCode == ReturnCodeType.Success)
                {
                    var dt = rs.Content.ToDataTable();
                    result = ToolbarHelper.GetToolBar(dt, pageName);
                }
            }

            return Content(result);
        }

        /// <summary>
        /// 获取指定机构的所有子机构，0表示获取所有
        /// </summary>
        /// <param name="orgId">机构id</param>
        /// <returns></returns>
        public ActionResult GetChildrenOrgs(int orgId)
        {
            //先获取指定机构的所有子机构
            //然后递归生成JSON数据
            var result = string.Empty;
            StringBuilder sb = new StringBuilder();

            using (var factory = new ChannelFactory<IRightsOrganizationService>("*"))
            {
                var client = factory.CreateChannel();
                var rs = client.GetChildrenOrgs(orgId);
                if (rs.ReturnCode == ReturnCodeType.Success)
                {
                    var orgs = rs.Content;
                    if (orgs.HasValue())
                    {
                        sb.Append(RecursionOrg(orgs, orgId));
                        sb = sb.Remove(sb.Length - 2, 2);
                        result = sb.ToString();
                    }
                    else
                    {
                        result = "[]";
                    }
                }
            }

            return Content(result);
        }

        #region Private method

        private string RecursionOrg(List<TRightsOrganization> list, int parentId)
        {
            StringBuilder sb = new StringBuilder();
            var childOrgs = list.Where(p => p.ParentId == parentId).ToList();
            if (childOrgs.HasValue())
            {
                sb.Append("[");
                for (int i = 0; i < childOrgs.Count; i++)
                {
                    var childStr = RecursionOrg(list, childOrgs[i].Id);
                    if (!childStr.IsNullOrEmpty())
                    {
                        sb.Append("{\"id\":\"" + childOrgs[i].Id.ToString() + "\",\"Code\":\"" + childOrgs[i].Code + "\",\"Enabled\":\"" + childOrgs[i].EnableFlag.Value + "\",\"Sort\":\"" + childOrgs[i].Sort.Value + "\",\"CreatedTime\":\"" + childOrgs[i].CreatedTime.ToString(DateFormat.DATETIME) + "\",\"ParentId\":\"" + childOrgs[i].ParentId.ToString() + "\",\"text\":\"" + childOrgs[i].Name + "\",\"children\":");
                        sb.Append(childStr);
                    }
                    else
                    {
                        sb.Append("{\"id\":\"" + childOrgs[i].Id.ToString() + "\",\"Code\":\"" + childOrgs[i].Code + "\",\"Enabled\":\"" + childOrgs[i].EnableFlag.Value + "\",\"Sort\":\"" + childOrgs[i].Sort.Value + "\",\"CreatedTime\":\"" + childOrgs[i].CreatedTime.ToString(DateFormat.DATETIME) + "\",\"ParentId\":\"" + childOrgs[i].ParentId.ToString() + "\",\"text\":\"" + childOrgs[i].Name + "\"},");
                    }

                }
                sb.Remove(sb.Length - 1, 1);
                sb.Append("]},");
            }
            return sb.ToString();
        }

        #endregion

    }
}