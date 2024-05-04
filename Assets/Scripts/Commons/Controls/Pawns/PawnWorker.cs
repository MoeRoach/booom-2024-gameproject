﻿// File create date:2024/4/28
using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
// Created By Yu.Liu
public class PawnWorker : BasePawnController  {

	private enum WorkerState {
		Collect,
		Produce
	}

	private bool _isSummon = true;
	private bool _isCarry = false;
	
	private static readonly int hashStateSummon = AnimeUtils.GetBaseStateHash(AnimeUtils.AnimatorStateNameSummon);
	private static readonly int hashStateCarry = AnimeUtils.GetBaseStateHash(AnimeUtils.AnimatorStateNameCarry);
	private static readonly int hashStateWork = AnimeUtils.GetBaseStateHash(AnimeUtils.AnimatorStateNameWork);
	
	protected override BasePawnRobot CreateRobot() {
		return new WorkerRobot(this);
	}

	protected override void SetupMachine() {
		SetupState(WorkerState.Collect).OnEntry(EnterCollect).OnUpdate(UpdateCollect).OnExit(ExitCollect);
		SetupState(WorkerState.Produce).OnEntry(EnterProduce).OnUpdate(UpdateProduce).OnExit(ExitProduce);
		base.SetupMachine();
	}

	#region State Machine Functions

	protected override void EnterIdle() {
		animatorMachine.Animator.Play(_isSummon ? hashStateSummon : hashStateIdle);
		_isSummon = false;
	}

	protected override void EnterMove() {
		animatorMachine.Animator.Play(_isCarry ? hashStateCarry : hashStateMove);
	}

	private void EnterCollect() {
		animatorMachine.Animator.Play(hashStateWork);
	}
	private void UpdateCollect() { }
	private void ExitCollect() { }

	private void EnterProduce() {
		animatorMachine.Animator.Play(hashStateWork);
	}
	private void UpdateProduce() { }
	private void ExitProduce() { }

	#endregion

	public override async UniTask AsyncCollect(GameObject target, float time, CancellationTokenSource cts) {
		var delta = target.transform.position - transform.position;
		RotateTo(MapUtils.GetDirectionWithVector(delta));
		if (!CurrentState.Equals(WorkerState.Collect)) ChangeState(WorkerState.Collect);
		var timer = 0f;
		while (timer < time) {
			timer += Time.deltaTime;
			await UniTask.Yield(cts.Token);
		}
	}

	public override async UniTask AsyncProduce(GameObject target, float time, CancellationTokenSource cts) {
		var delta = target.transform.position - transform.position;
		RotateTo(MapUtils.GetDirectionWithVector(delta));
		if (!CurrentState.Equals(WorkerState.Produce)) ChangeState(WorkerState.Produce);
		var timer = 0f;
		while (timer < time) {
			timer += Time.deltaTime;
			await UniTask.Yield(cts.Token);
		}
	}

	public override UniTask AsyncCarryon(GameObject target, CancellationTokenSource cts) {
		return base.AsyncCarryon(target, cts);
	}
}