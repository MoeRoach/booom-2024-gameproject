using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoachFramework {
    /// <summary>
    /// 服务提供器单例
    /// </summary>
    public class ServiceProvider : BaseSingleton<ServiceProvider> {
        /// <summary>
        /// 服务映射表
        /// </summary>
        private readonly Dictionary<string, IGameService> _services = new Dictionary<string, IGameService>();
        protected override void OnInitialized() { }
        
        /// <summary>
        /// 注册服务
        /// </summary>
        /// <param name="tag">服务标签</param>
        /// <param name="service">服务具体类</param>
        public void RegisterService(string tag, IGameService service) {
            if (_services.ContainsKey(tag)) {
                Debug.LogWarning("Duplicated Service Tag detected, please advice!");
            } else {
                _services[tag] = service;
                service.InitService();
            }
        }

        /// <summary>
        /// 反注册服务
        /// </summary>
        /// <param name="tag">服务标签</param>
        public void UnregisterService(string tag) {
            if (!_services.ContainsKey(tag)) return;
            _services[tag].KillService();
            _services.Remove(tag);
        }

        /// <summary>
        /// 提供服务，如果找不到对应服务则新建一个
        /// </summary>
        /// <typeparam name="TService">服务类型，必须是类并且实现IGameService接口</typeparam>
        /// <param name="tag">服务标签</param>
        /// <returns>服务对象</returns>
        public TService ProvideService<TService>(string tag) where TService : class, IGameService, new() {
            TService result;
            if (_services.ContainsKey(tag)) {
                result = _services[tag] as TService;
            } else {
                result = new TService();
                result.InitService();
                _services[tag] = result;
            }

            return result;
        }
    }
    
    /// <summary>
    /// 游戏服务基础接口
    /// </summary>
    public interface IGameService {
        void InitService();
        void KillService();
    }
}
