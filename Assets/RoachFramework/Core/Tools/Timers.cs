using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoachFramework {
    /// <summary>
	/// 基础计时器
	/// </summary>
	public class BaseTimer {
		
		public float TargetTime { get; protected set; }
		public float CurrentTime { get; protected set; }
		public bool Done { get; protected set; }
		public virtual float Percentage => TargetTime <= 0f ? 1f : CurrentTime / TargetTime;
		
		public event Action OnTimerFinish;
		
		public BaseTimer(float length, bool startDone = false) {
			TargetTime = length;
			if (!startDone) {
				CurrentTime = 0f;
				Done = false;
			} else {
				CurrentTime = TargetTime;
				Done = true;
			}
		}
		
		/// <summary>
		/// 计时器时间更新
		/// </summary>
		/// <param name="deltaTime">更新时长</param>
		public virtual void UpdateTimer(float deltaTime) {
			if (Done) return;
			CurrentTime += deltaTime;
			if (CurrentTime < TargetTime) return;
			Done = true;
			TriggerFinishEvent();
		}

		protected void TriggerFinishEvent() {
			OnTimerFinish?.Invoke();
		}
		
		/// <summary>
		/// 修改计时器目标时间
		/// </summary>
		/// <param name="time">新的目标时间</param>
		public virtual void ChangeTargetTime(float time) {
			TargetTime = time;
			if (CurrentTime > TargetTime) {
				CurrentTime = TargetTime;
			}
		}
		
		/// <summary>
		/// 重设计时器
		/// </summary>
		public virtual void ResetTimer() {
			CurrentTime = 0f;
			Done = false;
		}

		/// <summary>
		/// 结束计时器
		/// </summary>
		public virtual void FinishTimer() {
			CurrentTime = TargetTime;
			Done = true;
			OnTimerFinish?.Invoke();
		}
	}
	
	/// <summary>
	/// 倒计时器，用于倒数计时
	/// </summary>
	public class CountdownTimer : BaseTimer {
		
		public CountdownTimer(float length, bool startDone = false) : base(length, !startDone) {
			if (!startDone) {
				CurrentTime = TargetTime;
				Done = false;
			} else {
				CurrentTime = 0f;
				Done = true;
			}
		}

		public override void UpdateTimer(float deltaTime) {
			if (Done) return;
			CurrentTime -= deltaTime;
			if (CurrentTime > 0f) return;
			Done = true;
			TriggerFinishEvent();
		}

		public override void ResetTimer() {
			CurrentTime = TargetTime;
			Done = false;
		}

		public override void FinishTimer() {
			CurrentTime = 0f;
			Done = true;
		}
	}
	
	/// <summary>
	/// 可添加检查点回调的计时器
	/// </summary>
	public class CheckPointTimer : BaseTimer {

		protected readonly List<CheckPoint> checkPointList;

		public CheckPointTimer(float length, bool startDone = false) : base(
			length, startDone) {
			checkPointList = new List<CheckPoint>();
		}

		public override void UpdateTimer(float deltaTime) {
			if (Done) return;
			CurrentTime += deltaTime;
			if (CurrentTime >= TargetTime) {
				Done = true;
				TriggerFinishEvent();
			} else {
				EvaluateCheckPoints(CurrentTime);
			}
		}

		protected virtual void EvaluateCheckPoints(float current) {
			foreach (var point in checkPointList) {
				if (point.isChecked) continue;
				if (point.timePoint > current) continue;
				point.checkCallback?.Invoke(current);
				point.isChecked = true;
			}
		}

		protected virtual void ResetCheckPoints() {
			foreach (var point in checkPointList) {
				point.isChecked = false;
			}
		}

		public virtual void RegisterCheckPoint(float tp, Action<float> callback) {
			var point = new CheckPoint {timePoint = tp, checkCallback = callback};
			var isInserted = false;
			for (var i = 0; i < checkPointList.Count; i++) {
				if (checkPointList[i].timePoint <= point.timePoint) continue;
				checkPointList.Insert(i, point);
				isInserted = true;
				break;
			}

			if (!isInserted) {
				checkPointList.Add(point);
			}
		}

		public virtual void UnregisterCheckPoint(float tp, Action<float> callback) {
			for (var i = 0; i < checkPointList.Count; i++) {
				if (!checkPointList[i].timePoint.Equals(tp) || 
				    checkPointList[i].checkCallback != callback) continue;
				checkPointList.RemoveAt(i);
				break;
			}
		}

		public override void ResetTimer() {
			base.ResetTimer();
			ResetCheckPoints();
		}

		/// <summary>
		/// 检查点回调方法类
		/// </summary>
		protected class CheckPoint {
			public float timePoint;
			public Action<float> checkCallback;
			public bool isChecked;
		}
	}
	
	/// <summary>
	/// 带检查点的倒数计时器
	/// </summary>
	public class CheckPointCountdownTimer : CountdownTimer {
		
		protected readonly List<CheckPoint> checkPointList;
		
		public CheckPointCountdownTimer(float length, bool startDone = false) :
			base(length, startDone) {
			checkPointList = new List<CheckPoint>();
		}
		
		public override void UpdateTimer(float deltaTime) {
			if (Done) return;
			CurrentTime -= deltaTime;
			if (CurrentTime <= 0f) {
				Done = true;
				TriggerFinishEvent();
			} else {
				EvaluateCheckPoints(CurrentTime);
			}
		}

		protected virtual void EvaluateCheckPoints(float current) {
			foreach (var point in checkPointList) {
				if (point.isChecked) continue;
				if (point.timePoint < current) continue;
				point.checkCallback?.Invoke(current);
				point.isChecked = true;
			}
		}

		protected virtual void ResetCheckPoints() {
			foreach (var point in checkPointList) {
				point.isChecked = false;
			}
		}

		public virtual void RegisterCheckPoint(float tp, Action<float> callback) {
			var point = new CheckPoint {timePoint = tp, checkCallback = callback};
			var isInserted = false;
			for (var i = 0; i < checkPointList.Count; i++) {
				if (checkPointList[i].timePoint >= point.timePoint) continue;
				checkPointList.Insert(i, point);
				isInserted = true;
				break;
			}

			if (!isInserted) {
				checkPointList.Add(point);
			}
		}

		public virtual void UnregisterCheckPoint(float tp, Action<float> callback) {
			for (var i = 0; i < checkPointList.Count; i++) {
				if (!checkPointList[i].timePoint.Equals(tp) || 
				    checkPointList[i].checkCallback != callback) continue;
				checkPointList.RemoveAt(i);
				break;
			}
		}

		public override void ResetTimer() {
			base.ResetTimer();
			ResetCheckPoints();
		}
		
		/// <summary>
		/// 检查点回调方法类
		/// </summary>
		protected class CheckPoint {
			public float timePoint;
			public Action<float> checkCallback;
			public bool isChecked;
		}
	}
}
