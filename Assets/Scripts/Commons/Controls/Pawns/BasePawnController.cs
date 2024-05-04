﻿// File create date:2024/4/18
using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using RoachFramework;
using UnityEngine;
// Created By Yu.Liu
public abstract class BasePawnController : HierarchyStateMachine  {

	public enum PawnState {
		Idle,
		Wait,
		Move,
		Dead
	}

	protected SpriteRenderer avatarSprite;
	protected AnimatorStateMachine animatorMachine;
	protected BasePawnRobot robot;

	protected GameDataService gameData;
	protected bool isGenerate;
	
	public int Id { get; private set; }
	public Square Coord { get; protected set; }
	public Square Area { get; protected set; }
	public int FaceDirection { get; protected set; }

	public virtual bool IsAvailiable => robot?.IsAvailiable() ?? false;

	protected static readonly int hashStateIdle = AnimeUtils.GetBaseStateHash(AnimeUtils.AnimatorStateNameIdle);
	protected static readonly int hashStateWait = AnimeUtils.GetBaseStateHash(AnimeUtils.AnimatorStateNameWait);
	protected static readonly int hashStateMove = AnimeUtils.GetBaseStateHash(AnimeUtils.AnimatorStateNameMove);

	protected override void OnMachineAwake() {
		base.OnMachineAwake();
		FaceDirection = CoordinateUtils.SQR_DIR_LEFT;
		avatarSprite = FindComponent<SpriteRenderer>("Avatar");
		animatorMachine = avatarSprite.GetComponent<AnimatorStateMachine>();
	}

	protected override void OnMachineStart() {
		base.OnMachineStart();
		gameData = GameSystem.Instance.GameData;
		robot = CreateRobot();
		robot?.Initialize();
		SetupMachine();
	}

	protected abstract BasePawnRobot CreateRobot();

	protected virtual void SetupMachine() {
		SetupState(PawnState.Idle).OnEntry(EnterIdle).OnUpdate(UpdateIdle).OnExit(ExitIdle);
		SetupState(PawnState.Wait).OnEntry(EnterWait).OnUpdate(UpdateWait).OnExit(ExitWait);
		SetupState(PawnState.Move).OnEntry(EnterMove).OnUpdate(UpdateMove).OnExit(ExitMove);
		SetupState(PawnState.Dead).OnEntry(EnterDead).OnUpdate(UpdateDead).OnExit(ExitDead);
		SetupEntryState(PawnState.Idle);
	}

	protected override void OnLazyStart() {
		base.OnLazyStart();
		if (!isGenerate) OnPawnGenerated();
		StartMachine();
	}

	protected override void OnMachineUpdate() {
		base.OnMachineUpdate();
		robot?.OnRobotUpdate();
	}

	#region State Machine Functions

	protected virtual void EnterIdle() {
		animatorMachine.Animator.Play(hashStateIdle);
	}
	protected virtual void UpdateIdle() { }
	protected virtual void ExitIdle() { }
	
	protected virtual void EnterWait() {
		animatorMachine.Animator.Play(hashStateWait);
	}
	protected virtual void UpdateWait() { }
	protected virtual void ExitWait() { }

	protected virtual void EnterMove() {
		animatorMachine.Animator.Play(hashStateMove);
	}
	protected virtual void UpdateMove() { }
	protected virtual void ExitMove() { }

	protected virtual void EnterDead() {
		YieldAction(() => {
			Destroy(gameObject);
		});
	}
	protected virtual void UpdateDead() { }
	protected virtual void ExitDead() { }

	#endregion

	public void SetupIdentifier(int id) {
		Id = id;
	}

	public void SetupCoord(Square coord) {
		Coord = coord;
		transform.position = MapUtils.SquareToWorld(Coord);
	}

	public void SetupArea(Square area) {
		Area = area;
	}

	public void NotifyGenerate() {
		isGenerate = true;
		NextFrameAction(OnPawnGenerated);
	}

	public virtual void GetAttack() { }

	protected virtual void OnPawnGenerated() {
		gameData.stageEntityData.RegisterPawn(this);
	}

	protected virtual void UpdateCoord() {
		Coord = MapUtils.WorldToSquare(transform.position);
	}

	protected virtual void RotateTo(int dir) {
		FaceDirection = dir;
		avatarSprite.flipX = FaceDirection switch {
			CoordinateUtils.SQR_DIR_RIGHT => true,
			CoordinateUtils.SQR_DIR_LEFT => false,
			_ => avatarSprite.flipX
		};
	}

	public virtual void BackToIdle() {
		if (CurrentState.Equals(PawnState.Idle)) return;
		ChangeState(PawnState.Idle);
	}

	public virtual async UniTask AsyncWait(float time, CancellationTokenSource cts) {
		if (!CurrentState.Equals(PawnState.Wait)) ChangeState(PawnState.Wait);
		var ms = Mathf.RoundToInt(time * 1000f);
		await UniTask.Delay(ms, false, PlayerLoopTiming.Update, cts.Token);
	}

	public virtual async UniTask AsyncMove(Vector3 target, float spd, CancellationTokenSource cts) {
		var start = transform.position;
		var delta = target - start;
		var invt = spd / delta.magnitude;
		RotateTo(MapUtils.GetDirectionWithVector(delta));
		if (!CurrentState.Equals(PawnState.Move)) ChangeState(PawnState.Move);
		var timer = 0f;
		while (timer < 1f) {
			timer += Time.deltaTime * invt;
			transform.position = Vector3.Lerp(start, target, timer);
			await UniTask.Yield(cts.Token);
		}
		
		UpdateCoord();
	}

	public virtual UniTask AsyncCollect(GameObject target, float time, CancellationTokenSource cts) {
		return UniTask.CompletedTask;
	}

	public virtual UniTask AsyncCarryon(GameObject target, CancellationTokenSource cts) {
		// 把东西带上
		return UniTask.CompletedTask;
	}
	
	public virtual UniTask AsyncPutdown(GameObject target, CancellationTokenSource cts) {
		// 把东西放下
		return UniTask.CompletedTask;
	}

	public virtual UniTask AsyncBreak(GameObject target, CancellationTokenSource cts) {
		// 把手里的东西销毁
		return UniTask.CompletedTask;
	}
	
	public virtual UniTask AsyncProduce(GameObject target, float time, CancellationTokenSource cts) {
		return UniTask.CompletedTask;
	}
	
	public virtual UniTask AsyncAttack(GameObject target, CancellationTokenSource cts) {
		// 攻击目标，播放攻击动画，等待事件处理和动画完结，然后再返回
		return UniTask.CompletedTask;
	}
	
	public virtual UniTask AsyncThink(GameObject target, float time, CancellationTokenSource cts) {
		return UniTask.CompletedTask;
	}
	
	public virtual UniTask AsyncHappy(GameObject target, float time, CancellationTokenSource cts) {
		return UniTask.CompletedTask;
	}
	
	public virtual UniTask AsyncFreeze(GameObject target, float time, CancellationTokenSource cts) {
		return UniTask.CompletedTask;
	}
	
	public virtual UniTask AsyncThunder(GameObject target, float time, CancellationTokenSource cts) {
		return UniTask.CompletedTask;
	}
	
	public virtual UniTask AsyncMeteor(GameObject target, float time, CancellationTokenSource cts) {
		return UniTask.CompletedTask;
	}

	protected override void Release() {
		base.Release();
		robot?.CancelOperation();
		gameData.stageEntityData.UnregisterPawn(this);
	}
}