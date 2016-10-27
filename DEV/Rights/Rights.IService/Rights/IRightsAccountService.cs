﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Rights.Entity.Db;
using Rights.Entity.ViewModel;
using Rights.Entity.Common;

namespace Rights.IService.Rights
{
    /// <summary>
    /// 登录相关的服务
    /// </summary>
    [ServiceContract(ConfigurationName = "RightsAccountService.IRightsAccountService")]
    public interface IRightsAccountService
    {
        /// <summary>
        /// 检查登录
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        ServiceResult<TRightsUser> CheckLogin(CheckLoginRequest request);

    }
}