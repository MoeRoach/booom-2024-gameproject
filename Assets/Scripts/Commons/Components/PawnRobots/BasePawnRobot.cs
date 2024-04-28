﻿// File create date:2024/4/19
using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using RoachFramework;
using UnityEngine;
// Created By Yu.Liu
public abstract class BasePawnRobot {

	protected SimpleStateMachine stateMachine;

	protected BasePawnController owner;
	protected GameDataService dataService;
	protected StagePathFinder pathFinder;
	protected Queue<Square> pathQueue;
	protected PathState pathState;

	protected Queue<PawnOperation> operations;
	protected PawnOperation currOperation;
	protected CancellationTokenSource opCancel;
	protected bool isExecute;

	protected bool isActive;

	protected float moveSpeed = 2f;

	public BasePawnRobot(BasePawnController src) {
		owner = src;
	}

	public void Initialize() {
		stateMachine = new SimpleStateMachine();
		dataService = GameSystem.Instance.GameData;
		pathFinder = StagePathFinder.Instance;
		pathQueue = new Queue<Square>();
		operations = new Queue<PawnOperation>();
		pathState = PathState.Idle;
		isActive = true;
		SetupMachine();
	}

	protected virtual void SetupMachine() {
		stateMachine.SetupState(RobotState.Waiting).OnUpdateState(UpdateWaiting);
		stateMachine.SetupState(RobotState.Thinking).OnUpdateState(UpdateThinking);
		stateMachine.SetupState(RobotState.Executing).OnUpdateState(UpdateExecuting);
		stateMachine.EntryState(RobotState.Waiting);
	}
	
	public void SetupActive(bool act) {
		isActive = act;
	}
	
	#region State Machine Functions

	protected virtual void UpdateWaiting() {
		if (!isActive || !CanDecide()) return;
		stateMachine.ChangeState(RobotState.Thinking);
	}

	protected virtual bool CanDecide() {
		return true;
	}

	protected virtual void UpdateThinking() {
		if (!isActive) stateMachine.ChangeState(RobotState.Waiting);
		stateMachine.ChangeState(!MakeDecision() ? RobotState.Waiting : RobotState.Executing);
	}
	
	private bool MakeDecision() {
		if (PreDescision()) return false;
		Decide();
		PostDecision();
		return true;
	}

	protected virtual bool PreDescision() {
		return false;
	}

	protected abstract void Decide();

	protected virtual void PostDecision() { }

	protected virtual void UpdateExecuting() {
		if (!isActive) stateMachine.ChangeState(RobotState.Waiting);
		if (isExecute) {
			if (operations.Count <= 0) {
				stateMachine.ChangeState(RobotState.Waiting);
				return;
			}
			
			var peekOp = operations.Peek();
			if (currOperation.priority || !peekOp.priority) return;
			// 可以打断当前异步
			opCancel?.Cancel();
			isExecute = false;
		}
		
		var nextOp = operations.Dequeue();
		ProcessOperation(nextOp);
	}

	#endregion

	public virtual void OnRobotUpdate() {
		stateMachine.UpdateState();
	}

	private async void ProcessOperation(PawnOperation op) {
		opCancel = new CancellationTokenSource();
		isExecute = true;
		await DoOperation(op, opCancel);
		OnOperationDone();
	}

	protected virtual async UniTask DoOperation(PawnOperation op, CancellationTokenSource cts) {
		switch (op.op) {
			case PawnOperateConfigs.OpMoveto:
				var targetCoord = op.GetExtra<Square>(PawnOperateConfigs.ExtraKeyCoordinate);
				if (targetCoord == null) return;
				await DoMoveto(targetCoord, cts);
				break;
			case PawnOperateConfigs.OpCollect:
				await DoCollect(cts);
				break;
			case PawnOperateConfigs.OpCarryon:
				await DoCarryon(cts);
				break;
			case PawnOperateConfigs.OpPutdown:
				await DoPutdown(cts);
				break;
			case PawnOperateConfigs.OpProduce:
				await DoProduce(cts);
				break;
			case PawnOperateConfigs.OpAttack:
				await DoAttack(cts);
				break;
			case PawnOperateConfigs.OpThink:
				await DoThink(cts);
				break;
			case PawnOperateConfigs.OpHappy:
				await DoHappy(cts);
				break;
			case PawnOperateConfigs.OpFreeze:
				await DoFreeze(cts);
				break;
			case PawnOperateConfigs.OpThunder:
				await DoThunder(cts);
				break;
			case PawnOperateConfigs.OpMeteor:
				await DoMeteor(cts);
				break;
		}
	}

	protected virtual async UniTask DoMoveto(Square tc, CancellationTokenSource cts) {
		var request = StagePathFinder.CreateRequest(owner.Id, owner.Coord, tc, owner.Area, OnPathCallback);
		pathFinder.RequestForPath(request);
		pathState = PathState.Finding;
		while (pathState == PathState.Finding) await UniTask.Yield(cts.Token);
		pathState = PathState.Idle;
		// 路径找到了，开始执行移动
		while (pathQueue.Count > 0) {
			var wp = pathQueue.Dequeue();
			var tpos = MapUtils.SquareToWorld(wp);
			if (owner.transform.position == tpos) continue;
			await owner.AsyncMove(tpos, moveSpeed, cts);
		}
		// 移动完成
	}

	protected virtual async UniTask DoCollect(CancellationTokenSource cts) {
		await owner.AsyncCollect(null, 2f, cts);
	}
	
	protected virtual async UniTask DoCarryon(CancellationTokenSource cts) {
		await owner.AsyncCarryon(null, cts);
	}
	
	protected virtual async UniTask DoPutdown(CancellationTokenSource cts) {
		await owner.AsyncPutdown(null, cts);
	}
	
	protected virtual async UniTask DoProduce(CancellationTokenSource cts) {
		await owner.AsyncProduce(null, 2f, cts);
	}
	
	protected virtual async UniTask DoAttack(CancellationTokenSource cts) {
		await owner.AsyncAttack(null, cts);
	}
	
	protected virtual async UniTask DoThink(CancellationTokenSource cts) {
		await owner.AsyncThink(null, 2f, cts);
	}
	
	protected virtual async UniTask DoHappy(CancellationTokenSource cts) {
		await owner.AsyncHappy(null, 2f, cts);
	}
	
	protected virtual async UniTask DoFreeze(CancellationTokenSource cts) {
		await owner.AsyncFreeze(null, 2f, cts);
	}
	
	protected virtual async UniTask DoThunder(CancellationTokenSource cts) {
		await owner.AsyncThunder(null, 2f, cts);
	}
	
	protected virtual async UniTask DoMeteor(CancellationTokenSource cts) {
		await owner.AsyncMeteor(null, 2f, cts);
	}

	protected virtual void OnPathCallback(List<Square> path) {
		pathState = PathState.Done;
		pathQueue.Clear();
		foreach (var point in path) {
			pathQueue.Enqueue(point);
		}
	}

	protected virtual void OnOperationDone() {
		isExecute = false;
	}

	protected enum RobotState {
		Thinking,
		Executing,
		Waiting
	}

	public enum PathState {
		Idle,
		Finding,
		Done
	}
}