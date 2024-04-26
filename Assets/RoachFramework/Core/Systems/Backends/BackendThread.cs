using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace RoachFramework {
    /// <summary>
    /// 后台线程抽象基类，用于派生实际运行的功能线程
    /// </summary>
    public abstract class BackendThread {
    	
    	private static int threadCount = 0;

    	protected int threadTimestamp;
    	public string ThreadIdentifier { get; }

    	protected Thread thread;
    	protected AbstractBackend rootBackend;
    	protected readonly ManualResetEvent resetEvent;
    
    	public bool IsExecute { private set; get; }

    	protected BackendThread(AbstractBackend root) {
    		threadTimestamp = ((int) DateTime.Now.Ticks << 1) >> 1;
    		ThreadIdentifier = $"THD[{threadTimestamp:X8}]({threadCount})";
    		threadCount++;
    		rootBackend = root;
    		resetEvent = new ManualResetEvent(false);
    		IsExecute = false;
    	}
    
    	public void StartExecute() {
    		if (IsExecute) return;
    		thread?.Abort();
    		thread = new Thread(ExecuteThread);
    		ResumeExecute();
    		IsExecute = true;
    		BeforeStart();
    		thread.Start();
    	}
    
    	protected virtual void BeforeStart() { }

    	public void StopExecute() {
    		if (!IsExecute) return;
    		IsExecute = false;
    		thread.Abort();
    		ResumeExecute();
    		thread = null;
    	}
    
    	public void ResetExecute() {
    		if (IsExecute) return;
    		if (thread.IsAlive) {
    			thread.Abort();
    			ResumeExecute();
    		}
    		StartExecute();
    	}

    	public void PauseExecute() {
    		resetEvent.Reset();
    	}

    	public void ResumeExecute() {
    		resetEvent.Set();
    	}

    	protected virtual void ExecuteThread() {
    		while (IsExecute) {
    			OnExecuteUpdate();
    			resetEvent.WaitOne();
    		}
    	}

    	protected abstract void OnExecuteUpdate();
    }
}
