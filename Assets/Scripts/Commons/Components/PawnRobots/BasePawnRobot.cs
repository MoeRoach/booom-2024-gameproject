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

	protected PawnRootControl pawnRoot;
	protected ObjectRootControl objectRoot;
	protected FacilityRootControl facilityRoot;
	
	protected StagePathFinder pathFinder;
	protected Queue<Square> pathQueue;
	protected StagePathFinder.PathState pathState;

	protected Queue<PawnOperation> operations;
	protected PawnOperation currOperation;
	protected CancellationTokenSource opCancel;
	protected bool isExecute;

	protected bool isActive;

	protected float moveSpeed = 2f;

	public BasePawnRobot(BasePawnController src) {
		owner = src;
	}

	public virtual void Initialize() {
		stateMachine = new SimpleStateMachine();
		dataService = GameSystem.Instance.GameData;
		pawnRoot = PawnRootControl.Instance;
		objectRoot = ObjectRootControl.Instance;
		facilityRoot = FacilityRootControl.Instance;
		pathFinder = StagePathFinder.Instance;
		pathQueue = new Queue<Square>();
		operations = new Queue<PawnOperation>();
		pathState = StagePathFinder.PathState.Wait;
		isActive = true;
		SetupMachine();
	}

	protected virtual void SetupMachine() {
		stateMachine.SetupState(RobotState.Waiting)
			.OnEnterState(EnterWaiting).OnUpdateState(UpdateWaiting).OnExitState(ExitWaiting);
		stateMachine.SetupState(RobotState.Thinking)
			.OnEnterState(EnterThinking).OnUpdateState(UpdateThinking).OnExitState(ExitThinking);
		stateMachine.SetupState(RobotState.Executing)
			.OnEnterState(EnterExecuting).OnUpdateState(UpdateExecuting).OnExitState(ExitExecuting);
		stateMachine.EntryState(RobotState.Waiting);
	}
	
	public void SetupActive(bool act) {
		isActive = act;
	}
	
	#region State Machine Functions
	
	protected virtual void EnterWaiting() { }

	protected virtual void UpdateWaiting() {
		if (!isActive || !CanDecide()) return;
		stateMachine.ChangeState(RobotState.Thinking);
	}

	protected virtual bool CanDecide() {
		return true;
	}
	
	protected virtual void ExitWaiting() { }
	
	protected virtual void EnterThinking() { }

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
	
	protected virtual void ExitThinking() { }
	
	protected virtual void EnterExecuting() { }

	protected virtual void UpdateExecuting() {
		if (isExecute) {
			if (operations.Count <= 0) return;
			var peekOp = operations.Peek();
			if (!peekOp.priority || currOperation != null && currOperation.priority) return;
			// 可以打断当前异步
			opCancel?.Cancel();
			isExecute = false;
		}
		
		if (!isActive || operations.Count <= 0) {
			stateMachine.ChangeState(RobotState.Waiting);
			return;
		}
		
		currOperation = operations.Dequeue();
		ProcessOperation(currOperation);
	}
	
	protected virtual void ExitExecuting() { }

	#endregion

	public virtual void OnRobotUpdate() {
		stateMachine.UpdateState();
	}

	public virtual bool IsAvailiable() {
		return operations.Count <= 0;
	}

	public virtual void RequestOperation(PawnOperation op) {
		operations.Enqueue(op);
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
		request.checkWalkable = CheckMapTileWalkable;
		request.getDistance = GetTileDistance;
		pathFinder.RequestForPath(request);
		pathState = StagePathFinder.PathState.Finding;
		while (pathState == StagePathFinder.PathState.Finding) await UniTask.Yield(cts.Token);
		pathState = StagePathFinder.PathState.Wait;
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

	protected virtual bool CheckMapTileWalkable(int id, Square dst, Square area) {
		for (var x = 0; x < area.x; x++) {
			for (var y = 0; y < area.y; y++) {
				var sq = new Square(dst.x + x, dst.y + y);
				if (!dataService.stageData.CheckTileExist(sq)) return false;
				if (dataService.stageEntityData.CheckCoordOccupy(sq)) return false;
			}
		}

		return true;
	}

	protected virtual int GetTileDistance(Square src, Square dst) {
		var delta = dst - src;
		return delta.Clength;
	}

	protected virtual void OnPathCallback(List<Square> path) {
		pathState = StagePathFinder.PathState.Done;
		pathQueue.Clear();
		foreach (var point in path) {
			pathQueue.Enqueue(point);
		}
	}

	protected virtual void OnOperationDone() {
		isExecute = false;
	}

	public void CancelOperation() {
		opCancel?.Cancel();
	}

	protected PawnOperation CreateOperation(string op) {
		return new PawnOperation(owner.Id) {op = op};
	}

	protected enum RobotState {
		Thinking,
		Executing,
		Waiting
	}
}