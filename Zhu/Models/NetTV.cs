using GalaSoft.MvvmLight.Ioc;
using Infrastructure;
using Zhu.Services;
using System;

namespace Zhu.Models
{
    public class NetTV : Media
    {
        /// <summary>
        /// 网络地址
        /// </summary>
        public string StreamNetworkAddress { get; set; }




        public override string Path
        {
            get
            {
                return this.StreamNetworkAddress;
            }
        }
    }
}
