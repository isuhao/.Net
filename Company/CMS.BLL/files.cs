using System;
using System.Data;
using System.Collections.Generic;
using System.IO;
using CMS.Common;

namespace CMS.BLL
{
    /// <summary>
    /// �ϴ��ļ��洢
    /// </summary>
    public partial class files
    {
        private readonly Model.siteconfig siteConfig = new BLL.siteconfig().loadConfig(); //���վ��������Ϣ
        private readonly DAL.files dal;
        public files()
        {
            dal = new DAL.files(siteConfig.sysdatabaseprefix);
        }

        #region ��������============================
        /// <summary>
        /// �Ƿ���ڸü�¼
        /// </summary>
        public bool Exists(string fileHashValue)
        {
            return dal.Exists(fileHashValue);
        }

        /// <summary>
        /// ����һ������(�ж�HASHֵ�Ƿ����)
        /// </summary>
        public Int64 Add(Model.files model)
        {
            Int64 modelID = GetModeID(model.file_md5);
            return modelID > 0 ? modelID : dal.Add(model);
        }

        /// <summary>
        /// ����һ������(�����ļ���ַ)
        /// </summary>
        /// <param name="fileUrl"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public Model.files Add(string fileUrl, int userID = 0, bool neednew = false)
        {
            string filePath = Utils.GetMapPath(fileUrl);
            Model.files model = new Model.files();
            model.file_name = Path.GetFileName(filePath);
            model.file_fullpath = Path.GetDirectoryName(filePath);
            model.file_path = fileUrl;
            string fileExtension = Path.GetExtension(filePath);
            model.file_endwith = fileExtension;
            if (fileExtension.ToLower() == ".jpg" || fileExtension.ToLower() == ".jpeg" || fileExtension.ToLower() == ".png" || fileExtension.ToLower() == ".gif")
            {
                model.file_type = 0;
            }
            else if (fileExtension.ToLower() == ".rar" || fileExtension.ToLower() == ".zip" || fileExtension.ToLower() == ".7z")
            {
                model.file_type = 1;
            }
            else if (fileExtension.ToLower() == ".xls" || fileExtension.ToLower() == ".xlsx" || fileExtension.ToLower() == ".doc" || fileExtension.ToLower() == ".docx")
            {
                model.file_type = 2;
            }
            else
            {
                model.file_type = 3;
            }
            model.file_md5 = Utils.GetMD5HashFromFile(filePath);
            model.file_server = Utils.GetAppSettings("LocalHost");
            model.file_state = 0;
            model.file_uptime = DateTime.Now;
            model.file_upuser = userID;
            model.file_usetimes = 0;
            Int64 modelID = GetModeID(model.file_md5);
            if (modelID > 0)
            {
                Model.files modelold = GetModel(modelID);
                if (!neednew)
                {
                    //���ԭ�ļ��Ƿ���ڣ�������ɾ�����ļ������������¼
                    if (File.Exists(Utils.GetMapPath(modelold.file_path)))
                    {
                        if (Utils.GetMD5HashFromFile(Utils.GetMapPath(modelold.file_path)).ToLower() == modelold.file_md5)
                        {
                            File.Delete(Utils.GetMapPath(model.file_path));
                            return modelold;
                        }
                        else
                        {
                            Delete(modelold.id);
                        }
                    }
                    else
                    {
                        Delete(modelold.id);
                    }
                }
                else
                {
                    //���ԭ�ļ��Ƿ���ڣ�������ɾ�������������ļ�
                    if (File.Exists(Utils.GetMapPath(modelold.file_path)))
                    {
                        File.Delete(Utils.GetMapPath(modelold.file_path));
                    }
                    Delete(modelold.id);
                }
            }
            model.id = Add(model);
            return model;
        }

        /// <summary>
        /// ����һ������
        /// </summary>
        public bool Update(Model.files model)
        {
            return dal.Update(model);
        }

        /// <summary>
        /// ɾ��һ������
        /// </summary>
        public bool Delete(Int64 id)
        {
            return dal.Delete(id);
        }

        /// <summary>
        /// �����ļ�HASHֵɾ��һ������
        /// </summary>
        /// <param name="fileHashValue"></param>
        /// <returns></returns>
        public bool Delete(string fileHashValue)
        {
            Int64 modelID = GetModeID(fileHashValue);
            return modelID == 0 ? false : Delete(modelID);
        }

        /// <summary>
        /// �õ�һ������ʵ��
        /// </summary>
        public Model.files GetModel(Int64 id)
        {
            return dal.GetModel(id);
        }

        /// <summary>
        /// �����ļ�HASHֵ���һ������ʵ��
        /// </summary>
        /// <param name="fileHashValue"></param>
        /// <returns></returns>
        public Model.files GetModel(string fileHashValue)
        {
            Int64 modelID = GetModeID(fileHashValue);
            return modelID == 0 ? null : GetModel(modelID);
        }

        /// <summary>
        /// ��������б�
        /// </summary>
        public DataSet GetList(string strWhere)
        {
            return dal.GetList(strWhere);
        }

        /// <summary>
        /// ���ǰ��������
        /// </summary>
        public DataSet GetList(int Top, string strWhere, string filedOrder)
        {
            return dal.GetList(Top, strWhere, filedOrder);
        }

        /// <summary>
        /// ��ò�ѯ��ҳ����
        /// </summary>
        public DataSet GetList(int pageSize, int pageIndex, string strWhere, string filedOrder, out int recordCount)
        {
            return dal.GetList(pageSize, pageIndex, strWhere, filedOrder, out recordCount);
        }

        #endregion

        #region ��չ����============================
        /// <summary>
        /// ����HASHֵ����ID
        /// </summary>
        public Int64 GetModeID(string fileHashValue)
        {
            return dal.GetModeID(fileHashValue);
        }

        /// <summary>
        /// �޸�һ������
        /// </summary>
        public void UpdateField(Int64 id, string strValue)
        {
            dal.UpdateField(id, strValue);
        }

        /// <summary>
        /// �����ļ�HASHֵ�޸�һ������
        /// </summary>
        /// <param name="fileHashValue"></param>
        /// <param name="strValue"></param>
        public void UpdateField(string fileHashValue, string strValue)
        {
            Int64 modelID = GetModeID(fileHashValue);
            if (modelID > 0)
            {
                UpdateField(modelID, strValue);
            }
        }
        #endregion
    }
}