﻿using System;
using System.Text;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using CMS.Common;

namespace CMS.Web.Plugin.Forum.admin
{
    public partial class board_edit : CMS.Web.UI.ManagePage
    {
        protected string action = DTEnums.ActionEnum.Add.ToString(); //操作类型
        private int id = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            string _action = DTRequest.GetQueryString("action");
            if (!string.IsNullOrEmpty(_action) && _action == DTEnums.ActionEnum.Edit.ToString())
            {
                this.action = DTEnums.ActionEnum.Edit.ToString();//修改类型
                this.id = DTRequest.GetQueryInt("id");
                if (this.id == 0)
                {
                    JscriptMsg("传输参数不正确！", "back");
                    return;
                }
                if (!new BLL.forum_board().Exists(this.id))
                {
                    JscriptMsg("版块不存在或已被删除！", "back");
                    return;
                }
            }
            if (!Page.IsPostBack)
            {
                ChkAdminLevel("plugin_forum_board", DTEnums.ActionEnum.View.ToString()); //检查权限
                AllowUserGroup();
                if (action == DTEnums.ActionEnum.Edit.ToString()) //修改
                {
                    ShowInfo(this.id);
                }
            }
        }



        #region 绑定可访问会员组=========================
        private void AllowUserGroup()
        {
            CMS.BLL.user_groups bll = new CMS.BLL.user_groups();
            DataTable dt = bll.GetList(0, "", "id asc").Tables[0];
            this.cblAllowUserGroupID.Items.Clear();
            foreach (DataRow dr in dt.Rows)
            {
                this.cblAllowUserGroupID.Items.Add(new ListItem(dr["title"].ToString(), dr["id"].ToString()));
            }
        }
        #endregion

        #region 赋值操作=================================
        private void ShowInfo(int _id)
        {
            BLL.forum_board bll = new BLL.forum_board();
            Model.forum_board model = bll.GetModel(_id);
            rblStatus.SelectedValue = model.is_lock.ToString();
            txtBoardName.Text = model.boardname;
            txtImgUrl.Text = model.img_url;
            txtSortId.Text = model.sort_id.ToString();
            txtContent.Text = model.content;
            txtModeratorList.Text = model.moderator_list;
            //赋值授予权限
            if (model.allow_usergroupid_list != null)
            {
                for (int i = 0; i < cblAllowUserGroupID.Items.Count; i++)
                {
                    string[] allowusergroupid = model.allow_usergroupid_list.Split(',');
                    foreach (string id in allowusergroupid)
                    {
                        if (id != "" && id == cblAllowUserGroupID.Items[i].Value)
                        {
                            cblAllowUserGroupID.Items[i].Selected = true;
                        }
                    }
                }
            }
        }
        #endregion

        #region 增加操作=================================
        private bool DoAdd()
        {
            bool result = false;
            Model.forum_board model = new Model.forum_board();
            BLL.forum_board bll = new BLL.forum_board();
            model.is_lock = int.Parse(rblStatus.SelectedValue);
            model.boardname = txtBoardName.Text;
            model.img_url = txtImgUrl.Text;
            model.sort_id = Utils.StrToInt(txtSortId.Text.Trim(), 99);
            model.content = txtContent.Text;
            model.moderator_list = txtModeratorList.Text;
            for (int i = 0; i < cblAllowUserGroupID.Items.Count; i++)
            {
                if (cblAllowUserGroupID.Items[i].Selected)
                {
                    model.allow_usergroupid_list += cblAllowUserGroupID.Items[i].Value + ",";
                }
            }
            model.allow_usergroupid_list = model.allow_usergroupid_list.TrimEnd(',');
            if (bll.Add(model) > 0)
            {
                AddAdminLog(DTEnums.ActionEnum.Add.ToString(), "添加论坛版块:" + model.boardname); //记录日志
                result = true;
            }
            return result;
        }
        #endregion

        #region 修改操作=================================
        private bool DoEdit(int _id)
        {
            bool result = false;
            BLL.forum_board bll = new BLL.forum_board();
            Model.forum_board model = bll.GetModel(_id);
            model.is_lock = int.Parse(rblStatus.SelectedValue);
            model.boardname = txtBoardName.Text;
            model.img_url = txtImgUrl.Text;
            model.sort_id = Utils.StrToInt(txtSortId.Text.Trim(), 99);
            model.content = txtContent.Text;
            model.moderator_list = txtModeratorList.Text;
            model.allow_usergroupid_list = "";
            for (int i = 0; i < cblAllowUserGroupID.Items.Count; i++)
            {
                if (cblAllowUserGroupID.Items[i].Selected)
                {
                    model.allow_usergroupid_list += cblAllowUserGroupID.Items[i].Value + ",";
                }
            }
            model.allow_usergroupid_list = model.allow_usergroupid_list.TrimEnd(',');
            if (bll.Update(model))
            {
                AddAdminLog(DTEnums.ActionEnum.Edit.ToString(), "修改论坛版块信息:" + model.boardname); //记录日志
                result = true;
            }
            return result;
        }
        #endregion


        //保存
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (action == DTEnums.ActionEnum.Edit.ToString()) //修改
            {
                ChkAdminLevel("plugin_forum_board", DTEnums.ActionEnum.Edit.ToString()); //检查权限
                if (!DoEdit(this.id))
                {
                    JscriptMsg("保存过程中发生错误！", "");
                    return;
                }
                JscriptMsg("修改版块成功！", "board_list.aspx");
            }
            else //添加
            {
                ChkAdminLevel("plugin_forum_board", DTEnums.ActionEnum.Add.ToString()); //检查权限
                if (!DoAdd())
                {
                    JscriptMsg("保存过程中发生错误！", "");
                    return;
                }
                JscriptMsg("添加版块成功！", "board_list.aspx");
            }
        }

    }
}