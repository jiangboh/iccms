using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace JsonLib
{
    #region 消息Id处理
    public class ApMsgIdClass
    {
        /// <summary>
        /// 当前发送消息ID号（每发一条，消息Id加1，最大值从1再开始）
        /// </summary>
        static private UInt16 SendApNormalMsgId = MIN_NORMAL_MSG_ID;
        public static readonly object mutex_SendApNormalMsgId = new object();

        static private UInt16 SendApTransparentMsgId = MIN_TRANSPARENT_MSG_ID;
        public static readonly object mutex_SendApTransparentMsgId = new object();

        /// <summary>
        /// LTE AGENT主动发消息id；
        /// </summary>
        public const UInt16 LTE_AUTO_SEND = 0;

        /// <summary>
        /// 为透传APP消息
        /// </summary>
       // public const UInt16 APP_TRANSPARENT_MSG = 0xFFF0;

        /// <summary>
        /// 正常消息id最小值
        /// </summary>
        public const UInt16 MIN_NORMAL_MSG_ID = 0x1;
        /// <summary>
        /// 正常消息id最大值
        /// </summary>
        public const UInt16 MAX_NORMAL_MSG_ID = 0xF000;

        /// <summary>
        /// 透传消息id最小值
        /// </summary>
        public const UInt16 MIN_TRANSPARENT_MSG_ID = MAX_NORMAL_MSG_ID + 1;
        /// <summary>
        /// 透传消息id最大值
        /// </summary>
        public const UInt16 MAX_TRANSPARENT_MSG_ID = MIN_TRANSPARENT_MSG_ID + 1000;

        /// <summary>
        /// GSM AGENT主动发消息id；
        /// </summary>
        public const UInt16 GSM_AUTO_SEND = 0xFFFF;

        /// <summary>
        /// 获取当前正常消息id
        /// </summary>
        /// <returns>当前正常消息id</returns>
        public static UInt16 getNormalMsgId()
        {
            lock (mutex_SendApNormalMsgId)
            {
                return SendApNormalMsgId;
            }
        }

        /// <summary>
        /// 正常消息Id自增(+1)
        /// </summary>
        /// <returns>自增后的正常消息id</returns>
        public static UInt16 addNormalMsgId()
        {
            lock (mutex_SendApNormalMsgId)
            {
                if ((SendApNormalMsgId >= MAX_NORMAL_MSG_ID) || (SendApNormalMsgId < MIN_NORMAL_MSG_ID))
                {
                    SendApNormalMsgId = MIN_NORMAL_MSG_ID;
                }
                else
                {
                    SendApNormalMsgId++;
                }
                return SendApNormalMsgId;
            }
        }

        /// <summary>
        /// 获取当前正常消息id
        /// </summary>
        /// <returns>当前正常消息id</returns>
        public static UInt16 getTransparentMsgId()
        {
            lock (mutex_SendApTransparentMsgId)
            {
                return SendApTransparentMsgId;
            }
        }

        /// <summary>
        /// 正常消息Id自增(+1)
        /// </summary>
        /// <returns>自增后的正常消息id</returns>
        public static UInt16 addTransparentMsgId()
        {
            lock (mutex_SendApTransparentMsgId)
            {
                if ((SendApTransparentMsgId >= MAX_TRANSPARENT_MSG_ID) || (SendApTransparentMsgId < MIN_TRANSPARENT_MSG_ID))
                {
                    SendApTransparentMsgId = MIN_TRANSPARENT_MSG_ID;
                }
                else
                {
                    SendApTransparentMsgId++;
                }
                return SendApTransparentMsgId;
            }
        }
    }
    #endregion

    public class ApMsgType
    {
        public const string set_ip_address_request = "set_ip_address_request";
        public const string set_ip_address_response = "set_ip_address_response";
        public const string status_request = "status_request";
        public const string status_response = "status_response";
        public const string set_configuration = "set_configuration";
        public const string set_configuration_result = "set_configuration_result";
        public const string activate_nodeb_request = "activate_nodeb_request";
        public const string activate_nodeb_result = "activate_nodeb_result";
        public const string cell_info_request = "cell_info_request";
        public const string cell_info_result = "cell_info_result";
        public const string femto_status_request = "femto_status_request";
        public const string femto_status_response = "femto_status_response";
        public const string scanner = "scanner";
        public const string result_ack = "result_ack";
        public const string paging_request = "paging_request";
        public const string paging_response = "paging_response";
        public const string meas_report = "meas_report";
        public const string set_son_earfcn = "set_son_earfcn";
        public const string set_son_earfcn_response = "set_son_earfcn_response";
        public const string imsi_list_setconfig = "imsi_list_setconfig";
        public const string imsi_list_config_result = "imsi_list_config_result";
        public const string imsi_list_delconfig = "imsi_list_delconfig";
        public const string imsi_list_delconfig_result = "imsi_list_delconfig_result";
        public const string imsi_list_check = "imsi_list_check";
        public const string imsi_list_check_result = "imsi_list_check_result";
        public const string warning_status = "warning_status";
        public const string set_periodic_restart_time = "set_periodic_restart_time";
        public const string set_periodic_restart_time_response = "set_periodic_restart_time_response";
        public const string get_son_earfcn = "get_son_earfcn";
        public const string get_son_earfcn_response = "get_son_earfcn_response";
        public const string set_redirection_req = "set_redirection_req";
        public const string set_redirection_rsp = "set_redirection_rsp";
        public const string UE_cap_report = "UE_cap_report";
        public const string set_tac_value = "set_tac_value";
        public const string set_xml_msg_all = "set_xml_msg_all";
        public const string set_work_mode = "set_work_mode";
        public const string set_work_mode_reponse = "set_work_mode_reponse";
        public const string get_work_mode = "get_work_mode";
        public const string get_work_mode_reponse = "get_work_mode_reponse";
        public const string set_macro = "set_macro";
        public const string set_macro_response = "set_macro_response";
        public const string get_macro = "get_macro";
        public const string get_macro_response = "get_macro_response";
        public const string set_report_target_request = "set_report_target_request";
        public const string get_report_target_request = "get_report_target_request";
        public const string set_report_target_response = "set_report_target_response";
        public const string get_report_target_response = "get_report_target_response";
        public const string get_redirection_req = "get_redirection_req";
        public const string get_redirection_rsp = "get_redirection_rsp";
        public const string set_upload_req = "set_upload_req";
        public const string set_upload_response = "set_upload_response";
        public const string get_upload_req = "get_upload_req";
        public const string get_upload_response = "get_upload_response";
        public const string set_system_request = "set_system_request";
        public const string get_system_request = "get_system_request";
        public const string set_system_response = "set_system_response";
        public const string get_system_response = "get_system_response";
        public const string Update = "Update";
        public const string Update_result = "Update_result";
        public const string file_eof = "file_eof";
        public const string Get_Log = "Get_Log";
        public const string Get_Log_result = "Get_Log_result";
        public const string get_rt_info = "get_rt_info";
        public const string rt_info_report = "rt_info_report";
        public const string SYNC_request = "SYNC_request";
        public const string SYNC_info = "SYNC_info";
        public const string set_device_id = "set_device_id";
        public const string set_device_id_response = "set_device_id_response";
        public const string get_device_id = "get_device_id";
        public const string get_device_id_response = "get_device_id_response";
        public const string Syncinfo_set = "Syncinfo_set";
        public const string Syncinfo_set_response = "Syncinfo_set_response";
        public const string Syncinfo_get = "Syncinfo_get";
        public const string Syncinfo_get_response = "Syncinfo_get_response";
        public const string imei_list_setconfig = "imei_list_setconfig";
        public const string imei_list_delconfig = "imei_list_delconfig";
        public const string imei_list_config_result = "imei_list_config_result";
        public const string imei_list_delconfig_result = "imei_list_delconfig_result";
        public const string imei_list_check = "imei_list_check";
        public const string imei_list_check_result = "imei_list_check_result";
        public const string imsi_temp_list_config = "imsi_temp_list_config";
        public const string imsi_temp_list_result = "imsi_temp_list_result";
        public const string set_device_reboot = "set_device_reboot";

        public const string set_whitelist_study_request = "set_whitelist_study_request";
        public const string set_whitelist_study_result = "set_whitelist_study_result";

        public const string get_whitelist_study_request = "get_whitelist_study_request";
        public const string get_whitelist_study_result = "get_whitelist_study_result";

        public const string get_general_para_request = "get_general_para_request";
        public const string get_general_para_response = "get_general_para_response";

        public const string set_general_para_request = "set_general_para_request";
        public const string set_general_para_response = "set_general_para_response";

        // 2018-06-27
        public const string set_parameter_request = "set_parameter_request";
        public const string set_param_response = "set_param_response";

        #region GSM设备特有消息类型
        /// <summary>
        /// AGENT透传的GSM格式消息。AP回复消息
        ///
        public const string agent_transmit_gsm_msg = "gsm_msg";
        /// <summary>
        /// AGENT转发的GSM格式消息。配置数据更改(除短消息配置外)
        /// </summary>
        public const string agent_transmit_ack_msg = "ack_msg";
        /// <summary>
        /// AGENT转发的GSM格式消息。配置数据更改(短消息配置更改)
        /// </summary>
        public const string agent_transmit_ack_msg_ms = "ack_msg_ms";

        public const string agent_straight_msg = "straight_msg";

        #endregion
    }

    public class AppMsgType : ApMsgType
    {
        #region 通用出错消息回复

        /// <summary>
        /// APP通用出错消息回复,方向只有从Server到APP客户端
        /// </summary>
        //"dic":
        //{
        //    "RecvType":收到的消息类型
        //    "ErrStr":错误信息描述
        //}
        public const string general_error_result = "general_error_result";

        #endregion

        #region 登录相关

        //
        // app登录请求   
        //"type":"app_login_request",
        //"dic":{
        //    "UserName":"root",
        //    "PassWord":"root",
        //}
        public const string app_login_request = "app_login_request";

        //
        // app登录响应
        //"type":"app_login_response",
        //"dic":
        //    {
        //    "ReturnCode": 返回码：0,成功；其它值为失败
        //    "ReturnStr": 失败原因值。ReturnCode不为0时有意义
        //    "GroupName":"RoleSA",
        //    "DomainList":"设备.深圳.福田.中心广场.西北监控",
        //    "FunList":"查看日志,搜索黑名单,添加用户,添加设备,访问西北监控"
        //}
        public const string app_login_response = "app_login_response";

        #endregion

        #region 心跳相关

        /// <summary>
        /// APP心跳消息，APP每分钟主动发送一次
        /// </summary>
        //"dic":
        //{
        //    "user":用户名
        //    "group":用户所在组
        //}
        public const string app_heartbeat_request = "app_heartbeat_request";

        /// <summary>
        /// APP心跳消息回复
        /// </summary>
        //"dic":
        //{
        //    "timestamp":当前时间戳
        //}
        public const string app_heartbeat_response = "app_heartbeat_response";

        #endregion

        #region 用户相关

        //
        //  app获取所有的用户请求
        //"type":"app_all_user_request"   
        //
        public const string app_all_user_request = "app_all_user_request";

        //
        //  app获取所有的用户响应
        //"dic":
        //    {
        //    "ReturnCode": 返回码：0,成功；其它值为失败
        //    "ReturnStr": 失败原因值。ReturnCode不为0时有意义
        //    "UserCount":"5" 
        //    }
        //
        //   "n_dic":[
        //      {
        //         "name":"1",
        //         "dic":{
        //         "name":"engi",
        //         "des":"this is engi(default)."
        //       }
        //       ...
        //},
        //
        public const string app_all_user_response = "app_all_user_response";

        //
        //  添加用户请求
        //"type":"app_add_user_request"   
        //"dic":
        //    {
        //   "name":"bti-test",
        //   "psw":"123456",
        //   "des":"bti-test"
        //}
        public const string app_add_user_request = "app_add_user_request";

        //
        //  添加用户响应
        //"dic":
        //    {
        //    "ReturnCode": 返回码：0,成功；其它值为失败
        //    "ReturnStr": 失败原因值。ReturnCode不为0时有意义
        //}
        public const string app_add_user_response = "app_add_user_response";


        //
        //  删除用户请求
        //"type":"app_del_user_request"   
        //"dic":
        //    {
        //    "name":"bti-test",
        //}
        public const string app_del_user_request = "app_del_user_request";


        //
        //  删除用户响应
        //"dic":
        //    {
        //    "ReturnCode": 返回码：0,成功；其它值为失败
        //    "ReturnStr": 失败原因值。ReturnCode不为0时有意义
        //}
        public const string app_del_user_response = "app_del_user_response";


        //
        //  添加用户密码请求
        //"type":"app_modify_user_psw_request"   
        //"dic":
        //    {
        //   "name":"bti-test",
        //   "oldPasswd":"123456",
        //   "newPasswd":"654321"
        //}
        public const string app_modify_user_psw_request = "app_modify_user_psw_request";

        //
        //  添加用户响应
        //"dic":
        //    {
        //    "ReturnCode": 返回码：0,成功；其它值为失败
        //    "ReturnStr": 失败原因值。ReturnCode不为0时有意义
        //}
        public const string app_modify_user_psw_response = "app_modify_user_psw_response";

        #endregion

        #region 用户组类型相关

        //  app获取所有的用户组(角色)类型请求
        //"type":"app_all_roletype_request"   
        //
        public const string app_all_roletype_request = "app_all_roletype_request";

        //  app获取所有的用户组(角色)类型响应
        //"dic":
        //    {
        //    "ReturnCode": 返回码：0,成功；其它值为失败
        //    "ReturnStr": 失败原因值。ReturnCode不为0时有意义
        //    "TypeCount":"5"
        //    "RoleType1":"Engineering"     
        //    "Des1":"Engineering"  
        //    "RoleType2":"SuperAdmin"  
        //    "Des2":"Engineering"  
        //    "RoleType3":"Administrator"  
        //    "Des3":"Engineering"  
        //    "RoleType4":"SeniorOperator"  
        //    "Des4":"Engineering"  
        //    "RoleType5":"Operator"
        //    "Des5":"Engineering"  
        //}
        public const string app_all_roletype_response = "app_all_roletype_response";


        //  添加用户组(角色)类型请求
        //"type":"app_add_roletype_request"   
        //"dic":
        //    {
        //    "RoleType":"TempGrp",
        //    "Des":"创建一个临时组(角色)"
        //}
        public const string app_add_roletype_request = "app_add_roletype_request";

        //  添加用户组(角色)类型响应
        //"dic":
        //    {
        //    "ReturnCode": 返回码：0,成功；其它值为失败
        //    "ReturnStr": 失败原因值。ReturnCode不为0时有意义
        //}
        public const string app_add_roletype_response = "app_add_roletype_response";

        //  删除用户组(角色)类型请求
        //"type":"app_add_roletype_request"   
        //"dic":
        //    {
        //    "RoleType":"TempGrp",
        //}
        public const string app_del_roletype_request = "app_del_roletype_request";

        //  删除用户组(角色)类型响应
        //"dic":
        //    {
        //    "ReturnCode": 返回码：0,成功；其它值为失败
        //    "ReturnStr": 失败原因值。ReturnCode不为0时有意义
        //}
        public const string app_del_roletype_response = "app_del_roletype_response";

        #endregion 

        #region 用户组相关

        //  app获取所有的用户组(角色)请求
        //"type":"app_all_role_request"   
        //
        public const string app_all_role_request = "app_all_role_request";

        //  app获取所有的用户组(角色)响应
        //"dic":
        //    {
        //    "ReturnCode": 返回码：0,成功；其它值为失败
        //    "ReturnStr": 失败原因值。ReturnCode不为0时有意义
        //    "GroupCount":"5" 
        //    }
        //
        //   "n_dic":[
        //      {
        //         "name":"RoleEng",
        //         "dic":{
        //         "name":"RoleEng",
        //         "roleType":"Engineering",
        //         "timeStart":"1970-01-01 00:00:00",
        //         "timeEnd":"3000-01-01 00:00:00",
        //         "des":"this is RoleEng."
        //       }
        //       ...
        //},
        //
        public const string app_all_role_response = "app_all_role_response";

        //  添加用户组(角色)请求
        //"type":"app_add_role_request"   
        //"dic":
        //    {
        //   "name":"RoleEng",
        //   "roleType":"Engineering",
        //   "timeStart":"1970-01-01 00:00:00",
        //   "timeEnd":"3000-01-01 00:00:00",
        //   "des":"this is RoleEng."
        //}
        public const string app_add_role_request = "app_add_role_request";

        //  添加用户组(角色)响应
        //"dic":
        //    {
        //    "ReturnCode": 返回码：0,成功；其它值为失败
        //    "ReturnStr": 失败原因值。ReturnCode不为0时有意义
        //}
        public const string app_add_role_response = "app_add_role_response";


        //  删除用户组(角色)请求
        //"type":"app_add_roletype_request"   
        //"dic":
        //    {
        //    "name":"RoleEng",
        //}
        public const string app_del_role_request = "app_del_role_request";


        //  删除用户组(角色)类型响应
        //"dic":
        //    {
        //    "ReturnCode": 返回码：0,成功；其它值为失败
        //    "ReturnStr": 失败原因值。ReturnCode不为0时有意义
        //}
        public const string app_del_role_response = "app_del_role_response";

        #endregion

        #region 用户-用户组相关

        //
        //  app获取所有的用户-用户组请求
        //"type":"app_all_usr_group_request"   
        //
        public const string app_all_usr_group_request = "app_all_usr_group_request";

        //
        //  app获取所有的用户-用户组响应
        //"dic":
        //    {
        //    "ReturnCode": 返回码：0,成功；其它值为失败
        //    "ReturnStr": 失败原因值。ReturnCode不为0时有意义
        //    "UsrGroupCount":"5" 
        //    }
        //
        //   "n_dic":[
        //      {
        //         "name":"1",
        //         "dic":{
        //         "usrName":"engi",
        //         "roleName":"RoleEng",
        //         "des":"engi->RoleEng(默认存在)"
        //       }
        //       ...
        //},
        //
        public const string app_all_usr_group_response = "app_all_usr_group_response";

        //
        //  添加用户-用户组请求
        //"type":"app_add_usr_group_request"   
        //"dic":
        //    {
        //   "usrName":"bti-test",
        //   "roleName":"RoleRoot",
        //   "des":"备注"
        //}
        public const string app_add_usr_group_request = "app_add_usr_group_request";


        //
        //  添加用户-用户组响应
        //"dic":
        //    {
        //    "ReturnCode": 返回码：0,成功；其它值为失败
        //    "ReturnStr": 失败原因值。ReturnCode不为0时有意义
        //}
        public const string app_add_usr_group_response = "app_add_usr_group_response";


        //
        //  删除用户-用户组请求
        //"type":"app_del_usr_group_request"   
        //"dic":
        //    {
        //   "usrName":"bti-test",
        //   "roleName":"RoleRoot"
        //}
        public const string app_del_usr_group_request = "app_del_usr_group_request";


        //
        //  删除用户-用户组响应
        //"dic":
        //    {
        //    "ReturnCode": 返回码：0,成功；其它值为失败
        //    "ReturnStr": 失败原因值。ReturnCode不为0时有意义
        //}
        public const string app_del_usr_group_response = "app_del_usr_group_response";

        #endregion

        #region 权限相关

        //
        //  app获取所有的权限请求
        //"type":"app_all_privilege_request"   
        //
        public const string app_all_privilege_request = "app_all_privilege_request";

        //
        //  app获取所有的权限响应
        //  "dic":
        //    {
        //    "ReturnCode": 返回码：0,成功；其它值为失败
        //    "ReturnStr": 失败原因值。ReturnCode不为0时有意义
        //    "PrivilegeCount":"5" 
        //    }
        //
        //   "n_dic":[
        //       "name":"1",
        //      {
        //         "priId":"1",
        //         "funName":"查看日志"
        //         "aliasName":"查看日志",
        //         "des":"查看日志"
        //       }
        //       ...
        //},
        //
        public const string app_all_privilege_response = "app_all_privilege_response";

        //
        //  添加权限请求
        //"type":"app_add_privilege_request"   
        //"dic":
        //    {
        //   "funName":"搜索IMSI记录",
        //   "aliasName":"搜索IMSI记录",
        //   "des":"搜索IMSI记录"
        //}
        public const string app_add_privilege_request = "app_add_privilege_request";

        //
        //  添加权限响应
        //"dic":
        //    {
        //    "ReturnCode": 返回码：0,成功；其它值为失败
        //    "ReturnStr": 失败原因值。ReturnCode不为0时有意义
        //}
        public const string app_add_privilege_response = "app_add_privilege_response";


        //
        //  删除权限请求
        //"type":"app_del_user_request"   
        //"dic":
        //    {
        //     "funName":"搜索IMSI记录"
        //}
        public const string app_del_privilege_request = "app_del_privilege_request";


        //
        //  删除权限响应
        //"dic":
        //    {
        //    "ReturnCode": 返回码：0,成功；其它值为失败
        //    "ReturnStr": 失败原因值。ReturnCode不为0时有意义
        //}
        public const string app_del_privilege_response = "app_del_privilege_response";

        #endregion

        #region 用户组-权限相关

        //
        //  app获取所有用户组各种权限的请求
        //"type":"app_all_group_privilege_request"   
        //
        public const string app_all_group_privilege_request = "app_all_group_privilege_request";


        //
        //  app获取所有用户组各种权限的响应
        //  "dic":
        //    {
        //    "ReturnCode": 返回码：0,成功；其它值为失败
        //    "ReturnStr": 失败原因值。ReturnCode不为0时有意义
        //    "GroupPriCount":"5" 
        //    }
        //
        //   "n_dic":[
        //       "name":"RoleSA",
        //      {
        //         "priIdSet":"1,2,3,4,5,6,7",
        //         "des":"RoleSA的权限集合，以逗号分隔"
        //       }
        //       ...
        //}
        public const string app_all_group_privilege_response = "app_all_group_privilege_response";


        //
        //  添加用户组-权限请求
        //"type":"app_add_group_privilege_request"   
        //"dic":
        //    {
        //   "roleName":"RoleSA",
        //   "priIdSet":"1,2,3,4,5,6,7",
        //   "des":"备注"
        //}
        public const string app_add_group_privilege_request = "app_add_group_privilege_request";


        //
        //  添加用户组-权限响应
        //"dic":
        //    {
        //    "ReturnCode": 返回码：0,成功；其它值为失败
        //    "ReturnStr": 失败原因值。ReturnCode不为0时有意义
        //}
        public const string app_add_group_privilege_response = "app_add_group_privilege_response";


        //
        //  删除用户组-权限请求
        //"type":"app_del_group_privilege_request"   
        //"dic":
        //    {
        //   "roleName":"RoleSA"
        //}
        public const string app_del_group_privilege_request = "app_del_group_privilege_request";


        //
        //  删除用户组-权限响应
        //"dic":
        //    {
        //    "ReturnCode": 返回码：0,成功；其它值为失败
        //    "ReturnStr": 失败原因值。ReturnCode不为0时有意义
        //}
        public const string app_del_group_privilege_response = "app_del_group_privilege_response";


        //
        //  更新用户组-权限请求
        //"type":"app_update_group_privilege_request"   
        //"dic":
        //    {
        //   "roleName":"RoleSA",
        //   "priIdSet":"1,2,3,4,5,6,7,8,9",
        //   "des":"备注"
        //}
        public const string app_update_group_privilege_request = "app_update_group_privilege_request";


        //
        //  更新用户组-权限响应
        //"dic":
        //    {
        //    "ReturnCode": 返回码：0,成功；其它值为失败
        //    "ReturnStr": 失败原因值。ReturnCode不为0时有意义
        //}
        public const string app_update_group_privilege_response = "app_update_group_privilege_response";

        #endregion

        #region 域操作相关

        //  app获取所有的域信息(设备树)请求
        //"type":"app_all_domain_request"   
        //
        public const string app_all_domain_request = "app_all_domain_request";

        // app获取所有的域信息(设备树)响应
        //"type":"app_all_domain_response",
        //
        //"dic":
        //    {
        //    "ReturnCode": 返回码：0,成功；其它值为失败
        //    "ReturnStr": 失败原因值。ReturnCode不为0时有意义
        //    "NodeCount":"4"   
        //    "n_dic":[
        //      {
        //         "name":"设备",
        //         "dic":{
        //         "id":"1",
        //         "name":"设备",
        //         "parentId":"-1",
        //         "isStation":"0"
        //       }，
        //       {
        //         "name":"设备.深圳",
        //         "dic":{
        //         "id":"2",
        //         "name":"深圳",
        //         "parentId":"1",
        //         "isStation":"0"
        //       }
        //       ...
        //}
        public const string app_all_domain_response = "app_all_domain_response";

        //
        //  添加域请求
        //"type":"app_add_domain_request",
        //"dic":
        //    {
        //    "name":"南山",
        //    "parentNameFullPath":"设备.深圳",
        //    "isStation":0,
        //    "des":"添加一个域:设备.深圳.南山"   
        //}
        public const string app_add_domain_request = "app_add_domain_request";

        //
        //  添加域响应
        //"dic":
        //    {
        //    "ReturnCode": 返回码：0,成功；其它值为失败
        //    "ReturnStr": 失败原因值。ReturnCode不为0时有意义
        //}
        public const string app_add_domain_response = "app_add_domain_response";

        //
        //  删除域请求
        //"type":"app_del_domain_request",
        //"dic":
        //    {
        //    "nameFullPath":"设备.深圳.南山"
        //}
        public const string app_del_domain_request = "app_del_domain_request";

        //
        //  删除域响应
        //"dic":
        //    {
        //    "ReturnCode": 返回码：0,成功；其它值为失败
        //    "ReturnStr": 失败原因值。ReturnCode不为0时有意义
        //}
        public const string app_del_domain_response = "app_del_domain_response";

        //
        //  重命名域请求
        //"type":"app_rename_domain_request",
        //"dic":
        //    {
        //    "oldNameFullPath":"设备.深圳.南山",
        //    "newNameFullPath":"设备.深圳.宝安",
        //    "newDes":"描述"   //当该字段为""时，就不修改oldNameFullPath的描述
        //                      //当该字段不为""时，就修改oldNameFullPath的描述
        //}
        //
        public const string app_rename_domain_request = "app_rename_domain_request";

        //
        //  重命名域响应
        //"dic":
        //    {
        //    "ReturnCode": 返回码：0,成功；其它值为失败
        //    "ReturnStr": 失败原因值。ReturnCode不为0时有意义
        //}
        public const string app_rename_domain_response = "app_rename_domain_response";

        // app操作域的响应
        //"type":"app_oper_domain_response",
        //
        // 对于app_add_domain_request，app_del_domain_request和app_rename_domain_request
        // 三条消息，只响应如下部分
        //
        //"dic":
        //    {
        //    "ReturnCode": 返回码：0,成功；其它值为失败
        //    "ReturnStr": 失败原因值。ReturnCode不为0时有意义
        //}
        //
        //  对于app_all_domain_request，响应如下：
        //  LeafCount表示有多少个叶子节点，null表示非站点节点，
        //  电信FDD,移动TDD,联通FDD,联通W表示站点下的设备列表，用逗号隔开
        //"dic":
        //    {
        //    "ReturnCode":0,
        //    "ReturnStr":"成功"
        //    "LeafCount":"4"
        //    "DomainName1":"设备.深圳.福田.中心广场.西北监控"
        //    "DeviceList1":"电信FDD,移动TDD,联通FDD,联通W"    
        //    "DomainName2":"设备.深圳.福田.莲花山"
        //    "DeviceList2":"null"    
        //    "DomainName3":"设备.深圳.南山"
        //    "DeviceList3":"null"    
        //    "DomainName4":"设备.东莞.城区"
        //    "DeviceList4":"null"    
        //    "n_dic":[
        //      {
        //         "name":"设备.深圳.福田.中心广场.西北监控.电信FDD",
        //         "dic":{
        //         "sn":"EN1800S116340039",
        //         "carry":"-1",
        //         "ipAddr":"172.17.0.123",
        //         "port":"12345",
        //         "netmask":"255.255.255.0",
        //         "mode":"FDD",
        //         "online":"0",
        //         "lastOnline":"2018/5/15 12:34:56",
        //         "isActive":"1"
        //       }
        //},
        //}
        //public const string app_oper_domain_response = "app_oper_domain_response";

        #endregion        

        #region 用户-域相关

        //
        //  app获取所有用户可访问域集合的请求
        //"type":"app_all_usr_domain_request"   
        //
        public const string app_all_usr_domain_request = "app_all_usr_domain_request";


        //
        //  app获取所有用户可访问域集合的响应
        //  "dic":
        //    {
        //    "ReturnCode": 返回码：0,成功；其它值为失败
        //    "ReturnStr": 失败原因值。ReturnCode不为0时有意义
        //    "UsrDomaiCount":"5" 
        //    }
        //
        //   "n_dic":[
        //       "name":"engi",
        //      {
        //         "domainIdSet":"设备.深圳市.南山科技园.站点1,设备.深圳市.白石洲.站点2",
        //         "des":"用户engi可访问域的集合，以逗号分隔"
        //       }
        //       ...
        //}
        public const string app_all_usr_domain_response = "app_all_usr_domain_response";


        //
        //  添加用户-域请求
        //"type":"app_add_usr_domain_request"   
        //"dic":
        //    {
        //   "usrName":"root",
        //   "domainIdSet":"设备.深圳市.南山科技园.站点1,设备.深圳市.白石洲.站点2",
        //   "des":"添加用户root的域集合"
        //}
        public const string app_add_usr_domain_request = "app_add_usr_domain_request";


        //
        //  添加用户-域响应
        //"dic":
        //    {
        //    "ReturnCode": 返回码：0,成功；其它值为失败
        //    "ReturnStr": 失败原因值。ReturnCode不为0时有意义
        //}
        public const string app_add_usr_domain_response = "app_add_usr_domain_response";


        //
        //  删除用户-域请求
        //"type":"app_del_group_privilege_request"   
        //"dic":
        //    {
        //   "usrName":"root"
        //}
        public const string app_del_usr_domain_request = "app_del_usr_domain_request";


        //
        //  删除用户-域响应
        //"dic":
        //    {
        //    "ReturnCode": 返回码：0,成功；其它值为失败
        //    "ReturnStr": 失败原因值。ReturnCode不为0时有意义
        //}
        public const string app_del_usr_domain_response = "app_del_usr_domain_response";


        //
        //  更新用户-域请求
        //"type":"app_update_usr_domain_request"   
        //"dic":
        //    {
        //   "usrName":"root",
        //   "domainIdSet":"设备.深圳市.南山科技园.站点1,设备.深圳市.白石洲.站点2",
        //   "des":"添加用户root的域集合"
        //}
        public const string app_update_usr_domain_request = "app_update_usr_domain_request";


        //
        //  更新用户-域响应
        //"dic":
        //    {
        //    "ReturnCode": 返回码：0,成功；其它值为失败
        //    "ReturnStr": 失败原因值。ReturnCode不为0时有意义
        //}
        public const string app_update_usr_domain_response = "app_update_usr_domain_response";

        #endregion

        #region 设备相关

        //
        // 获取所有设备信息请求
        //"type":"app_all_device_request"   
        //"dic":
        //    {
        //   "parentFullPathName":"设备.深圳.福田.中心广场.西北监控"
        //}
        public const string app_all_device_request = "app_all_device_request";

        //
        //  获取所有设备信息请求响应
        //  "dic":
        //    {
        //    "ReturnCode":0,
        //    "ReturnCode": 返回码：0,成功；其它值为失败
        //    "ReturnStr": 失败原因值。ReturnCode不为0时有意义
        //    "n_dic":[
        //      {
        //         "name":"设备.深圳.福田.中心广场.西北监控.电信FDD",
        //         "dic":{
        //         "sn":"EN1800S116340039",
        //         "carry":"-1",
        //         "ipAddr":"172.17.0.123",
        //         "port":"12345",
        //         "netmask":"255.255.255.0",
        //         "mode":"LTE-FDD",
        //         "online":"0",
        //         "lastOnline":"2018/5/15 12:34:56",
        //         "isActive":"1"
        //         "innerType":"xxx"    //用于软件内部处理
        //         "apVersion":"V1.0"   //AP的版本信息，2018-08-17
        //       }
        //       ...
        //    "n_dic":[
        //      {
        //         "name":"device_unknown",   //device_unknown标识未指派的设备
        //         "dic":{
        //         "sn":"EN1800S116340039",
        //         "carry":"-1",
        //         "ipAddr":"172.17.0.123",
        //         "port":"12345",
        //         "netmask":"255.255.255.0",
        //         "mode":"LTE-FDD",
        //         "online":"0",
        //         "lastOnline":"2018/5/15 12:34:56",
        //         "isActive":"1"
        //         "innerType":"xxx"    //用于软件内部处理
        //         "apVersion":"V1.0"   //AP的版本信息，2018-08-17
        //       }
        //       ... 
        //}
        public const string app_all_device_response = "app_all_device_response";

        //
        //  添加设备请求
        //  n_dic用于将未指派的设备添加了指定的站点下面
        //"type":"app_add_device_request"   
        //"dic":
        //    {
        //   "parentFullPathName":"设备.深圳.福田.中心广场.西北监控",
        //   "name":"电信FDD",
        //   "mode":"LTE-TDD",    //GSM,GSM-V2,TD-SCDMA,WCDMA,LTE-TDD,LTE-FDD   
        //                        //GSM-V2,2018-07-29
        //}
        //   "n_dic":[     //2018-07-04
        //      {
        //         "name":"device_unknown",   //device_unknown标识未指派的设备
        //         "dic":{
        //         "ipAddr":"172.17.0.123",
        //         "port":"12345",
        //         "name":"FDD100"           //没有就为空，有的话就为设备名
        //      }
        //  ]
        public const string app_add_device_request = "app_add_device_request";

        //
        //  2018-07-10,添加了result，rebootflag和timestamp的返回项
        //
        //  添加设备响应
        //"dic":
        //    {
        //    "ReturnCode": 返回码：0,成功；其它值为失败
        //    "ReturnStr": 失败原因值。ReturnCode不为0时有意义
        //    result:"0",       // 0:SUCCESS ; 1:GENERAL FAILURE;2:CONFIGURATION FAIURE OR NOT SUPPORTED
        //    rebootflag:"1",	// 1—立刻reboot,2—需要reboot
        //    timestamp:"xxx"   // Time in seconds when send this message, start from 00:00:00 UTC 1
        //                      // 970, can be convert to local time.
        //}
        public const string app_add_device_response = "app_add_device_response";

        //
        //  删除设备请求
        //"type":"app_del_device_request"   
        //"dic":
        //    {
        //   "parentFullPathName":"设备.深圳.福田.中心广场.西北监控",
        //   "name":"电信FDD"
        //}
        public const string app_del_device_request = "app_del_device_request";

        //
        //  删除设备响应
        //"dic":
        //    {
        //    "ReturnCode": 返回码：0,成功；其它值为失败
        //    "ReturnStr": 失败原因值。ReturnCode不为0时有意义
        //}
        public const string app_del_device_response = "app_del_device_response";

        //
        //  删除设备(未指派)请求
        //"type":"app_del_device_unknown_request"   
        //"dic":
        //    {
        //   "ipAddr":"172.17.0.210",
        //   "port":"12345"
        //   "fullname":"设备.深圳.福田.中心广场.西北监控.电信FDD"  //未指派设备添加到设备树后的全名
        //}
        public const string app_del_device_unknown_request = "app_del_device_unknown_request";

        //
        //  删除设备(未指派)响应
        //"dic":
        //    {     
        // result:"0",      // 0:SUCCESS ; 1:GENERAL FAILURE;2:CONFIGURATION FAIURE OR NOT SUPPORTED
        // rebootflag:"1",	// 1—立刻reboot,2—需要reboot
        // timestamp:"xxx"  // Time in seconds when send this message, start from 00:00:00 UTC 1
        //                  // 970, can be convert to local time.
        //}
        public const string app_del_device_unknown_response = "app_del_device_unknown_response";

        //
        // 【此消息移到AP上线通知消息中实现，不再使用】
        //  更新设备请求
        //   n_dic.dic中的各个字段可以一个或多个，那个字段要更新就包含那个。
        //"type":"app_update_device_request"   
        //"dic":
        //    {
        //   "parentFullPathName":"设备.深圳.福田.中心广场.西北监控",
        //   "name":"电信FDD"
        //},
        //    "n_dic":[
        //      {
        //         "name":null,
        //         "dic":{
        //         "name":"电信FDD-band3",
        //         "sn":"EN1800S116340039",
        //         "ipAddr":"172.17.0.125",
        //         "port":"12345",
        //         "netmask":"255.255.255.0",
        //         "mode":"FDD",
        //       }
        public const string app_update_device_request = "app_update_device_request";

        //  
        // 【此消息移到AP上线通知消息中实现，不再使用】
        //  更新设备响应
        //"dic":
        //    {
        //    "ReturnCode": 返回码：0,成功；其它值为失败
        //    "ReturnStr": 失败原因值。ReturnCode不为0时有意义
        //}
        public const string app_update_device_response = "app_update_device_response";

        //
        //  获取设备详细信息的请求
        //"type":"app_get_device_detail_request"   
        //"dic":
        //    {
        //   "parentFullPathName":"设备.深圳.福田.中心广场.西北监控",
        //   "name":"电信FDD"
        //}
        public const string app_get_device_detail_request = "app_get_device_detail_request";

        //
        //  用于LTE/GSM-V2/CDMA，2018-07-31
        //  获取设备详细信息的响应
        //  1,正常；0，不正常
        //  "dic":
        //    {
        //    "ReturnCode": 返回码：0,成功；其它值为失败
        //    "ReturnStr": 失败原因值。ReturnCode不为0时有意义
        //    "domainId":"2",       
        //    "domainParentId":"1",
        //    "parentFullPathName":"设备.深圳.福田.中心广场.西北监控",
        //    "name":"电信FDD",
        //    "n_dic":[
        //      {
        //         "name":"carry0",
        //         "dic":{
        //         "SCTP":"1",                    //SCTP连接状态 ：1,正常；0，不正常
        //         "S1":"0",                      //S1连接状态   ：1,正常；0，不正常
        //         "GPS":"1",                     //GPS连接状态  ：1,正常；0，不正常
        //         "CELL":"0",                    //CELL状态     :1,正常；0，不正常
        //         "SYNC":"0",                    //同步状态     ：1,正常；0，不正常
        //         "LICENSE":"1",                 //LICENSE状态 ：1,正常；0，不正常
        //         "RADIO":"1",                   //射频状态     ：1,正常；0，不正常
        //         "ALIGN":                       //对齐状态     ：1,已经对齐；0，尚未对齐，2018-11-05
        //         "wSelfStudy"  "0"              //"0"正常状态，"1"自学习状态 ，2018-07-19
        //         "ApReadySt"   "XML-Not-Ready"  //0-"XML-Not-Ready"
        //                                        //1-"XML-Ready"
        //                                        //2-"Sniffering"
        //                                        //3-"Ready-For-Cell"
        //                                        //4-"Cell-Setuping"
        //                                        //5-"Cell-Ready"
        //                                        //6-"Cell-Failure"
        //         "source:"0， 同步源（0：GPS ； 1：CNM ； 2：no sync；3：1588；）  //2019.03.07增加1588同步
        //         "time":"2018-05-24 15:38:55"   //时间戳
        //       }
        //    "n_dic":[
        //      {
        //         "name":"carry1",
        //         "dic":{
        //         "SCTP":"1",                    //SCTP连接状态 ：1,正常；0，不正常
        //         "S1":"0",                      //S1连接状态   ：1,正常；0，不正常
        //         "GPS":"1",                     //GPS连接状态  ：1,正常；0，不正常
        //         "CELL":"0",                    //CELL状态     :1,正常；0，不正常
        //         "SYNC":"0",                    //同步状态     ：1,正常；0，不正常
        //         "LICENSE":"1",                 //LICENSE状态 ：1,正常；0，不正常
        //         "RADIO":"1",                   //射频状态     ：1,正常；0，不正常
        //         "ALIGN":                       //对齐状态     ：1,已经对齐；0，尚未对齐，2018-11-05
        //         "wSelfStudy"  "0"              //"0"正常状态，"1"自学习状态 ，2018-07-19
        //         "ApReadySt" "XML-Not-Ready"    //0-"XML-Not-Ready"
        //                                        //1-"XML-Ready"
        //                                        //2-"Sniffering"
        //                                        //3-"Ready-For-Cell"
        //                                        //4-"Cell-Setuping"
        //                                        //5-"Cell-Ready"
        //                                        //6-"Cell-Failure"
        //         "source:"0，                   //同步源（0：GPS ； 1：CNM ； 2：no sync）
        //         "time":"2018-05-24 15:38:55"   //时间戳
        //       }
        //}
        public const string app_get_device_detail_response = "app_get_device_detail_response";

        #endregion

        #region 黑白名单

        //
        //  2018-07-03添加最后面的5项
        //
        //  获取指定设备或域的黑白名单的请求
        //"type":"app_all_bwlist_request"   
        //"dic":
        //    {                  
        //   "bwListApplyTo":"device",                                    //黑白名单适用于那种类型，device或者domain        
        //   "deviceFullPathName":"设备.深圳.福田.中心广场.西北监控.电信TDD",   //bwListApplyTo为device时起作用
        //   "domainFullPathName":"设备.深圳.福田",                         //bwListApplyTo为domain时起作用
        //   "imsi":"46000xxxxxxxxx",                                     //指定要过滤的IMSI号或包含的字段，不指定是为""
        //   "imei":"46000xxxxxxxxx",                                     //指定要过滤的IMEI号或包含的字段，不指定是为""
        //   "bwFlag":"black",                                            //black,white,other,不指定是为""
        //   "timeStart":"2018-07-03 12:34:56",                           //开始时间，不指定是为""
        //   "timeEnded":"2018-07-04 12:34:56",                           //结束时间，不指定是为""
        //   "des":"xxx"                                                  //界面上用在别名
        //},
        //
        public const string app_all_bwlist_request = "app_all_bwlist_request";


        //  
        //   获取指定设备或域的黑白名单的请求的响应
        //"dic":
        //    {
        //    "CurPageIndex":"1:50",
        //
        //}
        public const string app_all_next_page_bwlist_request = "app_all_next_page_bwlist_request";

        //  
        //   获取指定设备或域的黑白名单下一页的请求
        //"dic":
        //    {
        //    "ReturnCode": 返回码：0,成功；其它值为失败
        //    "ReturnStr": 失败原因值。ReturnCode不为0时有意义
        //    "TotalRecords":"10000",
        //    "CurPageIndex":"1:50",
        //    "PageSize":"200",
        //     "n_dic":[
        //      {
        //         "name":"1",
        //         "dic":{
        //         "imsi":"46000123456788",       //imsi
        //         "imei":"46000123456789",       //imei
        //         "bwFlag":"black",              //黑白名单类型，black,white或者other
        //         "rbStart":"23",                //imsi号不空时有用
        //         "rbEnd":"34",                  //imsi号不空时有用
        //         "time":"2018-05-24 18:58:50",  //时间信息
        //         "des":"黑白名单描述",            //黑白名单描述    
        //         "name":西北监控.电信TDD          //2018-10-11
        //       }
        //       ....
        //
        //}
        public const string app_all_bwlist_response = "app_all_bwlist_response";


        //
        //  添加黑白名单的请求
        //"type":"app_add_bwlist_request"   
        //"dic":
        //    {                  
        //   "bwListApplyTo":"device",                                    //黑白名单适用于那种类型，device或者domain        
        //   "deviceFullPathName":"设备.深圳.福田.中心广场.西北监控.电信TDD",   //bwListApplyTo为device时起作用
        //   "domainFullPathName":"设备.深圳.福田",                         //bwListApplyTo为domain时起作用
        //},
        //    "n_dic":[
        //      {
        //         "name":null,
        //         "dic":{
        //         "imsi":"46000123456788",       //imsi和imei可以同时存在
        //         "imei":"46000123456789",       //imei和imsi可以同时存在
        //         "bwFlag":"black",              //黑白名单类型，black,white或者other
        //                                        //为other时，不下发给AP，直接存库， 2018-09-18
        //                            
        //         "rbStart":"23",                //imsi号不空时有用
        //         "rbEnd":"34",                  //imsi号不空时有用
        //         "time":"2018-05-24 18:58:50",  //时间信息
        //         "des":"黑白名单描述",            //黑白名单描述                    
        //       }
        //       .....
        //
        public const string app_add_bwlist_request = "app_add_bwlist_request";


        //  
        //   添加黑白名单的响应
        //"dic":
        //    {     
        // result:"0",      // 0:SUCCESS ; 1:GENERAL FAILURE;2:CONFIGURATION FAIURE OR NOT SUPPORTED
        // rebootflag:"1",	// 1—立刻reboot,2—需要reboot
        // timestamp:"xxx"  // Time in seconds when send this message, start from 00:00:00 UTC 1
        //                  // 970, can be convert to local time.
        //}
        public const string app_add_bwlist_response = "app_add_bwlist_response";


        //
        //  删除黑白名单的请求
        //"type":"app_del_bwlist_request"   
        //"dic":
        //    {                  
        //   "bwListApplyTo":"device",                                    //黑白名单适用于那种类型，device或者domain        
        //   "deviceFullPathName":"设备.深圳.福田.中心广场.西北监控.电信TDD",   //bwListApplyTo为device时起作用
        //   "domainFullPathName":"设备.深圳.福田",                         //bwListApplyTo为domain时起作用
        //},
        //    "n_dic":[
        //      {
        //         "name":null,
        //         "dic":{
        //         "imsi":"46000123456788",       //imsi和imei可以同时存在
        //         "imei":"46000123456789",       //imei和imsi可以同时存在        
        //         "bwFlag":"black",              //黑白名单类型，black,white或者other
        //                                        //为other时，不下发给AP，直接存库， 2018-09-18
        //       }
        //      ...
        //
        public const string app_del_bwlist_request = "app_del_bwlist_request";


        //  
        //   删除黑白名单的响应
        //"dic":
        //    {
        // result:"0",      // 0:SUCCESS ; 1:GENERAL FAILURE;2:CONFIGURATION FAIURE OR NOT SUPPORTED
        // rebootflag:"1",	// 1—立刻reboot,2—需要reboot
        // timestamp:"xxx"  // Time in seconds when send this message, start from 00:00:00 UTC 1
        //                  // 970, can be convert to local time.
        //}
        public const string app_del_bwlist_response = "app_del_bwlist_response";


        #endregion

        #region 省市区相关

        //
        //  app获取所有的省请求
        //"type":"app_all_province_request"   
        //
        public const string app_all_province_request = "app_all_province_request";

        //
        // app获取所有的省响应
        //"type":"app_all_province_response",
        //
        //"dic":
        //    {
        //    "ReturnCode": 返回码：0,成功；其它值为失败
        //    "ReturnStr": 失败原因值。ReturnCode不为0时有意义
        //    "ProvinceCount":"4"   
        //    "n_dic":[
        //      {
        //         "name":"1",
        //         "dic":{
        //         "provice_id":"110",
        //         "provice_name":"北京市"
        //       }，
        //       {
        //         "name":"2",
        //         "dic":{
        //         "provice_id":"440",
        //         "provice_name":"广东省"
        //       }
        //       ...
        //}
        public const string app_all_province_response = "app_all_province_response";

        //
        //  获取城市请求
        //"type":"app_get_city_request",
        //"dic":
        //    {
        //    "provice_id":"440"
        //}
        public const string app_get_city_request = "app_get_city_request";


        //
        //  获取城市响应
        //"dic":
        //    {
        //    "ReturnCode": 返回码：0,成功；其它值为失败
        //    "ReturnStr": 失败原因值。ReturnCode不为0时有意义
        //    "CityCount":"4"   
        //    "n_dic":[
        //      {
        //         "name":"1",
        //         "dic":{
        //         "city_id":"440300000000",
        //         "city_name":"深圳市"
        //       }，
        //       {
        //         "name":"2",
        //         "dic":{
        //         "city_id":"440100000000",
        //         "city_name":"广州市"
        //       }
        //       ...
        //}
        public const string app_get_city_response = "app_get_city_response";

        //
        //  获取区请求
        //"type":"app_get_distract_request",
        //"dic":
        //    {
        //    "city_id":"440300000000"
        //}
        public const string app_get_distract_request = "app_get_distract_request";


        //
        //  获取城市响应
        //"dic":
        //    {
        //    "ReturnCode": 返回码：0,成功；其它值为失败
        //    "ReturnStr": 失败原因值。ReturnCode不为0时有意义
        //    "DistractCount":"4"   
        //    "n_dic":[
        //      {
        //         "name":"1",
        //         "dic":{
        //         "county_name":"南山区"
        //       }，
        //       {
        //         "name":"2",
        //         "dic":{
        //         "county_name":"宝安区"
        //       }
        //       ...
        //}
        public const string app_get_distract_response = "app_get_distract_response";

        #endregion

        #region 通用参数相关

        //
        //  app设置生效时间段的请求
        //
        //   可用于设置LTE/GSM/GSM-V2/CDMA，2018-07-31
        //
        //"type":"app_set_GenPara_ActiveTime_Request"  
        //"dic":
        //    {
        //   "parentFullPathName":"设备.深圳.福田.中心广场.西北监控",
        //   "name":"电信FDD"
        //   "carry":"0/1/2"     //有载波时包含该字段，2018-07-31
        //                       //0表示载波1，1表示载波2，2表示两个载波都设置
        //
        //   "activeTime1Start":"09:30:00"  生效时间1的起始时间
        //   "activeTime1Ended":"12:30:00"  生效时间1的结束时间
        //   "activeTime2Start":"13:30:00"  生效时间2的起始时间
        //   "activeTime2Ended":"14:30:00"  生效时间2的结束时间
        //   "activeTime3Start":"16:30:00"  生效时间3的起始时间，有的话就添加该项
        //   "activeTime3Ended":"18:30:00"  生效时间3的结束时间，有的话就添加该项
        //   "activeTime4Start":"20:30:00"  生效时间4的起始时间，有的话就添加该项
        //   "activeTime4Ended":"22:30:00"  生效时间4的结束时间，有的话就添加该项
        //}
        //
        public const string app_set_GenPara_ActiveTime_Request = "app_set_GenPara_ActiveTime_Request";

        // 
        // app设置生效时间段的响应
        //"dic":
        //    {
        //    "ReturnCode": 返回码：0,成功；其它值为失败
        //    "ReturnStr": 失败原因值。ReturnCode不为0时有意义
        // }
        public const string app_set_GenPara_ActiveTime_Response = "app_set_GenPara_ActiveTime_Response";

        //
        //  app获取通用参数请求
        //"type":"app_get_GenPara_Request" 
        //"dic":
        //    {
        //   "parentFullPathName":"设备.深圳.福田.中心广场.西北监控",
        //   "name"   :"电信FDD"
        //   "carry"  :"0/1"     //GSM或GSM-V2时传入，2018-08-17
        //}
        public const string app_get_GenPara_Request = "app_get_GenPara_Request";

        //
        // app获取通用参数的响应
        //  
        //  用于TDD/FDD/WCDMA，2018-08-20
        //
        //"type":"app_get_GenPara_Response"   
        //"dic":
        //    {
        //   "ReturnCode": 返回码：0,成功；其它值为失败
        //   "ReturnStr": 失败原因值。ReturnCode不为0时有意义
        //   "domainId":"2",       
        //   "domainParentId":"1",
        //   "parentFullPathName":"设备.深圳.福田.中心广场.西北监控",
        //   "name":"电信FDD",
        //  
        //   "mode":"GSM",          设备工作制式：GSM,TD-SCDMA,WCDMA,LTE-TDD,LTE-FDD
        //   "primaryplmn:"xxx",    PLMN
        //   "earfcndl:"xxx",       工作频点（下行）
        //   "earfcnul:"xxx",       工作频点（上行）
        //   "cellid:"xxx",         cellid,2018-06-26
        //   "pci:"xxx",            工作PCI
        //   "bandwidth:"xxx",      工作Band
        //   "tac:"xxx",            工作Tac
        //   "txpower:"xxx",        发射功率
        //   "periodtac:"xxx",      变换Tac周期
        //   "manualfreq:"xxx",     手动选择工作频点（0：自动选择；1：手动选择）
        //   "bootMode:"0",         设备启动方式（0：半自动。此时需要发送Active命令才开始建小区 1：全自动）
        //   "Earfcnlist:"xxx,xxx", REM扫描频点列表
        //   "Bandoffse:"xxxx",     Band频偏值。用于GPS同步时的补偿值
        //   "NTP:"172.17.0.210",   NTP服务器地址
        //   "ntppri:"5",           NTP获取时间的优先级
        //   "source:"0，           同步源（0：GPS ； 1：CNM ； 2：no sync）
        //   "ManualEnable:"1",     是否启动手动选择同步频点功能（0：不启动 ； 1 :启动）
        //   "ManualEarfcn:"xxx",   手动选择的同步频点
        //   "ManualPci:"xxx",      手动选择的同步PCI
        //   "ManualBw:"xxx"        手动选择的同步小区带宽
        //   "gps_select":"0"       GPS配置，0表示NOGPS，1表示GPS
        //                          2018-07-23
        //   "otherplmn:"xxx,yyy",  多PLMN选项，多个之间用逗号隔开
        //   "periodFreq:"{周期:freq1,freq2,freq3}"  周期以S表示，0表示不做周期性变换；
        //                                          在freq list中进行循环，此时小区配置中的频点失效
        //   "activeTime1Start":"09:30:00"  生效时间1的起始时间
        //   "activeTime1Ended":"12:30:00"  生效时间1的结束时间
        //   "activeTime2Start":"13:30:00"  生效时间2的起始时间
        //   "activeTime2Ended":"14:30:00"  生效时间2的结束时间
        //   "activeTime3Start":"16:30:00"  生效时间3的起始时间，有的话就添加该项
        //   "activeTime3Ended":"18:30:00"  生效时间3的结束时间，有的话就添加该项
        //   "activeTime4Start":"20:30:00"  生效时间4的起始时间，有的话就添加该项
        //   "activeTime4Ended":"22:30:00"  生效时间4的结束时间，有的话就添加该项
        //}
        public const string app_get_GenPara_Response = "app_get_GenPara_Response";

        #endregion

        #region FTP相关

        //
        //  FTP相关操作的请求
        //"type":"app_ftp_oper_request"   
        //"dic":
        //    {                  
        //   "md5":"asdfadfs4adf3d3adf3",   //fileName的MD5
        //   "fileName":"xxxxx.tar.gz",     //要上传的文件名
        //   "version":"x.y.z",             //要上传的版本号       
        //
        public const string app_ftp_oper_request = "app_ftp_oper_request";

        //  
        //"type":"app_ftp_oper_response"   
        //   FTP相关操作的响应
        //"dic":
        //    {
        //    "ReturnCode": 0，            //返回码：0,成功；其它值为失败
        //    "ReturnStr": "成功"，         //失败原因值。ReturnCode不为0时有意义
        //    "ftpUsrName":"root",       
        //    "ftpPwd":"root",
        //    "ftpRootDir": "updaeFile",
        //    "ftpServerIp": "172.17.0.210",
        //    "ftpPort":"21",
        //    "needToUpdate":"0",         //是否需要上传，0不需要，1需要   
        //}
        public const string app_ftp_oper_response = "app_ftp_oper_response";

        //  
        //"type":"app_ftp_oper_response"   
        //   FTP相关操作的响应
        //"dic":
        //    {
        //    "canUpdateFlag":"1",           //是否可以升级，0不可以，1可以  
        //    "fileName":"xxxxx.tar.gz",     //要上传的文件名
        //    "deviceCnt":"2",
        //    "device1":"设备.深圳.福田.中心广场.西北监控.电信TDD"
        //    "device2":"设备.深圳.福田.中心广场.西北监控.移动FDD" 
        //}
        public const string app_ftp_update_request = "app_ftp_update_request";

        //
        //  FTP升级结果通知消息
        //  每升级一台发一条消息给APP，内容为"通用反馈信息体"
        //"type":"app_ftp_update_response"   
        //"dic":
        //    {     
        // result:"0",      // 0:SUCCESS ; 1:GENERAL FAILURE;2:CONFIGURATION FAIURE OR NOT SUPPORTED
        // rebootflag:"1",	// 1—立刻reboot,2—需要reboot
        // timestamp:"xxx"  // Time in seconds when send this message, start from 00:00:00 UTC 1
        //                  // 970, can be convert to local time
        //  }
        //
        public const string app_ftp_update_response = "app_ftp_update_response";

        #endregion

        #region 历史记录搜索       

        //
        //  获取历史记录搜索的请求
        //"type":"app_add_bwlist_request"   
        //"dic":
        //    {                  
        //   "bwListApplyTo":"device",                                    //历史记录搜索适用于那种类型，device，domain或者none
        //   "deviceFullPathName":"设备.深圳.福田.中心广场.西北监控.电信TDD",   //bwListApplyTo为device时起作用
        //   "domainFullPathName":"设备.深圳.福田",                         //bwListApplyTo为domain时起作用
        //   "imsi":"46000xxxxxxxxx",                                     //指定要搜索的IMSI号，不指定是为""
        //   "imei":"46000xxxxxxxxx",                                     //指定要搜索的IMEI号，不指定是为""
        //   "bwFlag":"black",                                            //black,white,other,不指定是为""
        //   "timeStart":"2018-05-23 12:34:56",                           //开始时间，不指定是为""
        //   "timeEnded":"2018-05-29 12:34:56",                           //结束时间，不指定是为""
        //   "RmDupFlag":"0",                                             //是否对设备名称和SN去重标志，0:不去重，1:去重
        //},
        //
        public const string app_history_record_request = "app_history_record_request";

        //  
        //   获取历史记录搜索下一页的请求
        //"dic":
        //    {
        //    "CurPageIndex":"1:50",
        //
        //}
        public const string app_history_record_next_page_request = "app_history_record_next_page_request";

        //  
        //  获取历史记录搜索的的响应
        //"dic":
        //    {
        //    "ReturnCode": 返回码：0,成功；其它值为失败
        //    "ReturnStr": 失败原因值。ReturnCode不为0时有意义
        //    "TotalRecords":"10000",
        //    "CurPageIndex":"1:50",
        //    "PageSize":"200",

        //     "n_dic":[         
        //      {
        //         "name":"1",
        //         "dic":{
        //         "imsi":"46000123456788",       //imsi
        //         "imei":"46000123456789",       //imei
        //         "name":"电信TDD",               //名称
        //         "tmsi":"xxxxxxx",              //TMSI
        //         "bsPwr":"-20",                 //bsPwr
        //         "time":"2018-05-24 18:58:50",  //时间信息
        //         "bwFlag":"black",              //黑白名单类型，black,white或者other
        //         "sn":"EN16110123456789",       //SN号     
        //         "des":"imsi对应的别名"           //2018-09-06新增
        //       }
        //       ....
        //
        //}
        public const string app_history_record_response = "app_history_record_response";

        //  
        //   获取历史记录搜索导出csv文件的请求
        //"dic":
        //    {
        //    "fileName":"abc.csv", //将当前的历史记录搜索导出为abc.csv文件
        //
        //}
        public const string app_history_record_export_csv_request = "app_history_record_export_csv_request";

        //  
        //   获取历史记录搜索导出csv文件的响应
        //
        //"type":"app_history_record_export_csv_response"   
        //   
        //"dic":
        //    {
        //    "ReturnCode": 0，            //返回码：0,成功；其它值为失败
        //    "ReturnStr": "成功"，         //失败原因值。ReturnCode不为0时有意义
        //    "ftpUsrName":"root",       
        //    "ftpPwd":"root",
        //    "ftpRootDir": "updaeFile",
        //    "ftpServerIp": "172.17.0.210",
        //    "ftpPort":"21",
        //    "fileName":"abc.csv",
        //}
        public const string app_history_record_export_csv_response = "app_history_record_export_csv_response";


        //  
        //   历史记录搜索窗口退出的通知（用于释放搜索记录所占的内存）
        //   2018-11-12
        //
        public const string app_history_record_exit_request = "app_history_record_exit_request";

        //  
        //  历史记录搜索窗口退出通知的响应
        //  2018-11-12
        //"dic":
        //    {
        //    "ReturnCode": 返回码：0,成功；其它值为失败
        //    "ReturnStr": 失败原因值。ReturnCode不为0时有意义
        //}
        public const string app_history_record_exit_response = "app_history_record_exit_response";

        //
        //  删除IMSI历史记录的请求
        //"type":"app_history_record_delete_request"   
        //"dic":
        //    {                  
        //   "timeStart":"2019-05-05 12:34:56",    //开始时间，为""时表示从最早的时间开始
        //   "timeEnded":"2019-05-05 22:34:56",    //结束时间，为""时表示到最晚的时间结束
        //},
        //
        public const string app_history_record_delete_request = "app_history_record_delete_request";

        //  
        //  删除IMSI历史记录的响应
        //"dic":
        //    {
        //    "ReturnCode": 返回码：0,成功；其它值为失败
        //    "ReturnStr" : 失败原因值。ReturnCode不为0时有意义
        //    "queryTime" : "123ms"  //查询时间，单位毫秒 
        //}
        public const string app_history_record_delete_response = "app_history_record_delete_response";

        #endregion

        #region GSM设备特有消息类型

        /// <summary>
        /// APP向GSM设备发送消息 （n_dic部分可以一个或多个）
        /// </summary>
        //"dic":
        //    {
        //      "sys":系统号，0表示系统1或通道1或射频1，1表示系统2或通道2或射频2
        //    }
        #region GSM_HJT消息
        //"n_dic":
        //   [
        //       "name":"RECV_SYS_PARA",                 //4.1   系统参数
        //      {
        //					"paraMcc":移动国家码
        //					"paraMnc":移动网号
        //					"paraBsic":基站识别码
        //					"paraLac":位置区号
        //					"paraCellId":小区ID
        //					"paraC2":C2偏移量
        //					"paraPeri":周期性位置更新周期
        //					"paraAccPwr":接入功率
        //					"paraMsPwr":手机发射功率
        //					"paraRejCau":位置更新拒绝原因
        //       }
        //    ],
        //"n_dic":
        //   [
        //       "name":"RECV_SYS_OPTION",                 //4.2  系统选项
        //      {
        //					"opLuSms":登录时发送短信
        //					"opLuImei":登录时获取IMEI
        //					"opCallEn":允许用户主叫
        //					"opDebug":调试模式，上报信令
        //					"opLuType":登录类型
        //					"opSmsType":短信类型
        //       }
        //    ],
        //"n_dic":
        //   [
        //       "name":"RECV_DLRX_PARA",                 //4.3	下行接收参数 （本次暂不实现）
        //      {
        //					"dlFreq":信道号
        //					"dlFn":总帧数
        //					"dlRxMod	":下行接收模式
        //					"dlEnable":下行接收使能
        //       }
        //    ],
        //"n_dic":
        //   [
        //       "name":"RECV_RF_PARA",                 //4.4	射频参数
        //      {
        //					"rfEnable":射频使能
        //					"rfFreq":信道号
        //					"rfPwr":发射功率衰减值
        //       }
        //    ],
        //"n_dic":
        //   [
        //       "name":"RECV_QUERY_VER",                 //4.5	查询版本（本次暂不实现）
        //      {
        //       }
        //    ],
        //"n_dic":
        //   [
        //       "name":"RECV_RF_PARA",                 //4.6	测试命令（本次暂不实现）
        //      {
        //					"gTestCmd":命令详情
        //       }
        //    ],
        //"n_dic":
        //   [
        //       "name":"RECV_SMS_OPTION",                //4.7	短消息中心号码;4.8	短消息原叫号码;4.9 短消息发送时间;4.10  短消息内容
        //       {
        //          "gSmsRpoa":短消息中心号码
        //          "gSmsTpoa":短消息原叫号码
        //          "gSmsScts":短消息发送时间  （时间格式为年/月/日/时/分/秒各两位，不足两位前补0。如2014年4月22日15点46分47秒的消息内容为“140422154647”；）
        //          "gSmsData":短消息内容 （编码格式为Unicode编码）
        //          "autoSendtiny":是否自动发送
        //          "autoFilterSMStiny":是否自动过滤短信
        //          "delayTime":发送延时时间
        //          "smsCodingtiny":短信的编码格式
        //         }
        //    ],
        //"n_dic":
        //   [
        //       "name":"RECV_LIBRARY_REG_ADD",            //4.15	添加登录库 
        //       {
        //          "type":号码类型：“IMSI”、“IMEI”、“TMSI”
        //          "gLibrary":IMSI、IMEI、TMSI号。每个IMSI号占用9个字节，IMSI号总数不能大于300个。
        //       }
        //    ];
        //"n_dic":
        //   [
        //       "name":"RECV_LIBRARY_REG_DELALL",            //4.16	清空登录库中所有号码
        //       {
        //       }
        //    ];
        //"n_dic":
        //   [
        //       "name":"RECV_LIBRARY_REG_QUERY",            //4.17	读出登录库中所有号码
        //       {
        //       }
        //    ];
        //"n_dic":
        //   [
        //       "name":"RECV_REG_MODE",            //4.33	注册工作模式
        //       {
        //          "regMode":模式0时由设备自行根据系统选项决定是否允许终端入网，是否对终端发送短信；
        //                    模式1时设备将终端标识发送给上位机，由上位机告知设备下一步的动作
        //       }
        //    ];
        #endregion
        #region GSM_ZYF/CDMA_ZYF消息
        //"n_dic":
        //   [
        //       "name":"QUERY_NB_CELL_INFO_MSG",            //4.2 GUI查询邻区信息
        //       {
        //       }
        //    ];
        //"n_dic":
        //   [
        //       "name":"CONFIG_FAP_MSG",            //4.4  GUI配置FAP的启动参数
        //       {
        //					"bWorkingMode":XXX		    工作模式:1 为侦码模式 ;3驻留模式.
        //					"bC":XXX		            是否自动切换模式。保留
        //					"wRedirectCellUarfcn":XXX	CDMA黑名单频点	
        //					"bPLMNId":XXX		    PLMN标志
        //					"bTxPower":XXX			实际发射功率.设置发射功率衰减寄存器, 0输出最大功率, 每增加1, 衰减1DB
        //					"bRxGain":XXX			接收信号衰减寄存器. 每增加1增加1DB的增益
        //					"wPhyCellId":XXX		物理小区ID.
        //					"wLAC":XXX			    追踪区域码。GSM：LAC;CDMA：REG_ZONE
        //					"wUARFCN":XXX			小区频点. CDMA 制式为BSID
        //					"dwCellId":XXX			小区ID。注意在CDMA制式没有小区ID，高位WORD 是SID ， 低位WORD 是NID
        //       }
        //    ];
        //"n_dic":
        //   [
        //       "name":"CONTROL_FAP_REBOOT_MSG",            //4.5  GUI控制FAP的重启
        //       {
        //          "bRebootFlag":0/1	FAP重启标志。1表示FAP需要立即重启。
        //       }
        //    ];
        //"n_dic":
        //   [
        //       "name":"UE_ORM_REPORT_MSG",        //4.9  FAP上报UE主叫信息，只用于GSM和CDMA
        //      {
        //					"bOrmType":XXX	    	主叫类型。1=呼叫号码, 2=短消息PDU,3=寻呼测量,
        //                                                    4=短信发送报告（bUeContent表示成功或失败）
        //					"bUeId":XXX	     	    IMSI
        //					"cRSRP":XXX	    	    接收信号强度。寻呼测量时，-128表示寻呼失败
        //					"bUeContentLen":XXX	    Ue主叫内容长度
        //					"bUeContent":XXX	    Ue主叫内容。最大249字节。
        //       }
        //    ],		
        //"n_dic":
        //   [
        //       "name":"PAGE_UE_MSG",                 //4.9  GUI寻呼目标IMSI手机
        //      {
        //					"bpageType":XXX	    	寻呼类型。1=寻呼定位；3=发送短信
        //					"bUeId":XXX	            UE id,一般为IMSI。
        //       }
        //    ],
        //"n_dic":
        //   [
        //       "name":"CONFIG_SMS_CONTENT_MSG",                 //4.10  FAP 配置下发短信号码和内容
        //      {
        //					"sms_ctrl":XXX	    	        上号后是否自动发送短信。0：不自动发送；1：自动发关
        //					"bSMSOriginalNum":XXX	    	主叫号码
        //					"bSMSContent":XXX	            短信内容.unicode编码，每个字符占2字节
        //       }
        //    ],
        //"n_dic":
        //   [
        //       "name":"CONTROL_FAP_RADIO_ON_MSG",            //4.11  GUI 控制FAP开启射频
        //       {
        //       }
        //    ];
        //"n_dic":
        //   [
        //       "name":"CONTROL_FAP_RADIO_OFF_MSG",            //4.12  GUI 控制FA关闭射频
        //       {
        //       }
        //    ];
        //"n_dic":
        //   [
        //       "name":"CONTROL_FAP_RESET_MSG",            //4.13  GUI 控制FAP的软复位
        //       {
        //       }
        //    ];
        //"n_dic":
        //   [
        //       "name":"CONFIG_CDMA_CARRIER_MSG",            //4.14  GUI 配置CDMA多载波参数
        //       {
        //					"wARFCN1":XXX	        工作频点1	
        //					"bARFCN1Mode":XXX	    工作频点1模式。0表示扫描，1表示常开,2表示关闭。
        //					"wARFCN1Duration":XXX	工作频点1扫描时长
        //					"wARFCN1Period":XXX	    工作频点1扫描间隔
        //					"wARFCN2":XXX	        工作频点2
        //					"bARFCN2Mode":XXX	    工作频点2模式。 0表示扫描，1表示常开,2表示关闭。
        //					"wARFCN2Duration":XXX	工作频点2扫描时长
        //					"wARFCN2Period":XXX	    工作频点2扫描间隔
        //					"wARFCN3":XXX	        工作频点3	
        //					"bARFCN3Mode":XXX	    工作频点3模式。 0表示扫描，1表示常开,2表示关闭。
        //					"wARFCN3Duration":XXX	工作频点3扫描时长	
        //					"wARFCN3Period":XXX	    工作频点3扫描间隔
        //					"wARFCN4":XXX	        工作频点4	
        //					"bARFCN4Mode":XXX	    工作频点4模式。	0表示扫描，1表示常开,2表示关闭。
        //					"wARFCN4Duration":XXX	工作频点4扫描时长
        //					"wARFCN4Period":XXX	    工作频点4扫描间隔
        //       }
        //    ];
        //"n_dic":
        //   [
        //       "name":"QUERY_FAP_PARAM_MSG",            //4.15  GUI 查询FAP运行参数
        //       {
        //       }
        //    ];
        //"n_dic":
        //   [
        //       "name":"CONFIG_IMSI_MSG_V3_ID",                 //4.17  大数量imsi名单，用于配置不同的目标IMSI不同的行为
        //      {
        //					"wTotalImsi":n		    本条消息中的IMSI(n<=1000)
        //					"bActionType":XXX		动作类型。1 = Delete All IMSI；2 = Delete Special IMSI；3 = Add IMSI；4 = Query IMSI
        //					"bIMSI_#n#":XXX	        IMSI数组。0~9	配置/删除/查询的IMSI
        //					"bUeActionFlag_#n#":XXX 目标IMSI对应的动作。1 = Reject；5 = Hold ON	
        //       }
        //    ],
        #endregion
        public const string gsm_msg_send = "gsm_msg_send";

        /// <summary>
        /// APP接收到GSM设备消息 （n_dic部分可以一个或多个）
        /// </summary>
        /// 
        //"dic":
        //    {
        //        "sys":系统号，0表示系统1或通道1或射频1，1表示系统2或通道2或射频2
        //        "hardware_id":硬件Id
        //        "Protocol":协议类型。GSM/CDMA。默认GSM，如果没有带此项，则为GSM。
        //    }
        #region GSM_HJT消息
        //"n_dic":
        //   [
        //       "name":"SEND_REQ_CNF",                 //5.1	请求确认
        //      {
        //					"cnfType;":确认的请求类型 //同enum RecvPktType
        //					"cnfInd":0表示正确接收，1表示参数错误
        //       }
        //    ],
        //"n_dic":
        //   [
        //       "name":"SEND_OM_INFO",                 //5.2	设备OM信息
        //      {
        //					"gOmInfo;":0表示设备工作正常，其它值表示设备故障，故障代码待定义。
        //       }
        //    ],
        //"n_dic":
        //   [
        //       "name":"SEND_VER_INFO",                 //5.3	设备版本信息
        //      {
        //					"verApp;":应用层版本号。16进制(如201305300001表示为ueImsi[0]~ ueImsi[5]分别为0x20, 0x13, 0x05, 0x30, 0x00, 0x01)
        //					"verPhy;":物理层版本号。16进制。同verApp
        //       }
        //    ],
        //"n_dic":
        //   [
        //       "name":"SEND_UE_INFO",                 //5.4	用户设备信息
        //      {
        //					"ueImsi":手机IMSI号
        //					"ueImei":手机IMEI号
        //					"ueMsisdn":手机号码号段
        //					"uePwr":设备接收的手机功率
        //                  "userType":用户类型，2018-11-05
        //					"UeRegtype":0表示位置更新接受，6表示位置更新拒绝
        //					"ueQueryResult":查询结果，同查询响应SEND_QUERY_RSP中的flagType
        //					"ueTmsi":TMSI号
        //					"ueLlac":采集设备自己的LAC号
        //					"ueSlac":从外网转移过来时，它原来的LAC号
        //       }
        //    ],
        //"n_dic":
        //   [
        //       "name":"SEND_TEST_INFO",                 //5.5	返回测试信息
        //      {
        //					"gTestInfo":测试信息，内容由设备自定义
        //       }
        //    ],
        //"n_dic":
        //   [
        //       "name":"SEND_BS_INFO",                 //5.6	返回基站信息
        //      {
        //					"gBsInfo":测试信息，内容由设备自定义
        //       }
        //    ],
        //"n_dic":
        //   [
        //       "name":"SEND_QUERY_REQ",                 //5.7	查询请求
        //      {
        //					"queryChno":查询信道号，设备用来区分不同的查询请求消息
        //					"queryImsi":查询IMSI，需要查询的UE的IMSI
        //       }
        //    ],
        //"n_dic":
        //   [
        //       "name":"SEND_LIBRARY_REG",                 //5.8	返回读出的登录库
        //       {
        //					"type":号码类型：“IMSI”、“IMEI”、“TMSI”
        //                  "gLibrary":IMSI号的格式见3.4.15节
        //       }
        //    ],
        //"n_dic":
        //   [
        //       "name":"SEND_LIBRARY_SMS",                 //5.9	返回读出的短信库
        //       {
        //					"type":号码类型：“IMSI”、“IMEI”、“TMSI”
        //                  "gLibrary":IMSI号的格式见3.4.15节
        //       }
        //    ],
        //"n_dic":
        //   [
        //       "name":"SEND_OBJECT_POWER",                 //5.10	定位信息
        //      {
        //					"power1":目标第1象限功率 (单位dBm)
        //					"power2":目标第2象限功率 (单位dBm)
        //					"power3":目标第3象限功率 (单位dBm)
        //					"power4":目标第4象限功率 (单位dBm)
        //					"bs":手机检测到BS的功率(单位dBm)
        //       }
        //    ],
        //"n_dic":
        //   [
        //       "name":"SEND_BS_SEAR_INFO",                 //5.11	基站搜索结果信息  (未实现)
        //      {
        //       }
        //    ],
        //"n_dic":
        //   [
        //       "name":"SEND_MS_CALL_SETUP",                 //5.12	手机主动发起呼叫
        //      {
        //					"imsi":IMSI
        //                  "number":手机号
        //       }
        //    ],
        //"n_dic":
        //   [
        //       "name":"SEND_MS_SMS_SEND",                 //5.13	手机主动发起短信
        //      {
        //					"imsi":IMSI
        //                  "number":手机号
        //					"codetype":编码类型
        //					"data":短信内容
        //       }
        //    ],
        //"n_dic":
        //   [
        //       "name":"SEND_MS_CALL_OPERATE",                 //5.14	手机接听和挂机操作
        //      {
        //					"call_operate":0x00：表示手机拒接电话或挂断电话
        //					               0x01：表示手机接听电话
        //					               0x02：表示手机未进行操作，最后系统超时
        //					               0x03：表示BS未进行操作，最后系统超时
        //					               0x04：表示手机接收短信成功
        //					               0x05：表示手机接收短信失败
        //       }
        //    ],
        #endregion
        #region GSM_ZYF/CDMA_ZYF消息
        //"n_dic":
        //   [
        //       "name":"SEND_REQ_CNF",                 // 	请求确认
        //      {
        //					"cnfType;":确认的请求类型 //同enum RecvPktType
        //					"cnfInd":0表示正确接收，1表示参数错误
        //       }
        //    ],
        //"n_dic":
        //   [
        //       "name":"FAP_NB_CELL_INFO_MSG",                 //4.3  FAP上报邻区信息
        //      {
        //					"bFapNbCellNum":n	         邻小区个数。最多16个(n<=16)
        //					"Cell_#n#/bGCId":XXX         小区ID。注意在CDMA制式没有小区ID，高位WORD是SID，低位WORD是NID
        //					"Cell_#n#/bPLMNId":XXX       邻小区PLMN标志。
        //					"Cell_#n#/cRSRP":XXX	     信号功率
        //					"Cell_#n#/wTac":XXX	         追踪区域码。GSM：LAC；CDMA：REG_ZONE
        //					"Cell_#n#/wPhyCellId":XXX	 物理小区ID。GSM：BSIC；CDMA：PN
        //					"Cell_#n#/wUARFCN":XXX	     小区频点
        //					"Cell_#n#/cRefTxPower":XXX	 参考发射功率。GSM制式时为C1测量值
        //					"Cell_#n#/bNbCellNum":XXX	 邻小区的令小区个数
        //					"Cell_#n#/bC2":XXX	         C2测量值。GSM,其他制式保留
        //					"Cell_#n#/bReserved1":XXX	 只用于LTE,其它保留
        //					"Cell_#n#/stNbCell":m		 邻小区的邻小区个数，最多32个（m<=32）
        //					"Cell_#n#/NeighCell_#m#/wUarfcn":XXX	    小区频点
        //					"Cell_#n#/NeighCell_#m#/wPhyCellId":XXX	    物理小区ID。GSM:BSIC；CDMA：PN
        //					"Cell_#n#/NeighCell_#m#/cRSRP":XXX	        信号功率
        //					"Cell_#n#/NeighCell_#m#/cC1":XXX	        C1测量值。只用于GSM制式
        //					"Cell_#n#/NeighCell_#m#/bC2":XXX	        C2测量值。只用于GSM制式
        //       }
        //    ],
        //"n_dic":
        //   [
        //       "name":"FAP_TRACE_MSG",                 //4.7  FAP上报一些事件和状态给GUI，GUI程序需要显示给操作者看。
        //      {
        //					"wTraceLen":XXX	      Trace长度
        //                  "cTrace":XXX          Trace内容
        //       }
        //    ],
        //"n_dic":
        //   [
        //       "name":"UE_STATUS_REPORT_MSG",                 //4.8  FAP上报UE相关状态
        //      {
        //					"imsi":XXX	    //上报imsi，如果没有为空
        //					"imei":XXX      //上报imei，如果没有为空      
        //					"tmsi":XXX	    //上报tmsi，如果没有为空       
        //					"rsrp":XXX	    //上报rsrp，如果没有为空
        //                  "sn":XXX        //上报ap的Sn
        //                  "userType":XXX  //用户类型，2018-11-05
        //       }
        //    ],
        //"n_dic":
        //   [
        //       "name":"UE_ORM_REPORT_MSG",                 //4.9  FAP上报UE主叫信息，只用于GSM和CDMA
        //      {
        //					"bOrmType":XXX	    	主叫类型。1=呼叫号码, 2=短消息PDU,3=寻呼测量,
        //                                                    4=短信发送报告（bUeContent表示成功或失败）
        //					"bUeId":XXX	     	    IMSI
        //					"cRSRP":XXX	    	    接收信号强度。寻呼测量时，-128表示寻呼失败
        //					"bUeContentLen":XXX	    Ue主叫内容长度
        //					"bUeContent":XXX	    Ue主叫内容。最大249字节。
        //       }
        //    ],
        //"n_dic":
        //   [
        //       "name":"FAP_PARAM_REPORT_MSG",                 //4.16  FAP上报FAP运行参数
        //      {
        //					"bWorkingMode":XXX		工作模式。1：侦码模式；3：驻留模式(GSM/CDMA支持)
        //					"wCDMAUarfcn":XXX		CDMA黑名单频点
        //					"bPLMNId":XXX		    PLMN标志。ASCII字符
        //					"bDlAtt":XXX		    发送衰减。0~89，Unit: dB
        //					"bRxGain":XXX		    保留字段。Unit: dB
        //					"wPhyCellId":XXX		物理小区ID。GSM：不用；CDMA：PN
        //					"wLac":XXX		        区域码。GSM：LAC；CDMA：REG_ZONE
        //					"wUARFCN":XXX		    小区频点。CDMA制式为BSID
        //					"dwCellId":XXX		    小区ID。注意在CDMA制式没有小区ID，高位WORD是SID，低位WORD是NID
        //					"wARFCN1":XXX	        工作频点1	
        //					"bARFCN1Mode":XXX	    工作频点1模式。0表示扫描，1表示常开,2表示关闭。
        //					"wARFCN1Duration":XXX	工作频点1扫描时长
        //					"wARFCN1Period":XXX	    工作频点1扫描间隔
        //					"wARFCN2":XXX	        工作频点2
        //					"bARFCN2Mode":XXX	    工作频点2模式。 0表示扫描，1表示常开,2表示关闭。
        //					"wARFCN2Duration":XXX	工作频点2扫描时长
        //					"wARFCN2Period":XXX	    工作频点2扫描间隔
        //					"wARFCN3":XXX	        工作频点3	
        //					"bARFCN3Mode":XXX	    工作频点3模式。 0表示扫描，1表示常开,2表示关闭。
        //					"wARFCN3Duration":XXX	工作频点3扫描时长	
        //					"wARFCN3Period":XXX	    工作频点3扫描间隔
        //					"wARFCN4":XXX	        工作频点4	
        //					"bARFCN4Mode":XXX	    工作频点4模式。	0表示扫描，1表示常开,2表示关闭。
        //					"wARFCN4Duration":XXX	工作频点4扫描时长
        //					"wARFCN4Period":XXX	    工作频点4扫描间隔
        //       }
        //    ], 
        #endregion
        public const string gsm_msg_recv = "gsm_msg_recv";

        //
        //  app获取GSM设备的信息
        //"type":"app_get_GsmInfo_Request" 
        //"dic":
        //    {
        //   "parentFullPathName":"设备.深圳.福田.中心广场.西北监控",
        //   "name":"GSM-Name"     //GSM的名称
        //   "carry":"0"           //GSM的载波标识，"0"或者"1"
        //}
        public const string app_get_GsmInfo_Request = "app_get_GsmInfo_Request";

        //
        //  app获取GSM设备的信息
        //"type":"app_get_GsmInfo_Response" 
        //"dic":
        //    {
        //    "ReturnCode": 返回码：0,成功；其它值为失败
        //    "ReturnStr": 失败原因值。ReturnCode不为0时有意义
        //    "domainId":"2",       
        //    "domainParentId":"1",
        //    "parentFullPathName":"设备.深圳.福田.中心广场.西北监控",
        //    "name":"GSM-Name",
        //    "carry":"0"          
        //					"paraMcc":移动国家码
        //					"paraMnc":移动网号
        //					"paraBsic":基站识别码
        //					"paraLac":位置区号
        //					"paraCellId":小区ID
        //					"paraC2":C2偏移量
        //					"paraPeri":周期性位置更新周期
        //					"paraAccPwr":接入功率
        //					"paraMsPwr":手机发射功率
        //					"paraRejCau":位置更新拒绝原因
        //					"opLuSms":登录时发送短信
        //					"opLuImei":登录时获取IMEI
        //					"opCallEn":允许用户主叫
        //					"opDebug":调试模式，上报信令
        //					"opLuType":登录类型
        //					"opSmsType":短信类型
        //                  "opRegModel":登录模式
        //					"rfEnable":射频使能
        //					"rfFreq":信道号
        //					"rfPwr":发射功率衰减值
        //
        //                  2018-08-20
        //                  "activeTime1Start":"09:30:00"  生效时间1的起始时间
        //                  "activeTime1Ended":"12:30:00"  生效时间1的结束时间
        //                  "activeTime2Start":"13:30:00"  生效时间2的起始时间
        //                  "activeTime2Ended":"14:30:00"  生效时间2的结束时间
        //                  "activeTime3Start":"16:30:00"  生效时间3的起始时间，有的话就添加该项
        //                  "activeTime3Ended":"18:30:00"  生效时间3的结束时间，有的话就添加该项
        //                  "activeTime4Start":"20:30:00"  生效时间4的起始时间，有的话就添加该项
        //                  "activeTime4Ended":"22:30:00"  生效时间4的结束时间，有的话就添加该项
        //
        //    "n_dic":[
        //      {
        //         "name":"1",
        //         "dic":{
        //          "smsRPOA":短消息中心号码
        //          "smsTPOA":短消息原叫号码
        //          "smsSCTS":短消息发送时间  
        //          "smsDATA":短消息内容 （编码格式为Unicode编码）
        //          "autoSend":是否自动发送
        //          "autoFilterSMS":是否自动过滤短信
        //          "delayTime":发送延时时间
        //          "smsCoding":短信的编码格式
        //       }，
        //       {
        //         "name":"2",
        //         "dic":{
        //          "smsRPOA":短消息中心号码
        //          "smsTPOA":短消息原叫号码
        //          "smsSCTS":短消息发送时间  
        //          "smsDATA":短消息内容 （编码格式为Unicode编码）
        //          "autoSend":是否自动发送
        //          "autoFilterSMS":是否自动过滤短信
        //          "delayTime":发送延时时间
        //          "smsCoding":短信的编码格式
        //       }
        //       ...
        //}
        public const string app_get_GsmInfo_Response = "app_get_GsmInfo_Response";

        //
        //  app获取GSM-V2或CDMA设备的信息
        //"type":"app_get_GCInfo_Request" 
        //"dic":
        //    {
        //   "parentFullPathName":"设备.深圳.福田.中心广场.西北监控",
        //   "name":"GCname"         //GSM-V2/CDMA的名称
        //   "carry":"x"             //GSM-V2的载波标识，"0"或者"1" ; CDMA时为"-1"
        //}
        public const string app_get_GCInfo_Request = "app_get_GCInfo_Request";

        //
        //  app获取GSM设备的信息
        //"type":"app_get_GsmInfo_Response" 
        //"dic":
        //    {
        //    "ReturnCode": 返回码：0,成功；其它值为失败
        //    "ReturnStr":  失败原因值。ReturnCode不为0时有意义
        //    "domainId":"2",       
        //    "domainParentId":"1",
        //    "parentFullPathName":"设备.深圳.福田.中心广场.西北监控",
        //    "name":"GCname",
        //    "carry":"0"          
        //"n_dic":
        //   [
        //       "name":"CONFIG_FAP_MSG",            //4.4  GUI配置FAP的启动参数
        //       {
        //					"bWorkingMode":XXX		    工作模式:1 为侦码模式 ;3驻留模式.
        //					"bC":XXX		            是否自动切换模式。保留
        //					"wRedirectCellUarfcn":XXX	CDMA黑名单频点
        //					"dwDateTime":XXX			当前时间	
        //					"bPLMNId":XXX		    PLMN标志
        //					"bTxPower":XXX			实际发射功率.设置发射功率衰减寄存器, 0输出最大功率, 每增加1, 衰减1DB
        //					"bRxGain":XXX			接收信号衰减寄存器. 每增加1增加1DB的增益
        //					"wPhyCellId":XXX		物理小区ID.
        //					"wLAC":XXX			    追踪区域码。GSM：LAC;CDMA：REG_ZONE
        //					"wUARFCN":XXX			小区频点. CDMA 制式为BSID
        //					"dwCellId":XXX			小区ID。注意在CDMA制式没有小区ID，高位WORD 是SID ， 低位WORD 是NID
        //       }
        //    ]; 
        //"n_dic":
        //   [
        //       "name":"FAP_TRACE_MSG",     //4.7  FAP上报一些事件和状态给GUI，GUI程序需要显示给操作者看。
        //      {
        //					"wTraceLen":XXX	      Trace长度
        //                  "cTrace":XXX          Trace内容
        //       }
        //    ],
        //"n_dic":
        //   [
        //       "name":"UE_ORM_REPORT_MSG",                 //4.9  FAP上报UE主叫信息，只用于GSM和CDMA
        //      {
        //					"bOrmType":XXX	    	主叫类型。1=呼叫号码, 2=短消息PDU,3=寻呼测量
        //					"bUeId":XXX	     	    IMSI
        //					"cRSRP":XXX	    	    接收信号强度。寻呼测量时，-128表示寻呼失败
        //					"bUeContentLen":XXX	    Ue主叫内容长度
        //					"bUeContent":XXX	    Ue主叫内容。最大249字节。
        //       }

        //    ],
        //"n_dic":
        //   [
        //       "name":"CONFIG_SMS_CONTENT_MSG",                 //4.10  FAP 配置下发短信号码和内容
        //      {
        //					"sms_ctrl":XXX	    	        上号后是否自动发送短信。0：不自动发送；1：自动发关
        //					"bSMSOriginalNumLen":XXX	    主叫号码长度
        //					"bSMSOriginalNum":XXX	    	主叫号码
        //					"bSMSContentLen":XXX	    	短信内容字数
        //					"bSMSContent":XXX	            短信内容.unicode编码，每个字符占2字节
        //       }
        //    ],
        //"n_dic":
        //   [
        //       "name":"CONFIG_CDMA_CARRIER_MSG",            //4.14  GUI 配置CDMA多载波参数
        //       {
        //					"wARFCN1":XXX	        工作频点1	
        //					"bARFCN1Mode":XXX	    工作频点1模式。0表示扫描，1表示常开,2表示关闭。
        //					"wARFCN1Duration":XXX	工作频点1扫描时长
        //					"wARFCN1Period":XXX	    工作频点1扫描间隔
        //					"wARFCN2":XXX	        工作频点2
        //					"bARFCN2Mode":XXX	    工作频点2模式。 0表示扫描，1表示常开,2表示关闭。
        //					"wARFCN2Duration":XXX	工作频点2扫描时长
        //					"wARFCN2Period":XXX	    工作频点2扫描间隔
        //					"wARFCN3":XXX	        工作频点3	
        //					"bARFCN3Mode":XXX	    工作频点3模式。 0表示扫描，1表示常开,2表示关闭。
        //					"wARFCN3Duration":XXX	工作频点3扫描时长	
        //					"wARFCN3Period":XXX	    工作频点3扫描间隔
        //					"wARFCN4":XXX	        工作频点4	
        //					"bARFCN4Mode":XXX	    工作频点4模式。	0表示扫描，1表示常开,2表示关闭。
        //					"wARFCN4Duration":XXX	工作频点4扫描时长
        //					"wARFCN4Period":XXX	    工作频点4扫描间隔
        //       }
        //    ];
        ////////"n_dic":  2018-11-09，这部分移除掉，变成了用app_all_bwlist_request进行请求
        ////////   [
        ////////       "name":"CONFIG_IMSI_MSG_V3_ID",    //4.17  大数量imsi名单，用于配置不同的目标IMSI不同的行为
        ////////      {
        ////////					"wTotalImsi":XXX		总的IMSI数（此版本忽略）				
        ////////					"bIMSI_#n#":XXX	        IMSI数组。0~9	配置/删除/查询的IMSI
        ////////					"bUeActionFlag_#n#":XXX 目标IMSI对应的动作。1 = Reject；5 = Hold ON	
        ////////       }
        ////////    ],
        //"n_dic":
        //   [
        //       "name":"TIME_CONTROL",            // 2018-08-20
        //      {
        //                  "activeTime1Start":"09:30:00"  生效时间1的起始时间
        //                  "activeTime1Ended":"12:30:00"  生效时间1的结束时间
        //                  "activeTime2Start":"13:30:00"  生效时间2的起始时间
        //                  "activeTime2Ended":"14:30:00"  生效时间2的结束时间
        //                  "activeTime3Start":"16:30:00"  生效时间3的起始时间，有的话就添加该项
        //                  "activeTime3Ended":"18:30:00"  生效时间3的结束时间，有的话就添加该项
        //                  "activeTime4Start":"20:30:00"  生效时间4的起始时间，有的话就添加该项
        //                  "activeTime4Ended":"22:30:00"  生效时间4的结束时间，有的话就添加该项
        //       }
        //    ],
        //}
        public const string app_get_GCInfo_Response = "app_get_GCInfo_Response";



        //
        //  app获取截获的号码信息请求(用于HJT GSM/GSM-V2/CDMA)
        //"type":"app_get_MsCall_Request" 
        //"dic":
        //    {
        //   "parentFullPathName":"设备.深圳.福田.中心广场.西北监控",   //为""时表示所有的设备
        //   "name":"GSM-Name"                                    //GSM的名称，为""时表示parentFullPathName下所有的设备
        //   "carry":"0"                                          //GSM的载波标识，"0"或者"1",CDMA时为"-1"
        //   "imsi":"46000xxxxxxxxx",                             //指定要搜索的IMSI号，不指定是为""
        //   "number":"138xxxxxxx",                               //指定要搜索的手机号，不指定是为""
        //   "timeStart":"2018-09-26 12:34:56",                   //开始时间，不指定是为""
        //   "timeEnded":"2018-95-27 12:34:56",                   //结束时间，不指定是为""
        //}
        public const string app_get_MsCall_Request = "app_get_MsCall_Request";

        //  
        //   获取下一页的请求
        //"dic":
        //    {
        //    "CurPageIndex":"1:50",
        //    }
        //}
        public const string app_get_MsCall_NextPage_Request = "app_get_MsCall_NextPage_Request";

        //
        //  app获取截获的号码信息响应(用于HJT GSM/GSM-V2/CDMA)
        //"type":"app_get_MsCall_Response" 
        //"dic":
        //    {
        //    "ReturnCode": 返回码：0,成功；其它值为失败
        //    "ReturnStr": 失败原因值。ReturnCode不为0时有意义
        //    "parentFullPathName":"设备.深圳.福田.中心广场.西北监控",
        //    "name":"GSM-Name",
        //    "carry":"0"        
        //    "TotalRecords":"10000",
        //    "CurPageIndex":"1:50",
        //    "PageSize":"200",
        //    "n_dic":[
        //      {
        //         "name":"1",
        //         "dic":{
        //          "imsi":IMSI
        //          "name":西北监控.GSM-Name //2018-10-11
        //          "number":手机号
        //          "time":时间  
        //       }，
        //      {
        //         "name":"2",
        //         "dic":{
        //          "imsi":IMSI
        //          "name":西北监控.GSM-Name //2018-10-11
        //          "number":手机号
        //          "time":时间  
        //       }，
        //       ...
        //}
        public const string app_get_MsCall_Response = "app_get_MsCall_Response";

        //  
        //   app获取截获的号码信息,导出csv文件的请求
        //"dic":
        //    {
        //    "fileName":"abc.csv", //将当前的历史记录搜索导出为abc.csv文件
        //
        //}
        public const string app_get_MsCall_ExportCSV_Request = "app_get_MsCall_ExportCSV_Request";

        //  
        //   app获取截获的号码信息,导出csv文件的响应
        //
        //"type":"app_get_MsCall_ExportCSV_Response"   
        //   
        //"dic":
        //    {
        //    "ReturnCode": 0，            //返回码：0,成功；其它值为失败
        //    "ReturnStr": "成功"，         //失败原因值。ReturnCode不为0时有意义
        //    "ftpUsrName":"root",       
        //    "ftpPwd":"root",
        //    "ftpRootDir": "updaeFile",
        //    "ftpServerIp": "172.17.0.210",
        //    "ftpPort":"21",
        //    "fileName":"abc.csv",
        //}
        public const string app_get_MsCall_ExportCSV_Response = "app_get_MsCall_ExportCSV_Response";



        //
        //  app获取截获的短信信息请求(用于HJT GSM/GSM-V2/CDMA)
        //"type":"app_get_MsSms_Request" 
        //"dic":
        //    {
        //   "parentFullPathName":"设备.深圳.福田.中心广场.西北监控",   //为""时表示所有的设备
        //   "name":"GSM-Name"                                    //GSM的名称，为""时表示parentFullPathName下所有的设备
        //   "carry":"0"                                          //GSM的载波标识，"0"或者"1",CDMA时为"-1"
        //   "imsi":"46000xxxxxxxxx",                             //指定要搜索的IMSI号，不指定是为""
        //   "number":"138xxxxxxx",                               //指定要搜索的手机号，不指定是为""
        //   "data":"服务器",                                      //指定要搜索的内容，不指定是为""
        //   "timeStart":"2018-09-26 12:34:56",                   //开始时间，不指定是为""
        //   "timeEnded":"2018-95-27 12:34:56",                   //结束时间，不指定是为""
        //}
        public const string app_get_MsSms_Request = "app_get_MsSms_Request";

        //  
        //   获取下一页的请求
        //"dic":
        //    {
        //    "CurPageIndex":"1:50",
        //    }
        //}
        public const string app_get_MsSms_NextPage_Request = "app_get_MsSms_NextPage_Request";

        //
        //  app获取截获的短信信息响应(用于HJT GSM/GSM-V2/CDMA)
        //"type":"app_get_MsSms_Response" 
        //"dic":
        //    {
        //    "ReturnCode": 返回码：0,成功；其它值为失败
        //    "ReturnStr": 失败原因值。ReturnCode不为0时有意义
        //    "parentFullPathName":"设备.深圳.福田.中心广场.西北监控",
        //    "name":"GSM-Name",
        //    "carry":"0"       
        //    "TotalRecords":"10000",
        //    "CurPageIndex":"1:50",
        //    "PageSize":"200",
        //    "n_dic":[
        //      {
        //         "name":"1",
        //         "dic":{
        //          "imsi":IMSI
        //          "name":西北监控.GSM-Name //2018-10-11
        //          "number":手机号
        //          "codetype":编码类型
        //          "data":短信内容
        //          "time":时间  
        //       }，
        //      {
        //         "name":"2",
        //         "dic":{
        //          "imsi":IMSI
        //          "name":西北监控.GSM-Name //2018-10-11
        //          "number":手机号
        //          "codetype":编码类型
        //          "data":短信内容
        //          "time":时间  
        //       }，
        //       ...
        //}
        public const string app_get_MsSms_Response = "app_get_MsSms_Response";

        //  
        //   app获取截获的号码信息,导出csv文件的请求
        //"dic":
        //    {
        //    "fileName":"abc.csv", //将当前的历史记录搜索导出为abc.csv文件
        //
        //}
        public const string app_get_MsSms_ExportCSV_Request = "app_get_MsSms_ExportCSV_Request";

        //  
        //   app获取截获的号码信息,导出csv文件的响应
        //
        //"type":"app_get_MsSms_ExportCSV_Response"   
        //   
        //"dic":
        //    {
        //    "ReturnCode": 0，            //返回码：0,成功；其它值为失败
        //    "ReturnStr": "成功"，         //失败原因值。ReturnCode不为0时有意义
        //    "ftpUsrName":"root",       
        //    "ftpPwd":"root",
        //    "ftpRootDir": "updaeFile",
        //    "ftpServerIp": "172.17.0.210",
        //    "ftpPort":"21",
        //    "fileName":"abc.csv",
        //}
        public const string app_get_MsSms_ExportCSV_Response = "app_get_MsSms_ExportCSV_Response";


        #endregion

        #region 重定向相关

        //
        //  app重定向设置的请求
        //"type":"app_set_redirection_request"  
        //"dic":
        //    {
        //   "parentFullPathName":"设备.深圳.福田.中心广场.西北监控",
        //   "name":"电信FDD"
        //   "category":"0"                       //0:white,1:black,2:other
        //   "priority":"2"                       //2:2G,3:3G,4:4G,Others:noredirect
        //   "GeranRedirect":"0"                  //0:disable;1:enable
        //   "arfcn":"2G frequency"               //2G frequency
        //   "UtranRedirect":"0"                  //0:disable;1:enable
        //   "uarfcn":"3G frequency"              //3G frequency
        //   "EutranRedirect":"0"                 //0:disable;1:enable
        //   "earfcn":"4G frequency"              //4G frequency
        //   "RejectMethod":"1"                   //1,2,0xFF,0x10-0xFE
        //   "additionalFreq":"uarfcn1,uarfcn2"   //不超过7个freq，超过7个freq的默认丢弃
        //}
        //
        public const string app_set_redirection_request = "app_set_redirection_request";

        // 
        // app重定向设置的响应
        //    {     
        // result:"0",      // 0:SUCCESS ; 1:GENERAL FAILURE;2:CONFIGURATION FAIURE OR NOT SUPPORTED
        // rebootflag:"1",	// 1—立刻reboot,2—需要reboot
        // timestamp:"xxx"  // Time in seconds when send this message, start from 00:00:00 UTC 1
        //                  // 970, can be convert to local time.
        //}
        public const string app_set_redirection_response = "app_set_redirection_response";

        //
        //  app重定向获取的请求
        //"type":"app_get_redirection_request"  
        //"dic":
        //    {
        //   "parentFullPathName":"设备.深圳.福田.中心广场.西北监控",
        //   "name":"电信FDD"
        //   "category":"0"     //0:white,1:black,2:other 
        //                      //3:all,返回所有，2018-09-28
        //}
        //
        public const string app_get_redirection_request = "app_get_redirection_request";


        // 
        // app重定向获取的响应
        //    {    
        //   "ReturnCode": 返回码：0,成功；其它值为失败
        //   "ReturnStr": 失败原因值。ReturnCode不为0时有意义

        //   "domainId":"2",       
        //   "domainParentId":"1",
        //   "parentFullPathName":"设备.深圳.福田.中心广场.西北监控",
        //   "name":"电信FDD",
        //
        //     "n_dic":[         
        //      {
        //         "name":"0",
        //         "dic":{
        //          "category":"0"                       //0:white,1:black,2:other  
        //          "priority":"2"                       //2:2G,3:3G,4:4G,Others:noredirect
        //          "GeranRedirect":"0"                  //0:disable;1:enable
        //          "arfcn":"2G frequency"               //2G frequency
        //           "UtranRedirect":"0"                  //0:disable;1:enable
        //          "uarfcn":"3G frequency"              //3G frequency
        //          "EutranRedirect":"0"                 //0:disable;1:enable
        //          "earfcn":"4G frequency"              //4G frequency
        //          "RejectMethod":"1"                   //1,2,0xFF,0x10-0xFE
        //          "additionalFreq":"uarfcn1,uarfcn2"   //不超过7个freq，超过7个freq的默认丢弃
        //      }
        //     "n_dic":[         
        //      {
        //         "name":"1",
        //         "dic":{
        //          "category":"1"                       //0:white,1:black,2:other  
        //          "priority":"2"                       //2:2G,3:3G,4:4G,Others:noredirect
        //          "GeranRedirect":"0"                  //0:disable;1:enable
        //          "arfcn":"2G frequency"               //2G frequency
        //           "UtranRedirect":"0"                  //0:disable;1:enable
        //          "uarfcn":"3G frequency"              //3G frequency
        //          "EutranRedirect":"0"                 //0:disable;1:enable
        //          "earfcn":"4G frequency"              //4G frequency
        //          "RejectMethod":"1"                   //1,2,0xFF,0x10-0xFE
        //          "additionalFreq":"uarfcn1,uarfcn2"   //不超过7个freq，超过7个freq的默认丢弃
        //      }
        //}
        public const string app_get_redirection_response = "app_get_redirection_response";

        #endregion

        #region 透传消息，用于工程设置

        /// <summary>
        /// APP向设备发送工程模式的透传消息
        /// </summary>
        //"dic":
        //    {
        //      "transparent_msg":消息内容(LTE为XML格式消息，GSM为16进制)
        //    }
        public const string transparent_msg_request = "transparent_msg_request";

        /// <summary>
        /// 设备向APP发送的透传消息
        /// </summary>
        //"dic":
        //    {
        //      "transparent_msg":消息内容(LTE为XML格式消息，GSM为16进制)
        //    }
        public const string transparent_msg_response = "transparent_msg_response";

        #endregion

        #region 服务器配置操作

        // 2018-11-19        
        //
        //  
        //   app获取服务器各种配置的请求
        //"dic":
        //    {
        //
        //}
        public const string app_get_ServerConfig_Request = "app_get_ServerConfig_Request";

        //  
        //   app获取服务器各种配置请求的响应(共18项)
        //
        //"type":"app_get_ServerConfig_Response"   
        //   
        //"dic":
        //    {
        //    "ReturnCode": 0，                   //返回码：0,成功；其它值为失败
        //    "ReturnStr": "成功"，                //失败原因值。ReturnCode不为0时有意义
        //    "strDbIpAddr":"127.0.0.1",          //数据库IP地址    
        //    "logOutputLevel": "1",              //DEBG = "0", INFO = "1",WARN = "2", EROR = "3"
        //    "strFtpIpAddr":"127.0.0.1",         //FTP服务器IP地址
        //    "strFtpUserId": "ftpuser",          //FTP用户名
        //    "strFtpUserPsw":"ftpuser",          //FTP用户密码
        //    "strFtpPort":"21",                  //FTP端口
        //    "strFtpUpdateDir": "Update",        //FTP的更新路径
        //    "strStartPortCDMA_ZYF": "14783",    //CDMA，ZYF的端口
        //    "strStartPortGSM_ZYF": "14784",     //GSM，ZYF的端口
        //    "strStartPortGSM_HJT": "14785",     //GSM，HJT的端口
        //    "strStartPortLTE": "14786",         //LTE的端口
        //    "strStartPortTDS": "14787",         //TDS的端口
        //    "strStartPortWCDMA": "14788",       //WCDMA的端口
        //    "strStartPortAppWindows": "14789",  //Windows APP的端口
        //    "strStartPortAppLinux": "14790",    //Linux APP的端口
        //    "strStartPortAppAndroid": "14791",  //Android APP的端口
        //    "dataAlignMode": "1",               //数据对齐基准:"0"数据库为基准，"1"以Ap为基准
        //    "logMaxSize": "10",                 //每个Log文件的大小，单位为MB
        //}
        public const string app_get_ServerConfig_Response = "app_get_ServerConfig_Response";


        //  
        //   app设置服务器各种配置的请求(可以只设置一部分)
        //
        //"type":"app_set_ServerConfig_Request"   
        //       
        //"dic":
        //    {
        //    "strDbIpAddr":"127.0.0.1",          //数据库IP地址    
        //    "logOutputLevel": "1",              //DEBG = "0", INFO = "1",WARN = "2", EROR = "3"
        //    "strFtpIpAddr":"127.0.0.1",         //FTP服务器IP地址
        //    "strFtpUserId": "ftpuser",          //FTP用户名
        //    "strFtpUserPsw":"ftpuser",          //FTP用户密码
        //    "strFtpPort":"21",                  //FTP端口
        //    "strFtpUpdateDir": "Update",        //FTP的更新路径
        //    "strStartPortCDMA_ZYF": "14783",    //CDMA，ZYF的端口
        //    "strStartPortGSM_ZYF": "14784",     //GSM，ZYF的端口
        //    "strStartPortGSM_HJT": "14785",     //GSM，HJT的端口
        //    "strStartPortLTE": "14786",         //LTE的端口
        //    "strStartPortTDS": "14787",         //TDS的端口
        //    "strStartPortWCDMA": "14788",       //WCDMA的端口
        //    "strStartPortAppWindows": "14789",  //Windows APP的端口
        //    "strStartPortAppLinux": "14790",    //Linux APP的端口
        //    "strStartPortAppAndroid": "14791",  //Android APP的端口
        //    "dataAlignMode": "1",               //数据对齐基准:"0"数据库为基准，"1"以Ap为基准
        //    "logMaxSize": "10",                 //每个Log文件的大小，单位为MB
        //}
        public const string app_set_ServerConfig_Request = "app_set_ServerConfig_Request";


        //  
        //   app设置服务器各种配置请求的效应
        // 
        //"type":"app_set_ServerConfig_Response"   
        //   
        //"dic":
        //    {
        //    "ReturnCode": 0，            //返回码：0,成功；其它值为失败
        //    "ReturnStr": "成功"，         //失败原因值。ReturnCode不为0时有意义
        //}
        public const string app_set_ServerConfig_Response = "app_set_ServerConfig_Response";

        #endregion

        #region 批量导入导出

        // 2018-11-19        
        //  
        //   app获取批量导出配置的请求
        //"dic":
        //    {
        //    "fileName":"abc.txt", //将当前的数据库各种配置导出到abc.txt文件中
        //
        //}
        public const string app_get_BIE_ExportConfig_Request = "app_get_BIE_ExportConfig_Request";

        //  
        //   app获取批量导出配置请求的响应
        //
        //"type":"app_get_BIE_ExportConfig_Response"   
        //   
        //"dic":
        //    {
        //    "ReturnCode": 0，            //返回码：0,成功；其它值为失败
        //    "ReturnStr": "成功"，         //失败原因值。ReturnCode不为0时有意义
        //    "ftpUsrName":"root",       
        //    "ftpPwd":"root",
        //    "ftpRootDir": "updaeFile",
        //    "ftpServerIp": "172.17.0.210",
        //    "ftpPort":"21",
        //    "fileName":"abc.txt",
        //}
        public const string app_get_BIE_ExportConfig_Response = "app_get_BIE_ExportConfig_Response";

        //  
        //   app设置批量导入配置的请求2
        //"dic":
        //    {
        //    "fileName":"abc.txt", //上传到服务器的文件
        //
        //}
        public const string app_set_BIE_ImportConfig_Request = "app_set_BIE_ImportConfig_Request";

        //  
        //   app获取批量导入配置请求2的响应
        // 
        //"type":"app_get_BIE_ExportConfig_Response"   
        //   
        //"dic":
        //    {
        //    "ReturnCode": 0，            //返回码：0:导入abc.txt到数据库成功，
        //                                 //      其他:导入abc.txt到数据库失败
        //    "ReturnStr": "成功"，         //失败原因值。ReturnCode不为0时有意义
        //}
        public const string app_set_BIE_ImportConfig_Response = "app_set_BIE_ImportConfig_Response";

        #endregion        

        #region 地图相关

        //
        //  获取站点的地理位置
        //"type":"app_get_station_location_request",
        //"dic":
        //    {
        //    "name":"南山",
        //    "parentNameFullPath":"设备.深圳",
        //}
        public const string app_get_station_location_request = "app_get_station_location_request";

        //
        //  获取站点的地理位置响应
        //"dic":
        //    {
        //    "ReturnCode": 返回码：0,成功；其它值为失败
        //    "ReturnStr": 失败原因值。ReturnCode不为0时有意义
        //    "name":"南山",
        //    "parentNameFullPath":"设备.深圳",
        //    "longitude":"114.06667",
        //    "latitude":"22.61667",
        //}
        public const string app_get_station_location_response = "app_get_station_location_response";

        //  
        //   设置站点的地理位置
        //"dic":
        //    {
        //    "name":"南山",
        //    "parentNameFullPath":"设备.深圳",
        //    "longitude":"114.06667",
        //    "latitude":"22.61667",
        //}
        public const string app_set_station_location_request = "app_set_station_location_request";

        //  
        //   设置站点的地理位置的响应
        // 
        //"type":"app_set_station_location_response"   
        //   
        //"dic":
        //    {
        //    "ReturnCode": 返回码：0,成功；其它值为失败
        //    "ReturnStr":  失败原因值。ReturnCode不为0时有意义
        //    "name":"南山",
        //    "parentNameFullPath":"设备.深圳",
        //}
        public const string app_set_station_location_response = "app_set_station_location_response";

        //
        //  获取统计信息(分时或实时)
        //"type":"app_get_statistics_request",
        //"dic":
        //    {     
        //   "timeStart":"2018-05-23 12:34:56",                      //开始时间，不指定是为""
        //   "timeEnded":"2018-05-29 12:34:56",                      //结束时间，不指定是为""
        //   "deviceCount":"3",                                      //设备列表个数
        //   "deviceFullPathName1":"设备.深圳.西北监控.电信TDD1",        //设备1的全路径   
        //   "deviceFullPathName2":"设备.深圳.西北监控.电信TDD2",        //设备2的全路径   
        //   "deviceFullPathName3":"设备.深圳.西北监控.电信TDD3",        //设备3的全路径   
        //   "bwFlag":"black",                                       //用户类型，black,white,other,不指定是为""
        //   "operators":"移动",                                      //运营商，移动，联通，电信,不指定是为""
        //},
        public const string app_get_statistics_request = "app_get_statistics_request";

        //
        //   获取统计信息(分时或实时)的响应
        //"dic":
        //    {
        //    "ReturnCode": 返回码：0,成功；其它值为失败
        //    "ReturnStr": 失败原因值。ReturnCode不为0时有意义
        //    "ImsiTotal":"10000",       //IMSI总个数(没有去重)
        //    "ImsiTotalRmDup":"9000",   //IMSI总个数(已经去重)
        //    "queryTime":"123ms"        //查询时间，单位毫秒 
        //}
        public const string app_get_statistics_response = "app_get_statistics_response";

        //
        //  获取轨迹统计
        //"type":"app_get_imsi_path_request",
        //"dic":
        //    {     
        //   "timeStart":"2018-05-23 12:34:56",        //开始时间，不指定是为""
        //   "timeEnded":"2018-05-29 12:34:56",        //结束时间，不指定是为""
        //   "imsi":"46000xxxxxxxxx",                  //指定要进行轨迹统计的IMSI号
        //},
        public const string app_get_imsi_path_request = "app_get_imsi_path_request";

        //
        //   获取轨迹统计的响应
        //"dic":
        //    {
        //    "ReturnCode": 返回码：0,成功；其它值为失败
        //    "ReturnStr": 失败原因值。ReturnCode不为0时有意义
        //    "WarnInfo": "警告信息"                  //警告信息
        //    "queryTime":"123ms"                    //查询时间，单位毫秒 
        //    "stationCount":"3",                    //抓取到该IMSI的站点个数
        //
        //    "nameFullPath1":"设备.深圳.西北监控1",    //站点1的全名
        //    "longitude1":"114.06667",              //站点1的经度
        //    "latitude1":"22.61667",                //站点1的纬度
        //
        //    "nameFullPath2":"设备.深圳.西北监控1",    //站点2的全名
        //    "longitude2":"114.06667",              //站点2的经度
        //    "latitude2":"22.61667",                //站点2的纬度
        //
        //    "nameFullPath3":"设备.深圳.西北监控1",    //站点3的全名
        //    "longitude3":"114.06667",              //站点3的经度
        //    "latitude3":"22.61667",                //站点4的纬度
        //}
        public const string app_get_imsi_path_response = "app_get_imsi_path_response";

        //
        //  获取常驻人口，阈值是可以设置值，默认是70%
        //"type":"app_get_resident_imsi_list_request",
        //"dic":
        //    {     
        //   "timeStart":"2018-05-23 12:34:56",                  //开始时间，以天为粒度
        //   "timeEnded":"2018-05-29 12:34:56",                  //结束时间，范围为[2,30]
        //   "deviceFullPathName":"设备.深圳.西北监控.电信TDD",      //设备的全路径   
        //},
        public const string app_get_resident_imsi_list_request = "app_get_resident_imsi_list_request";

        //
        //   获取常驻人口的响应
        //"dic":
        //    {
        //    "ReturnCode": 返回码：0,成功；其它值为失败
        //    "ReturnStr": 失败原因值。ReturnCode不为0时有意义
        //    "queryTime1":"123ms"         //查询时间，单位毫秒 
        //    "queryTime2":"1230ms"        //计算时间，单位毫秒 
        //    "imsiTotal":"1000",          //IMSI的总个数 -- 2019-01-28
        //    "imsiCount":"3",             //常驻IMSI个数
        //    "imsi1":"46000xxxxxxxxx",    //第1个IMSI
        //    "imsi2":"46000xxxxxxxxx",    //第2个IMSI
        //    "imsi3":"46000xxxxxxxxx",    //第3个IMSI
        //}
        public const string app_get_resident_imsi_list_response = "app_get_resident_imsi_list_response";

        //
        //  获取碰撞的IMSI列表
        //"type":"app_get_collision_imsi_list_request",
        //"dic":
        //    {     
        //   "groupCount":"4",                                   //条件组的个数，范围为[2,4]组
        //   "timeStart1":"2018-05-23 12:34:56",                 //开始时间1
        //   "timeEnded1":"2018-05-29 12:34:56",                 //结束时间1       
        //   "deviceFullPathName1":"设备.深圳.西北监控.电信TDD1",    //设备1的全路径   

        //   "timeStart2":"2018-05-23 12:34:56",                 //开始时间2
        //   "timeEnded2":"2018-05-29 12:34:56",                 //结束时间2      
        //   "deviceFullPathName2":"设备.深圳.西北监控.电信TDD2",    //设备2的全路径

        //   "timeStart3":"2018-05-23 12:34:56",                 //开始时间3
        //   "timeEnded3":"2018-05-29 12:34:56",                 //结束时间3     
        //   "deviceFullPathName3":"设备.深圳.西北监控.电信TDD3",    //设备3的全路径

        //   "timeStart4":"2018-05-23 12:34:56",                 //开始时间4
        //   "timeEnded4":"2018-05-29 12:34:56",                 //结束时间4    
        //   "deviceFullPathName4":"设备.深圳.西北监控.电信TDD4",    //设备4的全路径
        //},
        public const string app_get_collision_imsi_list_request = "app_get_collision_imsi_list_request";

        //
        //   获取碰撞的IMSI列表的响应
        //"dic":
        //    {
        //    "ReturnCode": 返回码：0,成功；其它值为失败
        //    "ReturnStr": 失败原因值。ReturnCode不为0时有意义
        //    "queryTime1":"123ms"         //查询时间，单位毫秒 
        //    "queryTime2":"1230ms"        //计算时间，单位毫秒 
        //    "imsiCount":"3",             //碰撞IMSI个数
        //    "imsi1":"46000xxxxxxxxx",    //第1个IMSI
        //    "imsi2":"46000xxxxxxxxx",    //第2个IMSI
        //    "imsi3":"46000xxxxxxxxx",    //第3个IMSI
        //}
        public const string app_get_collision_imsi_list_response = "app_get_collision_imsi_list_response";

        //
        //  获取指定IMSI是否为伴随IMSI，阈值是可以设置值，默认是70%
        //"type":"app_get_accompany_request",
        //"dic":
        //    {     
        //   "imsi":"46000xxxxxxxxx",                                //指定的IMSI
        //   "timeStart":"2018-05-23 12:34:56",                      //开始时间
        //   "timeEnded":"2018-05-29 12:34:56",                      //结束时间,范围最大7天
        //   "timeWindow":"10",                                      //时间窗口，10分钟
        //},
        public const string app_get_accompany_request = "app_get_accompany_request";

        //
        //   获取指定IMSI是否为伴随IMSI的响应
        //"dic":
        //    {
        //    "ReturnCode": 返回码：0,成功；其它值为失败
        //    "ReturnStr": 失败原因值。ReturnCode不为0时有意义
        //    "queryTime1":"123ms"          //查询时间，单位毫秒 
        //    "queryTime2":"1230ms"         //计算时间，单位毫秒 
        //    "imsiCount":"3",              //伴随IMSI个数
        //    "imsi1":"46000xxxxxxxxx",     //第1个IMSI
        //    "imsi2":"46000xxxxxxxxx",     //第2个IMSI
        //    "imsi3":"46000xxxxxxxxx",     //第3个IMSI
        //}
        public const string app_get_accompany_response = "app_get_accompany_response";

        #endregion
    }

    public class Main2ApControllerMsgType : AppMsgType
    {
        /// <summary>
        /// MainCtrl每隔一段时间(10分钟)就向所有的AP发该消息，用于确认AP和数据库中的上下线状态是否
        /// 一致，如果不一致的话，ApCtrl就使用OnOffLine消息上报状态给MainCtrl，也即以AP的消息为准.
        /// 
        /// MainController-->ApController),2018-08-07
        /// 
        /// </summary>
        //"dic":
        //{
        //    "Status":"OnLine"   //"OnLine"或"OffLine"
        //}
        public const string OnOffLineCheck = "OnOffLineCheck";


        /// <summary>
        /// AP上下线通知消息 (ApController-->MainController)
        /// </summary>
        //"dic":
        //{
        //    "AllOnLineNum": 1（当前在线设备总台数）
        //    "Status":OnLine：上线；OffLine:下线
        //    "mode":"xxx"     //GSM,TD-SCDMA,WCDMA,LTE-TDD,LTE-FDD,2018-06-28
        //    "version":"xxx"  //ap的版本信息,2018-08-17
        //    "timestamp"   时间戳
        //}

        public const string OnOffLine = "ApOnOffLine";

        /// <summary>
        /// AP上下线通知消息回复 (MainController-->ApController)
        /// </summary>
        //"dic":
        //{
        //    "ReturnCode": 返回码：0,成功；其它值为失败
        //    "ReturnStr": 失败原因值。ReturnCode不为0时有意义
        //    "Status":更新后，数据库保存的AP在线状态。只有回复ReturnCode为0时，这个字段才有意义。
        //             OnLine：上线；OffLine:下线
        //}
        public const string OnOffLine_Ack = "ApOnOffLine_Ack";

        /// <summary>
        /// 所有在线消息 (ApController-->MainController)
        /// </summary>
        public const string OnLineAPList = "OnLineAPList";

        /// <summary>
        /// AP状态改变通知 (ApController-->MainController)
        /// </summary>
        /// 
        /// 2018-08-03
        /// WCDMA/LTE-TDD/LTE-FDD对应数据库表ap_status
        /// GSM-V2/CDMA对应数据库表gc_misc
        /// GSM目前没有上报这个消息
        ///
        //"dic":
        //{        
        //    "carry":     "载波信息 0/1" 
        //    "SCTP":       SCTP连接状态：1,正常；0，不正常
        //    "S1":         S1连接状态：1,正常；0，不正常
        //    "GPS":        GPS连接状态：1,正常；0，不正常
        //    "CELL":       CELL状态：1,正常；0，不正常
        //    "SYNC":       同步状态：1,正常；0，不正常
        //    "LICENSE":    LICENSE状态：1,正常；0，不正常
        //    "RADIO":      射频状态：1,正常；0，不正常
        //    "ALIGN":      对齐状态：1,已经对齐；0，尚未对齐，2018-11-05
        //    "timestamp"   时间戳
        //    "wSelfStudy"  "0"      //"0"正常状态，"1"只学习状态 ，2018-07-19
        //                           // 2018-08-09
        //    "ApReadySt"   "XML-Not-Ready"     //0-"XML-Not-Ready"
        //                                      //1-"XML-Ready"
        //                                      //2-"Sniffering"
        //                                      //3-"Ready-For-Cell"
        //                                      //4-"Cell-Setuping"
        //                                      //5-"Cell-Ready"
        //                                      //6-"Cell-Failure"
        //    "detail"      : "0x3000000"       //16进制字符串
        //}
        public const string ApStatusChange = "ApStatusChange";


        /// <summary>
        /// AP状态改变通知 (MainController-->ApController)
        /// </summary>
        /// 
        /// 2018-08-16
        ///
        #region LTE/WCDMA产品
        //"dic":
        //{     
        //    "active_mode":   "1(start), 2(stop)，3(reboot,本消息不用)"
        //    "mode":          "1:scanner mode 2: audit mode"
        //}
        #endregion
        #region GSM_HJT产品
        //"dic":
        //{     
        //    "carry":          载波信息 0/1
        //    "rfEnable":       射频使能(0,关闭；1，打开)
        //	  "rfFreq":         信道号
        //	  "rfPwr":          发射功率衰减值
        //}
        #endregion
        #region GSM/CDMA_ZYF产品
        //"dic":
        //{     
        //    "carry":     "载波信息 0/1" ,CDMA为"0"
        //    "RADIO":     射频使能(0,关闭；1，打开)
        //}
        #endregion
        public const string ApSetRadio = "ApSetRadio";


        /// <summary>
        /// 界面删除设备通知 (MainController-->ApController)
        /// </summary>
        /// 
        /// 2018-08-16
        ///
        public const string ApDelete = "ApDelete";


        /// <summary>
        /// AP状态改变回复 (MainController-->ApController)
        /// </summary>
        //"dic":
        //{
        //    "carry"       : 载波信息 0/1
        //    "ReturnCode"  : 返回码：0,成功；其它值为失败
        //    "ReturnStr"   : 失败原因值。ReturnCode不为0时有意义
        //    "detail"      : "0x3000000"   //16进制字符串
        //    "ApReadySt"   : "XML-Not-Ready"
        //}
        public const string ApStatusChange_Ack = "ApStatusChange_Ack";

        /// <summary>
        /// MainController透传消息
        /// </summary>
        public const string Transparent = "Transparent";

        //
        // AP参数上报 (ApController-->MainController)
        //"type":"ReportGenPara"    （跟据产品类型，选择其中一种通用参数）
        #region LTE通用参数
        //"dic":  
        //    {
        //          "reportType":"change或report" change:表示以下参数有修改。report：表示上报所有参数(用于数据对齐)
        //          "whiteimsi_md5":"xxx"  白名单MD5校验值。"reportType"为“report”时才有该项
        //          "blackimsi_md5":"xxx"  黑名单MD5校验值。"reportType"为“report”时才有该项
        //          "mode":"GSM",          设备工作制式：GSM、TDD-LTE、FDD-LTE等
        //          "primaryplmn:"xxx",    PLMN
        //          "earfcndl:"xxx",       工作频点（下行）
        //          "earfcnul:"xxx",       工作频点（上行）
        //          "cellid:"xxx",         cellid,2018-06-26
        //          "pci:"xxx",            工作PCI
        //          "bandwidth:"xxx",      工作Band
        //          "tac:"xxx",            工作Tac
        //          "txpower:"xxx",        发射功率
        //          "periodtac:"xxx",      变换Tac周期
        //          "manualfreq:"xxx",     手动选择工作频点（0：自动选择；1：手动选择）
        //          "bootMode:"0",         设备启动方式（0：半自动。此时需要发送Active命令才开始建小区 1：全自动）
        //          "Earfcnlist:"xxx,xxx", REM扫描频点列表
        //          "Bandoffse:"xxxx",     Band频偏值。用于GPS同步时的补偿值
        //          "NTP:"172.17.0.210",   NTP服务器地址
        //          "ntppri:"5",           NTP获取时间的优先级
        //          "source:"0，           同步源（0：GPS ； 1：CNM ； 2：no sync）
        //          "ManualEnable:"1",     是否启动手动选择同步频点功能（0：不启动 ； 1 :启动）
        //          "ManualEarfcn:"xxx",   手动选择的同步频点
        //          "ManualPci:"xxx",      手动选择的同步PCI
        //          "ManualBw:"xxx"        手动选择的同步小区带宽
        //          "gps_select":"0"       GPS配置，0表示NOGPS，1表示GPS
        //                                               2018-07-23
        //          "otherplmn:"xxx,yyy",                多PLMN选项，多个之间用逗号隔开
        //          "periodFreq:"周期:freq1,freq2,freq3"  周期以S表示，0表示不做周期性变换；
        //                                               在freq list中进行循环，此时小区配置中的频点失效
        //      }
        #endregion
        #region GSM_HJT通用参数
        //"dic":
        //    {
        //          "reportType":"change或report" change:表示以下参数有修改（以n_dic有一项或多项）。report：表示上报所有参数(用于数据对齐)
        //          "whiteimsi_md5":"xxx"  白名单MD5校验值。"reportType"为“report”时才有该项 （预留接口，暂无值）
        //          "blackimsi_md5":"xxx"  黑名单MD5校验值。"reportType"为“report”时才有该项  (预留接口，暂无值）
        //          "sys":系统号，0表示系统1或通道1或射频1，1表示系统2或通道2或射频2
        //          "hardware_id":硬件Id
        //    }
        //"n_dic":
        //   [
        //       "name":"RECV_SYS_PARA",                 //4.1   系统参数
        //      {
        //					"paraMcc":移动国家码
        //					"paraMnc":移动网号
        //					"paraBsic":基站识别码
        //					"paraLac":位置区号
        //					"paraCellId":小区ID
        //					"paraC2":C2偏移量
        //					"paraPeri":周期性位置更新周期
        //					"paraAccPwr":接入功率
        //					"paraMsPwr":手机发射功率
        //					"paraRejCau":位置更新拒绝原因
        //       }
        //    ],
        //"n_dic":
        //   [
        //       "name":"RECV_SYS_OPTION",                 //4.2  系统选项
        //      {
        //					"opLuSms":登录时发送短信
        //					"opLuImei":登录时获取IMEI
        //					"opCallEn":允许用户主叫
        //					"opDebug":调试模式，上报信令
        //					"opLuType":登录类型
        //					"opSmsType":短信类型
        //       }
        //    ],
        //"n_dic":
        //   [
        //       "name":"RECV_RF_PARA",                 //4.4	射频参数
        //      {
        //					"rfEnable":射频使能
        //					"rfFreq":信道号
        //					"rfPwr":发射功率衰减值
        //       }
        //    ],
        //"n_dic":   --report时为预留接口，暂不支持 
        //   [
        //       "name":"RECV_SMS_OPTION",               //4.7	短消息中心号码;4.8	短消息原叫号码;4.9 短消息发送时间;4.10  短消息内容
        //       {
        //          "gSmsRpoa":短消息中心号码
        //          "gSmsTpoa":短消息原叫号码
        //          "gSmsScts":短消息发送时间 （时间格式为年/月/日/时/分/秒各两位，不足两位前补0。如2014年4月22日15点46分47秒的消息内容为“140422154647”）
        //          "gSmsData":短消息内容 （编码格式为Unicode编码）
        //          "autoSendtiny":是否自动发送
        //          "autoFilterSMStiny":是否自动过滤短信
        //          "delayTime":发送延时时间
        //          "smsCodingtiny":短信的编码格式
        //         }
        //    ],
        //"n_dic":
        //   [
        //       "name":"RECV_REG_MODE",            //4.33	注册工作模式
        //       {
        //          "regMode":模式0时由设备自行根据系统选项决定是否允许终端入网，是否对终端发送短信；
        //                    模式1时设备将终端标识发送给上位机，由上位机告知设备下一步的动作
        //       }
        //    ];
        #endregion
        #region GSM/CDMA_ZYF通用参数
        //"dic":       --n_dic可以有一个或多个
        //      {
        //          "reportType":"change或report" change:表示以下参数有修改。report：表示上报所有参数(用于数据对齐)
        //          "whiteimsi_md5":"xxx"  白名单MD5校验值。"reportType"为“report”且sys为0时才有该项
        //          "blackimsi_md5":"xxx"  黑名单MD5校验值。"reportType"为“report”且sys为0时才有该项
        //          "sys":系统号，0表示系统1或通道1或射频1，1表示系统2或通道2或射频2
        //    }
        //"n_dic":
        //   [
        //       "name":"CONFIG_FAP_MSG",            //4.4  GUI配置FAP的启动参数
        //       {
        //					"bWorkingMode":XXX		    工作模式:1 为侦码模式 ;3驻留模式.
        //					"bC":XXX		            是否自动切换模式。保留
        //					"wRedirectCellUarfcn":XXX	CDMA黑名单频点
        //					"bPLMNId":XXX		        PLMN标志
        //					"bTxPower":XXX			    实际发射功率.设置发射功率衰减寄存器, 0输出最大功率, 每增加1, 衰减1DB
        //					"bRxGain":XXX			    接收信号衰减寄存器. 每增加1增加1DB的增益
        //					"wPhyCellId":XXX		    物理小区ID.
        //					"wLAC":XXX			        追踪区域码。GSM：LAC;CDMA：REG_ZONE
        //					"wUARFCN":XXX			    小区频点. CDMA 制式为BSID
        //					"dwCellId":XXX			    小区ID。注意在CDMA制式没有小区ID，高位WORD 是SID ， 低位WORD 是NID
        //       }
        //    ];
        //"n_dic":  --"reportType"为“change”时才有该项
        //   [
        //       "name":"FAP_TRACE_MSG",             //4.7  FAP上报一些事件和状态给GUI，GUI程序需要显示给操作者看。
        //      {
        //					"wTraceLen":XXX	      Trace长度
        //                  "cTrace":XXX          Trace内容
        //       }
        //    ],
        //"n_dic":   ----"reportType"为“change”时才有该项
        //   [
        //       "name":"UE_ORM_REPORT_MSG",        //4.9  FAP上报UE主叫信息，只用于GSM和CDMA
        //      {
        //					"bOrmType":XXX	    	主叫类型。1=呼叫号码, 2=短消息PDU,3=寻呼测量
        //					"bUeId":XXX	     	    IMSI
        //					"cRSRP":XXX	    	    接收信号强度。寻呼测量时，-128表示寻呼失败
        //					"bUeContentLen":XXX	    Ue主叫内容长度
        //					"bUeContent":XXX	    Ue主叫内容。最大249字节。
        //       }
        //    ],
        //"n_dic":
        //   [
        //       "name":"CONFIG_SMS_CONTENT_MSG",           //4.10  FAP 配置下发短信号码和内容
        //      {
        //					"sms_ctrl":XXX	    	        上号后是否自动发送短信。0：不自动发送；1：自动发关  
        //					"bSMSOriginalNumLen":XXX	    主叫号码长度
        //					"bSMSOriginalNum":XXX	    	主叫号码
        //					"bSMSContentLen":XXX	    	短信内容字数
        //					"bSMSContent":XXX	            短信内容.unicode编码，每个字符占2字节
        //       }
        //    ],
        //"n_dic":  --CDMA有该项，GSM-V2没有该项
        //   [
        //       "name":"CONFIG_CDMA_CARRIER_MSG",            //4.14  GUI 配置CDMA多载波参数
        //       {
        //					"wARFCN1":XXX	        工作频点1	
        //					"bARFCN1Mode":XXX	    工作频点1模式。0表示扫描，1表示常开,2表示关闭。
        //					"wARFCN1Duration":XXX	工作频点1扫描时长
        //					"wARFCN1Period":XXX	    工作频点1扫描间隔
        //					"wARFCN2":XXX	        工作频点2
        //					"bARFCN2Mode":XXX	    工作频点2模式。 0表示扫描，1表示常开,2表示关闭。
        //					"wARFCN2Duration":XXX	工作频点2扫描时长
        //					"wARFCN2Period":XXX	    工作频点2扫描间隔
        //					"wARFCN3":XXX	        工作频点3	
        //					"bARFCN3Mode":XXX	    工作频点3模式。 0表示扫描，1表示常开,2表示关闭。
        //					"wARFCN3Duration":XXX	工作频点3扫描时长	
        //					"wARFCN3Period":XXX	    工作频点3扫描间隔
        //					"wARFCN4":XXX	        工作频点4	
        //					"bARFCN4Mode":XXX	    工作频点4模式。	0表示扫描，1表示常开,2表示关闭。
        //					"wARFCN4Duration":XXX	工作频点4扫描时长
        //					"wARFCN4Period":XXX	    工作频点4扫描间隔
        //       }
        //    ];
        //"n_dic":    ----"reportType"为“change”时才有该项
        //   [
        //       "name":"CONFIG_IMSI_MSG_V3_ID",                 //4.17  大数量imsi名单，用于配置不同的目标IMSI不同的行为
        //      {
        //					"wTotalImsi":XXX		总的IMSI数（此版本忽略）
        //					"bIMSINum":n		    本条消息中的IMSI(n<=50)
        //					"bSegmentType":XXX		分段类型。1=First, 2=SubSq, 3=Last, 4=Complete
        //                                          (如果配置/查询的IMSI超过50，使用多个消息来配置， 此时需要填写分段类型) （此版本忽略）
        //					"bSegmentID":XXX		分段ID（此版本忽略）
        //					"bActionType":XXX		动作类型。1 = Delete All IMSI；2 = Delete Special IMSI；3 = Add IMSI；4 = Query IMSI
        //					"bIMSI_#n#":XXX	        IMSI数组。0~9	配置/删除/查询的IMSI
        //					"bUeActionFlag_#n#":XXX 目标IMSI对应的动作。1 = Reject；5 = Hold ON	
        //       }
        //    ],
        #endregion
        public const string ReportGenPara = "ReportGenPara";

        //  
        //AP参数上报响应(保存到数据库中) (MainController-->ApController)
        //"dic":
        //{
        //    "ReturnCode": 返回码：0,成功；其它值为失败
        //    "ReturnStr": 失败原因值。ReturnCode不为0时有意义
        //    "sys":系统号，0表示系统1或通道1或射频1，1表示系统2或通道2或射频2(无此字段时，表示sys为0)
        //}
        public const string ReportGenParaAck = "ReportGenParaAck";

        //
        // 设置通用参数 (MainController-->ApController)
        //"type":"SetGenParaReq"   （跟据产品类型，选择其中一种通用参数）
        #region LTE通用参数
        //"dic": 
        //    {
        //          "ApIsBase":"xxx"       对齐基准。0表示数据库为基准，1表示以Ap为基准
        //          "FtpUrl_White":"xxx"   白名单文件FTP地址
        //          "FtpUrl_Black":"xxx"   黑名单文件FTP地址
        //          "FtpUser":"xxx"        FTP用户名
        //          "FtpPas":"xxx"         FTP密码
        //          "mode":"GSM",          设备工作制式：GSM、TDD-LTE、FDD-LTE等
        //          "primaryplmn:"xxx",    PLMN
        //          "earfcndl:"xxx",       工作频点（下行）
        //          "earfcnul:"xxx",       工作频点（上行）
        //          "cellid:"xxx",         cellid,2018-06-26
        //          "pci:"xxx",            工作PCI
        //          "bandwidth:"xxx",      工作Band
        //          "tac:"xxx",            工作Tac
        //          "txpower:"xxx",        发射功率
        //          "periodtac:"xxx",      变换Tac周期
        //          "manualfreq:"xxx",     手动选择工作频点（0：自动选择；1：手动选择）
        //          "bootMode:"0",         设备启动方式（0：半自动。此时需要发送Active命令才开始建小区 1：全自动）
        //          "Earfcnlist:"xxx,xxx", REM扫描频点列表
        //          "Bandoffse:"xxxx",     Band频偏值。用于GPS同步时的补偿值
        //          "NTP:"172.17.0.210",   NTP服务器地址
        //          "ntppri:"5",           NTP获取时间的优先级
        //          "source:"0，           同步源（0：GPS ； 1：CNM ； 2：no sync）
        //          "ManualEnable:"1",     是否启动手动选择同步频点功能（0：不启动 ； 1 :启动）
        //          "ManualEarfcn:"xxx",   手动选择的同步频点
        //          "ManualPci:"xxx",      手动选择的同步PCI
        //          "ManualBw:"xxx"        手动选择的同步小区带宽
        //          "gps_select":"0"       GPS配置，0表示NOGPS，1表示GPS    
        //                                 2018-07-23
        //          "otherplmn:"xxx,yyy",  多PLMN选项，多个之间用逗号隔开
        //          "periodFreq:"{周期:freq1,freq2,freq3}"  周期以S表示，0表示不做周期性变换；
        //                                                 在freq list中进行循环，此时小区配置中的频点失效
        //      }
        #endregion
        #region  GSM_HJT通用参数
        //"dic":       --n_dic可以有一个或多个
        //      {
        //          "ApIsBase":"xxx"       对齐基准。0表示数据库为基准，1表示以Ap为基准
        //          "FtpUrl_White":"xxx"   白名单文件FTP地址  （预留接口，暂不支持）
        //          "FtpUrl_Black":"xxx"   黑名单文件FTP地址  （预留接口，暂不支持）
        //          "FtpUser":"xxx"        FTP用户名           （预留接口，暂不支持）
        //          "FtpPas":"xxx"         FTP密码            （预留接口，暂不支持）
        //          "sys":系统号，0表示系统1或通道1或射频1，1表示系统2或通道2或射频2
        //    }
        //"n_dic":
        //   [
        //       "name":"RECV_SYS_PARA",                 //4.1   系统参数
        //      {
        //					"paraMcc":移动国家码
        //					"paraMnc":移动网号
        //					"paraBsic":基站识别码
        //					"paraLac":位置区号
        //					"paraCellId":小区ID
        //					"paraC2":C2偏移量
        //					"paraPeri":周期性位置更新周期
        //					"paraAccPwr":接入功率
        //					"paraMsPwr":手机发射功率
        //					"paraRejCau":位置更新拒绝原因
        //       }
        //    ],
        //"n_dic":
        //   [
        //       "name":"RECV_SYS_OPTION",                 //4.2  系统选项
        //      {
        //					"opLuSms":登录时发送短信
        //					"opLuImei":登录时获取IMEI
        //					"opCallEn":允许用户主叫
        //					"opDebug":调试模式，上报信令
        //					"opLuType":登录类型
        //					"opSmsType":短信类型
        //       }
        //    ],
        //"n_dic":
        //   [
        //       "name":"RECV_RF_PARA",                 //4.4	射频参数
        //      {
        //					"rfEnable":射频使能
        //					"rfFreq":信道号
        //					"rfPwr":发射功率衰减值
        //       }
        //    ],
        //"n_dic":   
        //   [
        //       "name":"RECV_SMS_OPTION",               //4.7	短消息中心号码;4.8	短消息原叫号码;4.9 短消息发送时间;4.10  短消息内容
        //       {
        //          "gSmsRpoa":短消息中心号码
        //          "gSmsTpoa":短消息原叫号码
        //          "gSmsScts":短消息发送时间 （时间格式为年/月/日/时/分/秒各两位，不足两位前补0。如2014年4月22日15点46分47秒的消息内容为“140422154647”）
        //          "gSmsData":短消息内容 （编码格式为Unicode编码）
        //          "autoSendtiny":是否自动发送
        //          "autoFilterSMStiny":是否自动过滤短信
        //          "delayTime":发送延时时间
        //          "smsCodingtiny":短信的编码格式
        //         }
        //    ],
        //"n_dic":
        //   [
        //       "name":"RECV_REG_MODE",            //4.33	注册工作模式
        //       {
        //          "regMode":模式0时由设备自行根据系统选项决定是否允许终端入网，是否对终端发送短信；
        //                    模式1时设备将终端标识发送给上位机，由上位机告知设备下一步的动作
        //       }
        //    ];
        #endregion
        #region GSM/CDMA_ZYF通用参数
        //"dic":       
        //      {
        //          "ApIsBase":"xxx"       对齐基准。0表示数据库为基准，1表示以Ap为基准
        //          "FtpUrl_White":"xxx"   白名单文件FTP地址  （sys为1时，没有该项）
        //          "FtpUrl_Black":"xxx"   黑名单文件FTP地址  （sys为1时，没有该项）
        //          "FtpUser":"xxx"        FTP用户名           （sys为1时，没有该项）
        //          "FtpPas":"xxx"         FTP密码            （sys为1时，没有该项）
        //          "sys":系统号，0表示系统1或通道1或射频1，1表示系统2或通道2或射频2
        //    }
        //"n_dic":
        //   [
        //       "name":"CONFIG_FAP_MSG",            //4.4  GUI配置FAP的启动参数
        //       {
        //					"bWorkingMode":XXX		    工作模式:1 为侦码模式 ;3驻留模式.
        //					"bC":XXX		            是否自动切换模式。保留
        //					"wRedirectCellUarfcn":XXX	CDMA黑名单频点	
        //					"bPLMNId":XXX		    PLMN标志
        //					"bTxPower":XXX			实际发射功率.设置发射功率衰减寄存器, 0输出最大功率, 每增加1, 衰减1DB
        //					"bRxGain":XXX			接收信号衰减寄存器. 每增加1增加1DB的增益
        //					"wPhyCellId":XXX		物理小区ID.
        //					"wLAC":XXX			    追踪区域码。GSM：LAC;CDMA：REG_ZONE
        //					"wUARFCN":XXX			小区频点. CDMA 制式为BSID
        //					"dwCellId":XXX			小区ID。注意在CDMA制式没有小区ID，高位WORD 是SID ， 低位WORD 是NID
        //       }
        //    ];
        //"n_dic":
        //   [
        //       "name":"CONFIG_SMS_CONTENT_MSG",                 //4.10  FAP 配置下发短信号码和内容
        //      {
        //					"sms_ctrl":XXX	    	        上号后是否自动发送短信。0：不自动发送；1：自动发关
        //					"bSMSOriginalNum":XXX	    	主叫号码
        //					"bSMSContent":XXX	            短信内容.unicode编码，每个字符占2字节
        //       }
        //    ],
        //"n_dic":   ---GSM设备没有该项
        //   [
        //       "name":"CONFIG_CDMA_CARRIER_MSG",            //4.14  GUI 配置CDMA多载波参数
        //       {
        //					"wARFCN1":XXX	        工作频点1	
        //					"bARFCN1Mode":XXX	    工作频点1模式。0表示扫描，1表示常开,2表示关闭。
        //					"wARFCN1Duration":XXX	工作频点1扫描时长
        //					"wARFCN1Period":XXX	    工作频点1扫描间隔
        //					"wARFCN2":XXX	        工作频点2
        //					"bARFCN2Mode":XXX	    工作频点2模式。 0表示扫描，1表示常开,2表示关闭。
        //					"wARFCN2Duration":XXX	工作频点2扫描时长
        //					"wARFCN2Period":XXX	    工作频点2扫描间隔
        //					"wARFCN3":XXX	        工作频点3	
        //					"bARFCN3Mode":XXX	    工作频点3模式。 0表示扫描，1表示常开,2表示关闭。
        //					"wARFCN3Duration":XXX	工作频点3扫描时长	
        //					"wARFCN3Period":XXX	    工作频点3扫描间隔
        //					"wARFCN4":XXX	        工作频点4	
        //					"bARFCN4Mode":XXX	    工作频点4模式。	0表示扫描，1表示常开,2表示关闭。
        //					"wARFCN4Duration":XXX	工作频点4扫描时长
        //					"wARFCN4Period":XXX	    工作频点4扫描间隔
        //       }
        //    ];
        #endregion
        public const string SetGenParaReq = "SetGenParaReq";

        //  
        //设置通用参数的响应 (ApController-->MainController)
        //"dic":
        //{
        //  "rebootflag":"0",  
        //  "result":"1",     //0成功，1失败
        //  "sys":系统号，0表示系统1或通道1或射频1，1表示系统2或通道2或射频2(无此字段时，表示sys为0)
        //}
        public const string SetGenParaRsp = "SetGenParaRsp";

        /// <summary>
        /// 数据对齐完成。(MainController-->ApController)
        /// </summary>
        //"dic":
        //{
        //    "ReturnCode": 返回码：0,成功；其它值为失败
        //    "ReturnStr": 失败原因值。ReturnCode不为0时有意义
        //    "sys":系统号，0表示系统1或通道1或射频1，1表示系统2或通道2或射频2(无此字段时，表示sys为0)
        //}
        public const string DataAlignOver = "DataAlignOver";

        /// <summary>
        /// 数据对齐完成回复。(ApController-->MainController)
        //"dic":
        //{
        //  "rebootflag":"0",  
        //  "result":"1",     //0成功，1失败
        //  "sys":系统号，0表示系统1或通道1或射频1，1表示系统2或通道2或射频2(无此字段时，表示sys为0)
        //}
        public const string DataAlignOverAck = "DataAlignOverAck";

        //#region GSM设备特有消息类型
        ///// <summary>
        ///// GSM设备参数更改通知(ApController-->MainController) （n_dic部分可以一个或多个）
        ///// </summary>
        ///// 
        ////"dic":
        ////    {
        ////        "sys":系统号，0表示系统1或通道1或射频1，1表示系统2或通道2或射频2
        ////        "hardware_id":硬件Id
        ////    }
        ////"n_dic":
        ////   [
        ////       "name":"RECV_SYS_PARA",                 //4.1   系统参数
        ////      {
        ////					"paraMcc":移动国家码
        ////					"paraMnc":移动网号
        ////					"paraBsic":基站识别码
        ////					"paraLac":位置区号
        ////					"paraCellId":小区ID
        ////					"paraC2":C2偏移量
        ////					"paraPeri":周期性位置更新周期
        ////					"paraAccPwr":接入功率
        ////					"paraMsPwr":手机发射功率
        ////					"paraRejCau":位置更新拒绝原因
        ////       }
        ////    ],
        ////"n_dic":
        ////   [
        ////       "name":"RECV_SYS_OPTION",                 //4.2  系统选项
        ////      {
        ////					"opLuSms":登录时发送短信
        ////					"opLuImei":登录时获取IMEI
        ////					"opCallEn":允许用户主叫
        ////					"opDebug":调试模式，上报信令
        ////					"opLuType":登录类型
        ////					"opSmsType":短信类型
        ////       }
        ////    ],
        ////"n_dic":
        ////   [
        ////       "name":"RECV_RF_PARA",                 //4.4	射频参数
        ////      {
        ////					"rfEnable":射频使能
        ////					"rfFreq":信道号
        ////					"rfPwr":发射功率衰减值
        ////       }
        ////    ],
        ////"n_dic":
        ////   [
        ////       "name":"RECV_SMS_OPTION",                //4.7	短消息中心号码;4.8	短消息原叫号码;4.9 短消息发送时间;4.10  短消息内容
        ////       {
        ////          "gSmsRpoa":短消息中心号码
        ////          "gSmsTpoa":短消息原叫号码
        ////          "gSmsScts":短消息发送时间 （时间格式为年/月/日/时/分/秒各两位，不足两位前补0。如2014年4月22日15点46分47秒的消息内容为“140422154647”）
        ////          "gSmsData":短消息内容 （编码格式为Unicode编码）
        ////          "autoSendtiny":是否自动发送
        ////          "autoFilterSMStiny":是否自动过滤短信
        ////          "delayTime":发送延时时间
        ////          "smsCodingtiny":短信的编码格式
        ////         }
        ////    ],
        ////"n_dic":
        ////   [
        ////       "name":"RECV_REG_MODE",            //4.33	注册工作模式
        ////       {
        ////          "regMode":模式0时由设备自行根据系统选项决定是否允许终端入网，是否对终端发送短信；
        ////                    模式1时设备将终端标识发送给上位机，由上位机告知设备下一步的动作
        ////       }
        ////    ];
        //public const string gsm_para_change = "gsm_para_change";

        ///// <summary>
        ///// GSM设备参数更改通知回复 (MainController-->ApController)
        ///// </summary>
        ////"dic":
        ////{
        ////    "ReturnCode": 返回码：0,成功；其它值为失败
        ////    "ReturnStr": 失败原因值。ReturnCode不为0时有意义
        ////}
        //public const string gsm_para_change_ack = "gsm_para_change_ack";

        //#endregion
    }

    public class MsgStruct
    {
        /// <summary>
        /// 表示空设备
        /// </summary>
        static public string NullDevice = "NULL_DEVICE";
        /// <summary>
        /// 表示所有设备
        /// </summary>
        static public string AllDevice = "ALL_DEVICE";
        /// <summary>
        /// 表示字典里（包括AllNum）所有键值对数。
        /// </summary>
        static public string AllNum = "AllNum";

        /// <summary>
        /// AP的内部类型
        /// </summary>
        public enum ApInnerType
        {
            GSM = 0,     //GSM_HJT
            GSM_V2,   //GSM_ZYF
            TD_SCDMA,
            CDMA,      //CDMA_ZYF
            WCDMA,
            LTE_TDD,
            LTE_FDD
        }

        /// <summary>
        /// App的内部类型
        /// </summary>
        public enum AppInnerType
        {
            APP_WINDOWS = 0
        }

        public enum MsgType
        {
            /// <summary>
            /// 通知消息。用于AP上线，Imsi上报等
            /// </summary>
            NOTICE = 1,
            /// <summary>
            /// 透传消息。MainController收到该消息后不用处理，直接透传
            /// </summary>
            TRANSPARENT,
            /// <summary>
            /// 配置消息。用于配置读取、设置及配置回复
            /// </summary>
            CONFIG,
        }

        /// <summary>
        /// 消息id号与App对应关系
        /// </summary>
        public class MsgId2App
        {
            /// <summary>
            /// 消息id
            /// </summary>
            public UInt16 id;
            /// <summary>
            /// App信息
            /// </summary>
            public App_Info_Struct AppInfo;
            /// <summary>
            /// 消息添加时间
            /// </summary>
            public DateTime AddTime;

            public MsgId2App()
            {
                AppInfo = new App_Info_Struct();
                AddTime = DateTime.Now;
            }
        }
        /// <summary>
        /// Ap设备信息
        /// </summary>
        public class Ap_Info_Struct
        {
            /// <summary>
            /// Ap的Sn
            /// </summary>
            public string SN { get; set; }
            /// <summary>
            /// Ap的全名
            /// </summary>
            public string Fullname { get; set; }
            /// <summary>  
            /// Ap的Ip 。当为NullDevice时表示不向设备发送该消息
            /// </summary>  
            public string IP { get; set; }
            /// <summary>  
            /// Ap的Port
            /// </summary>  
            public int Port { get; set; }
            /// <summary>  
            /// Ap的类型：LTE,WCDMA,GSM,TD-SCDMA           
            /// </summary>  
            public string Type { get; set; }

            public Ap_Info_Struct()
            {
                this.SN = string.Empty;
                this.Fullname = string.Empty;
                this.IP = NullDevice;
                this.Port = 0;
                this.Type = string.Empty;
            }
        }

        /// <summary>
        /// App信息
        /// </summary>
        public class App_Info_Struct
        {
            /// <summary>
            /// App登录用户名
            /// </summary>
            public string User { get; set; }
            /// <summary>
            /// App登录用户所属组
            /// </summary>
            public string Group { get; set; }
            /// <summary>
            /// App登录用户所属域
            /// </summary>
            public string Domain { get; set; }
            /// <summary>  
            /// Ap的Ip 
            /// </summary>  
            public string Ip { get; set; }
            /// <summary>  
            /// Ap的Port
            /// </summary>  
            public int Port { get; set; }
            /// <summary>  
            /// App的类型：WEB,WINDOWS,ANDROID等
            /// </summary>  
            public string Type { get; set; }

            public App_Info_Struct()
            {
                this.User = string.Empty;
                this.Group = string.Empty;
                this.Domain = string.Empty;
                this.Ip = NullDevice;
                this.Port = 0;
                this.Type = string.Empty;
            }
        }

        /// <summary>
        /// 设备(Ap/App)与本程序通信消息结构
        /// </summary>
        public class DeviceServerMsgStruct
        {
            /// <summary>
            /// 版本信息
            /// </summary>
            public string Version { get; set; }
            /// <summary>
            /// Ap信息
            /// </summary>
            public Ap_Info_Struct ApInfo;
            /// <summary>
            /// 消息内容
            /// </summary>
            //public List<Msg_Body_Struct> TypeKeyValueList;
            public Msg_Body_Struct Body;

            public DeviceServerMsgStruct()
            {
                ApInfo = new Ap_Info_Struct();
                Body = new Msg_Body_Struct();
            }

            public DeviceServerMsgStruct(string type)
            {
                ApInfo = new Ap_Info_Struct();
                Body = new Msg_Body_Struct(type);
            }
        }

        /// <summary>
        /// 本程序模块间消息结构
        /// </summary>
        public class InterModuleMsgStruct : DeviceServerMsgStruct
        {
            /// <summary>
            /// 消息类型（Notice;Get;GetAck;Set;SetAck）
            /// </summary>
            public string MsgType { get; set; }

            /// <summary>
            /// App信息
            /// </summary>
            public App_Info_Struct AppInfo;

            public InterModuleMsgStruct()
            {
                AppInfo = new App_Info_Struct();
            }
        }

        public class Msg_Body_Struct
        {
            public string type;
            public Dictionary<string, object> dic;
            public List<Name_DIC_Struct> n_dic;

            #region 构造函数

            public Msg_Body_Struct(string type)
            {
                this.type = type;
                dic = new Dictionary<string, object>();
                n_dic = new List<Name_DIC_Struct>();
            }
            public Msg_Body_Struct() : this(string.Empty)
            {
            }
            public Msg_Body_Struct(string type, Dictionary<string, object> dic)
            {
                this.type = type;
                this.dic = dic;
                n_dic = null;
            }

            public Msg_Body_Struct(string type, Dictionary<string, object> dic, List<Name_DIC_Struct> n_dic)
            {
                this.type = type;
                this.dic = dic;
                this.n_dic = n_dic;
            }
            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="xmlType">xml消息类型</param>
            /// <param name="KeyValue">xml消息里的键值对，键值对必须成对出现。</param>
            public Msg_Body_Struct(string xmlType, params string[] KeyValue)
            {
                type = xmlType;
                dic = new Dictionary<string, object>();

                if ((KeyValue.Length % 2) != 0)
                {
                    //OutputLog("输入的参数不是2的倍数。键值对必须成对出现。");
                }
                else
                {
                    for (int i = 0; i < KeyValue.Length; i = i + 2)
                    {
                        dic.Add(KeyValue[i], KeyValue[i + 1]);
                    }

                    //dic.Add(MsgStruct.AllNum, dic.Count + 1);
                }
            }

            public Msg_Body_Struct(string xmlType, params object[] KeyValue)
            {
                type = xmlType;
                dic = new Dictionary<string, object>();

                if ((KeyValue.Length % 2) != 0)
                {
                    //OutputLog("输入的参数不是2的倍数。键值对必须成对出现。");
                }
                else
                {
                    for (int i = 0; i < KeyValue.Length; i = i + 2)
                    {
                        dic.Add((string)KeyValue[i], KeyValue[i + 1]);
                    }

                    //dic.Add(MsgStruct.AllNum, dic.Count + 1);
                }
            }

            #endregion
        }

        public class Name_DIC_Struct
        {
            public string name;
            public Dictionary<string, object> dic;

            public Name_DIC_Struct(string name)
            {
                this.name = string.Empty;
                dic = new Dictionary<string, object>();
            }

            public Name_DIC_Struct() : this(string.Empty)
            {
            }
        }

        /// <summary>
        /// 在字典中查找值
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="dic">字典</param>
        /// <returns>值，未找到返回空</returns>
        static public object GetMsgValueInList(string key, Dictionary<string, object> dic)
        {
            if (!dic.ContainsKey(key)) return null;
            return dic[key];
        }
        static public object GetMsgValueInList(string key, Dictionary<string, object> dic, object def)
        {
            if (!dic.ContainsKey(key)) return def;
            return dic[key];
        }
        static public string GetMsgStringValueInList(string key, Dictionary<string, object> dic)
        {
            if (!dic.ContainsKey(key)) return string.Empty;
            return Convert.ToString(dic[key]);
        }
        static public string GetMsgStringValueInList(string key, Dictionary<string, object> dic, string def)
        {
            if (!dic.ContainsKey(key)) return def;
            return Convert.ToString(dic[key]);
        }
        static public Byte GetMsgByteValueInList(string key, Dictionary<string, object> dic)
        {
            if (!dic.ContainsKey(key)) return 0;
            return Convert.ToByte(dic[key]);
        }
        static public Byte GetMsgByteValueInList(string key, Dictionary<string, object> dic, byte def)
        {
            if (!dic.ContainsKey(key)) return def;
            return Convert.ToByte(dic[key]);
        }
        static public SByte GetMsgSByteValueInList(string key, Dictionary<string, object> dic)
        {
            if (!dic.ContainsKey(key)) return 0;
            return Convert.ToSByte(dic[key]);
        }
        static public SByte GetMsgSByteValueInList(string key, Dictionary<string, object> dic, sbyte def)
        {
            if (!dic.ContainsKey(key)) return def;
            return Convert.ToSByte(dic[key]);
        }
        static public UInt16 GetMsgU16ValueInList(string key, Dictionary<string, object> dic)
        {
            if (!dic.ContainsKey(key)) return 0;
            return Convert.ToUInt16(dic[key]);
        }
        static public UInt16 GetMsgU16ValueInList(string key, Dictionary<string, object> dic, UInt16 def)
        {
            if (!dic.ContainsKey(key)) return def;
            return Convert.ToUInt16(dic[key]);
        }
        static public UInt32 GetMsgU32ValueInList(string key, Dictionary<string, object> dic)
        {
            if (!dic.ContainsKey(key)) return 0;
            return Convert.ToUInt32(dic[key]);
        }
        static public UInt32 GetMsgU32ValueInList(string key, Dictionary<string, object> dic, UInt32 def)
        {
            if (!dic.ContainsKey(key)) return def;
            return Convert.ToUInt32(dic[key]);
        }
        static public int GetMsgIntValueInList(string key, Dictionary<string, object> dic)
        {
            if (!dic.ContainsKey(key)) return 0;
            return Convert.ToInt32(dic[key]);
        }
        static public int GetMsgIntValueInList(string key, Dictionary<string, object> dic, int def)
        {
            if (!dic.ContainsKey(key)) return def;
            return Convert.ToInt32(dic[key]);
        }
        static public double GetMsgDoubleValueInList(string key, Dictionary<string, object> dic)
        {
            if (!dic.ContainsKey(key)) return 0;
            return Convert.ToDouble(dic[key]);
        }
        static public double GetMsgDoubleValueInList(string key, Dictionary<string, object> dic, double def)
        {
            if (!dic.ContainsKey(key)) return def;
            return Convert.ToDouble(dic[key]);
        }

        /// <summary>
        /// 在第一层字典里查找值
        /// </summary>
        /// <param name="name">第二层名称</param>
        /// <param name="key">键</param>
        /// <param name="msgBody">消息内容</param>
        /// <returns>值。未找到时返回空</returns>
        static public object GetMsgValueInList(string name, string key, Msg_Body_Struct msgBody)
        {
            List<Name_DIC_Struct> n_dic = msgBody.n_dic;
            if (n_dic == null)
                return string.Empty;

            foreach (Name_DIC_Struct x in n_dic)
            {
                if (String.Compare(x.name, name, true) == 0)
                {
                    Dictionary<string, object> dic = x.dic;
                    //没有该键
                    if (!dic.ContainsKey(key)) return string.Empty;

                    return dic[key];
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// 在第一层字典里查找值
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="msgBody">消息内容</param>
        /// <returns>值。未找到时返回空</returns>
        static public object GetMsgValueInList(string key, Msg_Body_Struct msgBody)
        {
            Dictionary<string, object> dic = msgBody.dic;
            if (!dic.ContainsKey(key)) return string.Empty;
            return dic[key];
        }
        static public int GetMsgIntValueInList(string key, Msg_Body_Struct msgBody)
        {
            Dictionary<string, object> dic = msgBody.dic;
            if (!dic.ContainsKey(key)) return 0;
            return Convert.ToInt32(dic[key]);
        }
        static public string GetMsgStringValueInList(string key, Msg_Body_Struct msgBody)
        {
            Dictionary<string, object> dic = msgBody.dic;
            if (!dic.ContainsKey(key)) return string.Empty;
            return Convert.ToString(dic[key]);
        }
        static public double GetMsgDoubleValueInList(string key, Msg_Body_Struct msgBody)
        {
            Dictionary<string, object> dic = msgBody.dic;
            if (!dic.ContainsKey(key)) return 0;
            return Convert.ToDouble(dic[key]);
        }
        /// <summary>
        /// Ap(app)与本程序通信消息封装示例
        /// </summary>
        private void DeviceServerMsgEncodeDemo()
        {
            //只有单层字典的封装示例
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("timeout", Convert.ToString(5));
            dic.Add("timestamp", DateTime.Now.ToLocalTime().ToString());
            //AllNum表示该字典中所有键值对的个数,协议规定，每条消息必须包括
            //dic.Add(MsgStruct.AllNum, dic.Count + 1);

            Msg_Body_Struct TypeKeyValue = new Msg_Body_Struct(ApMsgType.status_request, dic);


            /*
            //多层字典封装示例
            List<Name_DIC_Struct> n_dicList = new List<Name_DIC_Struct>();
            Name_DIC_Struct n_dic = new Name_DIC_Struct();

            n_dic.name = "name1";
            n_dic.dic.Add("key1","1");
            n_dic.dic.Add("key2","2");
            //AllNum表示该字典中所有键值对的个数,协议规定，每条消息必须包括
            n_dic.dic.Add(MsgStruct.AllNum, n_dic.dic.Count + 1);
            n_dicList.Add(n_dic);

            n_dic.name = "name2";
            n_dic.dic.Add("key1", "1");
            n_dic.dic.Add("key2", "2");
            //AllNum表示该字典中所有键值对的个数,协议规定，每条消息必须包括
            n_dic.dic.Add(MsgStruct.AllNum, n_dic.dic.Count + 1);
            n_dicList.Add(n_dic);

            Msg_Body_Struct TypeKeyValue2 = new Msg_Body_Struct(ApMsgType.status_request, dic,n_dicList);
            */

            DeviceServerMsgStruct msgStruct = new DeviceServerMsgStruct();
            msgStruct.Version = "1.0.0";
            //msgStruct.MsgType = MsgType.CONFIG.ToString();

            msgStruct.ApInfo.IP = "192.168.88.100";
            msgStruct.ApInfo.Port = 12345;
            msgStruct.ApInfo.SN = "EN1800123456789";
            msgStruct.ApInfo.Fullname = "guangdong.shenzhen.nanshan.1234";

            msgStruct.Body = TypeKeyValue;

            string strJosn = JsonConvert.SerializeObject(msgStruct);
        }

        /// <summary>
        /// Ap(app)与本程序通信消息解析示例
        /// </summary>
        /// <param name="str">收到的消息</param>
        private void DeviceServerMsgDecodeDemo(string str)
        {
            int i = 0;
            // 解析收到的消息
            DeviceServerMsgStruct msgStruct = JsonConvert.DeserializeObject<DeviceServerMsgStruct>(str);

            Console.WriteLine("------------------------------------------------------");
            Console.WriteLine("消息类型：" + msgStruct.Body.type.ToString());

            foreach (KeyValuePair<string, object> kvp in msgStruct.Body.dic)
            {
                Console.WriteLine("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
            }

            foreach (Name_DIC_Struct x in msgStruct.Body.n_dic)
            {
                Console.WriteLine("List [" + i.ToString() + "] 名称:" + x.name);
                i++;
                foreach (KeyValuePair<string, object> kvp in x.dic)
                {
                    Console.WriteLine("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
                }
            }
            Console.WriteLine("------------------------------------------------------");
        }

        /// <summary>
        /// ApController(AppController)模块与MainController模块通信消息封装示例
        /// </summary>
        private void InterModuleMsgEncodeDemo()
        {
            //只有单层字典的封装示例
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("timeout", Convert.ToString(5));
            dic.Add("timestamp", DateTime.Now.ToLocalTime().ToString());
            //AllNum表示该字典中所有键值对的个数,协议规定，每条消息必须包括
            //dic.Add(MsgStruct.AllNum, dic.Count + 1);

            Msg_Body_Struct TypeKeyValue = new Msg_Body_Struct(ApMsgType.status_request, dic);


            /*
            //多层字典封装示例
            List<Name_DIC_Struct> n_dicList = new List<Name_DIC_Struct>();
            Name_DIC_Struct n_dic = new Name_DIC_Struct();

            n_dic.name = "name1";
            n_dic.dic.Add("key1","1");
            n_dic.dic.Add("key2","2");
            //AllNum表示该字典中所有键值对的个数,协议规定，每条消息必须包括
            n_dic.dic.Add("AllNum", n_dic.dic.Count + 1);
            n_dicList.Add(n_dic);

            n_dic.name = "name2";
            n_dic.dic.Add("key1", "1");
            n_dic.dic.Add("key2", "2");
            //AllNum表示该字典中所有键值对的个数,协议规定，每条消息必须包括
            n_dic.dic.Add("AllNum", n_dic.dic.Count + 1);
            n_dicList.Add(n_dic);

            Msg_Body_Struct TypeKeyValue2 = new Msg_Body_Struct(ApMsgType.status_request, dic,n_dicList);
            */

            InterModuleMsgStruct msgStruct = new InterModuleMsgStruct();
            msgStruct.Version = "1.0.0";
            msgStruct.MsgType = MsgType.CONFIG.ToString();

            msgStruct.ApInfo.IP = "192.168.88.100";
            msgStruct.ApInfo.Port = 12345;
            msgStruct.ApInfo.SN = "EN1800123456789";
            msgStruct.ApInfo.Fullname = "guangdong.shenzhen.nanshan.1234";

            msgStruct.AppInfo.Ip = "192.168.88.104";
            msgStruct.AppInfo.Port = 65478;
            msgStruct.AppInfo.User = "root";
            msgStruct.AppInfo.Group = "guangdong";

            msgStruct.Body = TypeKeyValue;

            string strJosn = JsonConvert.SerializeObject(msgStruct);
        }

        /// <summary>
        /// ApController(AppController)模块与MainController模块通信消息解析示例
        /// </summary>
        /// <param name="str">收到的消息</param>
        private void InterModuleMsgDecodeDemo(string str)
        {
            int i = 0;
            // 解析收到的消息
            InterModuleMsgStruct msgStruct = JsonConvert.DeserializeObject<InterModuleMsgStruct>(str);

            Console.WriteLine("------------------------------------------------------");
            Console.WriteLine("消息类型：" + msgStruct.Body.type.ToString());

            foreach (KeyValuePair<string, object> kvp in msgStruct.Body.dic)
            {
                Console.WriteLine("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
            }

            if (msgStruct.Body.n_dic != null)
            {
                foreach (Name_DIC_Struct x in msgStruct.Body.n_dic)
                {
                    Console.WriteLine("List [" + i.ToString() + "] 名称:" + x.name);
                    i++;
                    foreach (KeyValuePair<string, object> kvp in x.dic)
                    {
                        Console.WriteLine("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
                    }
                }
            }
            Console.WriteLine("------------------------------------------------------");
        }

    }
}
