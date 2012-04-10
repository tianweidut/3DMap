using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace _D基本元素
{
    class IniOperator   //Ini文件操作类
    {
        public string inipath;
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section,string key,string val,string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section,string key,string def,StringBuilder retVal,int size,string filePath);
    
        //构造方法
        public IniOperator()
        {
            //inipath = @"F:\！！！矿井项目备份\新UI便携仪数据管理软件\demo.ini";
            inipath = @".\demo.ini";
        }
        //写入
        public void IniWriteValue(string Section, string Key, string Value)
        {
            WritePrivateProfileString(Section, Key, Value, this.inipath);
        }
        //读出
        public string IniReadValue(string Section, string Key)
        {
            StringBuilder temp = new StringBuilder(500);
            int i = GetPrivateProfileString(Section, Key, "", temp, 500, this.inipath);
            return temp.ToString();
        }
        //验证存在
        
    }
}
