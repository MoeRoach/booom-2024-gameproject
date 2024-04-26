using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace RoachFramework {
    /// <summary>
    /// 广播服务，提供基础的广播注册和发送方法
    /// </summary>
    public class BroadcastService : IGameService {

        public const string SERVICE_NAME = "BroadcastService";

        private readonly Dictionary<string, HashSet<IBroadcastReceiver>> _receivers;

        public BroadcastService() {
            _receivers = new Dictionary<string, HashSet<IBroadcastReceiver>>();
        }

        public void InitService() {
            LogUtils.LogNotice("Broadcast Service Initiated!");
            MessageBroker.Default.Receive<BroadcastInfo>().Subscribe(ReceiveInformation);
        }

        public void KillService() {
            LogUtils.LogNotice("Broadcast Service Destoryed!");
        }

        public void BroadcastInformation(BroadcastInfo info) {
            MessageBroker.Default.Publish(info);
        }

        private void ReceiveInformation(BroadcastInfo info) {
            if (!_receivers.ContainsKey(info.Action)) return;
            var receiverSet = _receivers[info.Action];
            foreach (var receiver in receiverSet) {
                receiver.ReceiveBroadcast(info);
            }
        }

        public void RegisterBroadcastReceiver(BroadcastFilter filter, IBroadcastReceiver receiver) {
            foreach (var filterStr in filter) {
                if (_receivers.ContainsKey(filterStr)) {
                    _receivers[filterStr].Add(receiver);
                } else {
                    var actionSet = new HashSet<IBroadcastReceiver> {receiver};
                    _receivers[filterStr] = actionSet;
                }
            }
        }

        public void UnregisterBroadcastReceiver(IBroadcastReceiver receiver) {
            var filters = new BroadcastFilter();
            foreach (var filter in _receivers.Keys) {
                var receiverSet = _receivers[filter];
                if (receiverSet.Contains(receiver)) {
                    filters.AddFilter(filter);
                }
            }
            foreach (var filter in filters) {
                _receivers[filter].Remove(receiver);
            }
        }
    }
}
