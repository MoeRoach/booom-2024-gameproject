using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

namespace RoachFramework {
    /// <summary>
    /// 多线程后台抽象基类，可以派生各种类型的多线程后台控制器
    /// </summary>
    public abstract class AbstractBackend : BaseGameObject {
        
        public int callbackToMainPerFrame = 8;
        	
        protected ConcurrentQueue<CallbackData> callbackQueue;
        protected Dictionary<string, bool> threadBarrier;
    
        protected override void OnAwake() {
        	base.OnAwake();
        	callbackQueue = new ConcurrentQueue<CallbackData>();
        	threadBarrier = new Dictionary<string, bool>();
        }

        protected override void OnUpdate() {
        	base.OnUpdate();
        	if (callbackQueue == null) return;
        	for (var i = 0; i < callbackToMainPerFrame; i++) {
        		if (callbackQueue.Count > 0) {
        			if (callbackQueue.TryDequeue(out var data)) {
        				ProcessCallback(data);
        			} else {
        				break;
        			}
        		} else {
        			break;
        		}
        	}
        }

        protected virtual void ProcessCallback(CallbackData data) {
        	var msg = BroadcastInfo.Create(data.action, data.content);
        	foreach (var key in data.extras.Keys) {
        		var val = data.extras[key];
        		switch (val) {
        			case int i:
        				msg.PutIntegerExtra(key, i);
        				break;
        			case float f:
        				msg.PutFloatExtra(key, f);
        				break;
        			case string s:
        				msg.PutStringExtra(key, s);
        				break;
        			case bool b:
        				msg.PutBooleanExtra(key, b);
        				break;
        			default:
        				msg.PutComplexExtra(key, val);
        				break;
        		}
        	}
        	broadcastService.BroadcastInformation(msg);
        	if (!string.IsNullOrEmpty(data.threadId)) {
        		threadBarrier[data.threadId] = true;
        	}
        }

        /// <summary>
        /// 将回调加入主线程队列
        /// </summary>
        /// <param name="data">预先构造的回调数据</param>
        public virtual void CallbackToMainThread(CallbackData data) {
        	callbackQueue.Enqueue(data);
        }

        /// <summary>
        /// 获取线程回调状态
        /// </summary>
        /// <param name="tid">线程标识符</param>
        /// <returns>是否完成回调</returns>
        public virtual bool CheckThreadBarrier(string tid) {
	        return threadBarrier.ContainsKey(tid) && threadBarrier[tid];
        }
        
        public class CallbackData {

        	public string threadId;
        	public string action;
        	public string content;
        	public Dictionary<string, object> extras;

        	public CallbackData(string tid, string act, string c = "") {
        		threadId = tid;
        		action = act;
        		content = c;
        		extras = new Dictionary<string, object>();
        	}
        }
    }
}
