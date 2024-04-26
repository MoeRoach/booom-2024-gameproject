// File create date:2024/4/19
using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
// Created By Yu.Liu
public abstract class BasePawnRobot {

	protected BasePawnController owner;
	protected GameDataService dataService;

	protected Queue<PawnOperation> operations;
	protected PawnOperation currOperation;
	protected CancellationTokenSource opCancel;
	protected bool isExecute;

	protected bool isActive;

	public BasePawnRobot(BasePawnController src) {
		owner = src;
	}

	public void Initialize() {
		dataService = GameSystem.Instance.GameData;
		operations = new Queue<PawnOperation>();
		isActive = true;
	}

	public virtual void OnRobotUpdate() {
		if (!isActive) return;
		if (isExecute) {
			if (operations.Count <= 0) return;
			var peekOp = operations.Peek();
			if (currOperation.priority || !peekOp.priority) return;
			// 可以打断当前异步
			opCancel?.Cancel();
			isExecute = false;
		}
		
		var nextOp = operations.Dequeue();
		ProcessOperation(nextOp);
	}

	private async void ProcessOperation(PawnOperation op) {
		opCancel = new CancellationTokenSource();
		isExecute = true;
		await DoOperation(opCancel);
		OnOperationDone();
	}

	protected abstract UniTask DoOperation(CancellationTokenSource cts);

	protected virtual void OnOperationDone() {
		isExecute = false;
	}

	public void SetupActive(bool act) {
		isActive = act;
	}

	protected void MakeDecision() {
		if (PreDescision()) return;
		Decide();
		PostDecision();
	}

	protected virtual bool PreDescision() {
		return false;
	}

	protected abstract void Decide();

	protected virtual void PostDecision() {
		
	}

	// public enum PawnAction {
	// 	Wait,
	// 	Wander,
	// 	Moveto,
	// 	Collect,
	// 	Carryon,
	// 	Putdown,
	// 	Attack,
	// 	Escape,
	// 	Hide
	// }
}